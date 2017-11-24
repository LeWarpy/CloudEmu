﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Cloud.Core;
using Cloud.HabboHotel.Items;
using Cloud.HabboHotel.Rooms.Pathfinding;
using Cloud.HabboHotel.Groups;
using Cloud.HabboHotel.Rooms.Games.Teams;
using System.Collections.Concurrent;
using Cloud.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Cloud.HabboHotel.Rooms
{
	public class Gamemap
	{
		private Room _room;
		private byte[,] mGameMap;//0 = none, 1 = pool, 2 = normal skates, 3 = ice skates

		public bool gotPublicPool;
		public bool DiagonalEnabled;
		private RoomModel mStaticModel;
		private byte[,] mUserItemEffect;
		private double[,] mItemHeightMap;
		private DynamicRoomModel mDynamicModel;
		private ConcurrentDictionary<Point, List<int>> mCoordinatedItems;
		private ConcurrentDictionary<Point, List<RoomUser>> userMap;

		public Gamemap(Room room)
		{
			this._room = room;
			this.DiagonalEnabled = true;

			mStaticModel = CloudServer.GetGame().GetRoomManager().GetModel(room.ModelName);
			if (mStaticModel == null)
			{
				CloudServer.GetGame().GetRoomManager().LoadModel(room.ModelName);
				mStaticModel = CloudServer.GetGame().GetRoomManager().GetModel(room.ModelName);
			}

			if (mStaticModel == null)
				return;

			mDynamicModel = new DynamicRoomModel(mStaticModel);

			mCoordinatedItems = new ConcurrentDictionary<Point, List<int>>();


			gotPublicPool = room.RoomData.Model.gotPublicPool;
			mGameMap = new byte[Model.MapSizeX, Model.MapSizeY];
			mItemHeightMap = new double[Model.MapSizeX, Model.MapSizeY];

			userMap = new ConcurrentDictionary<Point, List<RoomUser>>();
		}

		public void AddUserToMap(RoomUser user, Point coord)
		{
			if (userMap.ContainsKey(coord))
			{
				((List<RoomUser>)userMap[coord]).Add(user);
			}
			else
			{
				List<RoomUser> users = new List<RoomUser>
				{
					user
				};
				userMap.TryAdd(coord, users);
			}
		}

		public void TeleportToItem(RoomUser user, Item item)
		{
			if (item == null || user == null)
				return;

			GameMap[user.X, user.Y] = user.SqState;
			UpdateUserMovement(new Point(user.Coordinate.X, user.Coordinate.Y), new Point(item.Coordinate.X, item.Coordinate.Y), user);
			user.X = item.GetX;
			user.Y = item.GetY;
			user.Z = item.GetZ;

			user.SqState = GameMap[item.GetX, item.GetY];
			GameMap[user.X, user.Y] = 1;
			user.RotBody = item.Rotation;
			user.RotHead = item.Rotation;

			user.GoalX = user.X;
			user.GoalY = user.Y;
			user.SetStep = false;
			user.IsWalking = false;
			user.UpdateNeeded = true;
		}

		public void UpdateUserMovement(Point oldCoord, Point newCoord, RoomUser user)
		{
			RemoveUserFromMap(user, oldCoord);
			AddUserToMap(user, newCoord);
		}

		public void RemoveUserFromMap(RoomUser user, Point coord)
		{
			if (userMap.ContainsKey(coord))
				((List<RoomUser>)userMap[coord]).RemoveAll(x => x != null && x.VirtualId == user.VirtualId);
		}

		public bool MapGotUser(Point coord)
		{
			return (GetRoomUsers(coord).Count > 0);
		}

		public List<RoomUser> GetRoomUsers(Point coord)
		{
			if (userMap.ContainsKey(coord))
				return (List<RoomUser>)userMap[coord];
			else
				return new List<RoomUser>();
		}

		public Point GetRandomWalkableSquare()
		{
			var walkableSquares = new List<Point>();
			for (int y = 0; y < mGameMap.GetUpperBound(1); y++)
			{
				for (int x = 0; x < mGameMap.GetUpperBound(0); x++)
				{
					if (mStaticModel.DoorX != x && mStaticModel.DoorY != y && mGameMap[x, y] == 1)
						walkableSquares.Add(new Point(x, y));
				}
			}

			int RandomNumber = CloudServer.GetRandomNumber(0, walkableSquares.Count);
			int i = 0;

			foreach (Point coord in walkableSquares.ToList())
			{
				if (i == RandomNumber)
					return coord;
				i++;
			}

			return new Point(0, 0);
		}


		public bool IsInMap(int X, int Y)
		{
			var walkableSquares = new List<Point>();
			for (int y = 0; y < mGameMap.GetUpperBound(1); y++)
			{
				for (int x = 0; x < mGameMap.GetUpperBound(0); x++)
				{
					if (mStaticModel.DoorX != x && mStaticModel.DoorY != y && mGameMap[x, y] == 1)
						walkableSquares.Add(new Point(x, y));
				}
			}

			if (walkableSquares.Contains(new Point(X, Y)))
				return true;
			return false;
		}

		public void AddToMap(Item item)
		{
			AddItemToMap(item);
		}

		private void SetDefaultValue(int x, int y)
		{
			mGameMap[x, y] = 0;
			mUserItemEffect[x, y] = 0;
			mItemHeightMap[x, y] = 0.0;

			if (x == Model.DoorX && y == Model.DoorY)
			{
				mGameMap[x, y] = 3;
			}
			else if (Model.SqState[x, y] == SquareState.OPEN)
			{
				mGameMap[x, y] = 1;
			}
			else if (Model.SqState[x, y] == SquareState.SEAT)
			{
				mGameMap[x, y] = 2;
			}
		}

		public void UpdateMapForItem(Item item)
		{
			RemoveFromMap(item);
			AddToMap(item);
		}

		public void GenerateMaps(bool checkLines = true)
		{
			int MaxX = 0;
			int MaxY = 0;
			mCoordinatedItems = new ConcurrentDictionary<Point, List<int>>();

			if (checkLines)
			{
				Item[] items = _room.GetRoomItemHandler().GetFloor.ToArray();
				foreach (Item item in items.ToList())
				{
					if (item == null)
						continue;

					if (item.GetX > Model.MapSizeX && item.GetX > MaxX)
						MaxX = item.GetX;
					if (item.GetY > Model.MapSizeY && item.GetY > MaxY)
						MaxY = item.GetY;
				}

				Array.Clear(items, 0, items.Length);
				items = null;
			}

			#region Dynamic game map handling

			if (MaxY > (Model.MapSizeY - 1) || MaxX > (Model.MapSizeX - 1))
			{
				if (MaxX < Model.MapSizeX)
					MaxX = Model.MapSizeX;
				if (MaxY < Model.MapSizeY)
					MaxY = Model.MapSizeY;

				Model.SetMapsize(MaxX + 7, MaxY + 7);
				GenerateMaps(false);
				return;
			}

			if (MaxX != StaticModel.MapSizeX || MaxY != StaticModel.MapSizeY)
			{
				mUserItemEffect = new byte[Model.MapSizeX, Model.MapSizeY];
				mGameMap = new byte[Model.MapSizeX, Model.MapSizeY];


				mItemHeightMap = new double[Model.MapSizeX, Model.MapSizeY];
				//if (modelRemap)
				//    Model.Generate(); //Clears model

				for (int line = 0; line < Model.MapSizeY; line++)
				{
					for (int chr = 0; chr < Model.MapSizeX; chr++)
					{
						mGameMap[chr, line] = 0;
						mUserItemEffect[chr, line] = 0;

						if (chr == Model.DoorX && line == Model.DoorY)
						{
							mGameMap[chr, line] = 3;
						}
						else if (Model.SqState[chr, line] == SquareState.OPEN)
						{
							mGameMap[chr, line] = 1;
						}
						else if (Model.SqState[chr, line] == SquareState.SEAT)
						{
							mGameMap[chr, line] = 2;
						}
						else if (Model.SqState[chr, line] == SquareState.POOL)
						{
							mUserItemEffect[chr, line] = 6;
						}
					}
				}

				if (gotPublicPool)
				{
					for (int y = 0; y < StaticModel.MapSizeY; y++)
					{
						for (int x = 0; x < StaticModel.MapSizeX; x++)
						{
							if (StaticModel.mRoomModelfx[x, y] != 0)
							{
								mUserItemEffect[x, y] = StaticModel.mRoomModelfx[x, y];
							}
						}
					}
				}

				/** COMENTADO YA QUE SALAS PUBLICAS NUEVA CRYPTO NO NECESARIO
                if (!string.IsNullOrEmpty(StaticModel.StaticFurniMap)) 
                {
                     * foreach (PublicRoomSquare square in StaticModel.Furnis)
                    {
                        if (square.Content.Contains("chair") || square.Content.Contains("sofa"))
                        {
                            mGameMap[square.X, square.Y] = 1;
                        } else {
                            mGameMap[square.X, square.Y] = 0;
                        }
                    }
                }*/
			}
			#endregion

			#region Static game map handling

			else
			{
				//mGameMap
				//mUserItemEffect
				mUserItemEffect = new byte[Model.MapSizeX, Model.MapSizeY];
				mGameMap = new byte[Model.MapSizeX, Model.MapSizeY];


				mItemHeightMap = new double[Model.MapSizeX, Model.MapSizeY];
				//if (modelRemap)
				//    Model.Generate(); //Clears model

				for (int line = 0; line < Model.MapSizeY; line++)
				{
					for (int chr = 0; chr < Model.MapSizeX; chr++)
					{
						mGameMap[chr, line] = 0;
						mUserItemEffect[chr, line] = 0;

						if (chr == Model.DoorX && line == Model.DoorY)
						{
							mGameMap[chr, line] = 3;
						}
						else if (Model.SqState[chr, line] == SquareState.OPEN)
						{
							mGameMap[chr, line] = 1;
						}
						else if (Model.SqState[chr, line] == SquareState.SEAT)
						{
							mGameMap[chr, line] = 2;
						}
						else if (Model.SqState[chr, line] == SquareState.POOL)
						{
							mUserItemEffect[chr, line] = 6;
						}
					}
				}

				if (gotPublicPool)
				{
					for (int y = 0; y < StaticModel.MapSizeY; y++)
					{
						for (int x = 0; x < StaticModel.MapSizeX; x++)
						{
							if (StaticModel.mRoomModelfx[x, y] != 0)
							{
								mUserItemEffect[x, y] = StaticModel.mRoomModelfx[x, y];
							}
						}
					}
				}

				/** COMENTADO YA QUE SALAS PUBLICAS NUEVA CRYPTO NO NECESARIO
                 * foreach (PublicRoomSquare square in StaticModel.Furnis)
                {
                    if (square.Content.Contains("chair") || square.Content.Contains("sofa"))
                    {
                        mGameMap[square.X, square.Y] = 1;
                    }
                    else
                    {
                        mGameMap[square.X, square.Y] = 0;
                    }
                }*/
			}

			#endregion

			Item[] tmpItems = _room.GetRoomItemHandler().GetFloor.ToArray();
			foreach (Item Item in tmpItems.ToList())
			{
				if (Item == null)
					continue;

				if (!AddItemToMap(Item))
					continue;
			}
			Array.Clear(tmpItems, 0, tmpItems.Length);
			tmpItems = null;

			if (_room.RoomBlockingEnabled == 0)
			{
				foreach (RoomUser user in _room.GetRoomUserManager().GetUserList().ToList())
				{
					if (user == null)
						continue;

					user.SqState = mGameMap[user.X, user.Y];
					mGameMap[user.X, user.Y] = 0;
				}
			}

			try
			{
				mGameMap[Model.DoorX, Model.DoorY] = 3;
			}
			catch { }
		}

		private bool ConstructMapForItem(Item Item, Point Coord)
		{
			try
			{
				if (Coord.X > (Model.MapSizeX - 1))
				{
					Model.AddX();
					GenerateMaps();
					return false;
				}

				if (Coord.Y > (Model.MapSizeY - 1))
				{
					Model.AddY();
					GenerateMaps();
					return false;
				}

				if (Model.SqState[Coord.X, Coord.Y] == SquareState.BLOCKED)
				{
					Model.OpenSquare(Coord.X, Coord.Y, Item.GetZ);
				}
				if (mItemHeightMap[Coord.X, Coord.Y] <= Item.TotalHeight)
				{
					mItemHeightMap[Coord.X, Coord.Y] = Item.TotalHeight - mDynamicModel.SqFloorHeight[Item.GetX, Item.GetY];
					mUserItemEffect[Coord.X, Coord.Y] = 0;


					switch (Item.GetBaseItem().InteractionType)
					{
						case InteractionType.POOL:
							mUserItemEffect[Coord.X, Coord.Y] = 1;
							break;
						case InteractionType.NORMAL_SKATES:
							mUserItemEffect[Coord.X, Coord.Y] = 2;
							break;
						case InteractionType.ICE_SKATES:
							mUserItemEffect[Coord.X, Coord.Y] = 3;
							break;
						case InteractionType.lowpool:
							mUserItemEffect[Coord.X, Coord.Y] = 4;
							break;
						case InteractionType.haloweenpool:
							mUserItemEffect[Coord.X, Coord.Y] = 5;
							break;
						case InteractionType.SILLAGUIA:
							mUserItemEffect[Coord.X, Coord.Y] = 7;
							break;
					}


					//SwimHalloween
					if (Item.GetBaseItem().Walkable)    // If this item is walkable and on the floor, allow users to walk here.
					{
						if (mGameMap[Coord.X, Coord.Y] != 3)
							mGameMap[Coord.X, Coord.Y] = 1;
					}
                    else if (Item.GetBaseItem().InteractionType == InteractionType.GATE && Item.ExtraData == "1")// If this item is a gate, open, and on the floor, allow users to walk here.
                    {
						if (mGameMap[Coord.X, Coord.Y] != 3)
							mGameMap[Coord.X, Coord.Y] = 1;
					}
					else if (Item.GetBaseItem().IsSeat || Item.GetBaseItem().InteractionType == InteractionType.BED || Item.GetBaseItem().InteractionType == InteractionType.TENT_SMALL)
					{
						mGameMap[Coord.X, Coord.Y] = 3;
					}
					else // Finally, if it's none of those, block the square.
					{
						if (mGameMap[Coord.X, Coord.Y] != 3)
							mGameMap[Coord.X, Coord.Y] = 0;
					}
				}

				// Set bad maps
				if (Item.GetBaseItem().InteractionType == InteractionType.BED || Item.GetBaseItem().InteractionType == InteractionType.TENT_SMALL)
					mGameMap[Coord.X, Coord.Y] = 3;
			}
			catch (Exception e)
			{
				ExceptionLogger.LogException(e);
			}
			return true;
		}

		public void AddCoordinatedItem(Item item, Point coord)
		{
			List<int> Items = new List<int>(); //mCoordinatedItems[CoordForItem];

			if (!mCoordinatedItems.TryGetValue(coord, out Items))
			{
				Items = new List<int>();

				if (!Items.Contains(item.Id))
					Items.Add(item.Id);

				if (!mCoordinatedItems.ContainsKey(coord))
					mCoordinatedItems.TryAdd(coord, Items);
			}
			else
			{
				if (!Items.Contains(item.Id))
				{
					Items.Add(item.Id);
					mCoordinatedItems[coord] = Items;
				}
			}
		}

		public List<Item> GetCoordinatedItems(Point coord)
		{
			var point = new Point(coord.X, coord.Y);
			List<Item> Items = new List<Item>();

			if (mCoordinatedItems.ContainsKey(point))
			{
				List<int> Ids = mCoordinatedItems[point];
				Items = GetItemsFromIds(Ids);
				return Items;
			}

			return new List<Item>();
		}

		public bool RemoveCoordinatedItem(Item item, Point coord)
		{
			Point point = new Point(coord.X, coord.Y);
			if (mCoordinatedItems != null && mCoordinatedItems.ContainsKey(point))
			{
				((List<int>)mCoordinatedItems[point]).RemoveAll(x => x == item.Id);
				return true;
			}
			return false;
		}

		private void AddSpecialItems(Item item)
		{
			switch (item.GetBaseItem().InteractionType)
			{
				case InteractionType.FOOTBALL_GATE:
					//IsTrans = true;
					_room.GetSoccer().RegisterGate(item);


					string[] splittedExtraData = item.ExtraData.Split(':');

					if (string.IsNullOrEmpty(item.ExtraData) || splittedExtraData.Length <= 1)
					{
						item.Gender = "M";
						switch (item.team)
						{
							case TEAM.YELLOW:
								item.Figure = "lg-275-93.hr-115-61.hd-207-14.ch-265-93.sh-305-62";
								break;
							case TEAM.RED:
								item.Figure = "lg-275-96.hr-115-61.hd-180-3.ch-265-96.sh-305-62";
								break;
							case TEAM.GREEN:
								item.Figure = "lg-275-102.hr-115-61.hd-180-3.ch-265-102.sh-305-62";
								break;
							case TEAM.BLUE:
								item.Figure = "lg-275-108.hr-115-61.hd-180-3.ch-265-108.sh-305-62";
								break;
						}
					}
					else
					{
						item.Gender = splittedExtraData[0];
						item.Figure = splittedExtraData[1];
					}
					break;

				case InteractionType.banzaifloor:
					{
						_room.GetBanzai().AddTile(item, item.Id);
						break;
					}

				case InteractionType.banzaipyramid:
					{
						_room.GetGameItemHandler().AddPyramid(item, item.Id);
						break;
					}

				case InteractionType.banzaitele:
					{
						_room.GetGameItemHandler().AddTeleport(item, item.Id);
						item.ExtraData = "";
						break;
					}
				case InteractionType.banzaipuck:
					{
						_room.GetBanzai().AddPuck(item);
						break;
					}

				case InteractionType.FOOTBALL:
					{
						_room.GetSoccer().AddBall(item);
						break;
					}
				case InteractionType.FREEZE_TILE_BLOCK:
					{
						_room.GetFreeze().AddFreezeBlock(item);
						break;
					}
				case InteractionType.FREEZE_TILE:
					{
						_room.GetFreeze().AddFreezeTile(item);
						break;
					}
				case InteractionType.freezeexit:
					{
						_room.GetFreeze().AddExitTile(item);
						break;
					}
			}
		}

		private void RemoveSpecialItem(Item item)
		{
			switch (item.GetBaseItem().InteractionType)
			{
				case InteractionType.FOOTBALL_GATE:
					_room.GetSoccer().UnRegisterGate(item);
					break;
				case InteractionType.banzaifloor:
					_room.GetBanzai().RemoveTile(item.Id);
					break;
				case InteractionType.banzaipuck:
					_room.GetBanzai().RemovePuck(item.Id);
					break;
				case InteractionType.banzaipyramid:
					_room.GetGameItemHandler().RemovePyramid(item.Id);
					break;
				case InteractionType.banzaitele:
					_room.GetGameItemHandler().RemoveTeleport(item.Id);
					break;
				case InteractionType.FOOTBALL:
					_room.GetSoccer().RemoveBall(item.Id);
					break;
				case InteractionType.FREEZE_TILE:
					_room.GetFreeze().RemoveFreezeTile(item.Id);
					break;
				case InteractionType.FREEZE_TILE_BLOCK:
					_room.GetFreeze().RemoveFreezeBlock(item.Id);
					break;
				case InteractionType.freezeexit:
					_room.GetFreeze().RemoveExitTile(item.Id);
					break;
			}
		}

		public bool RemoveFromMap(Item item, bool handleGameItem)
		{
			if (handleGameItem)
				RemoveSpecialItem(item);

			if (_room.GotSoccer())
				_room.GetSoccer().onGateRemove(item);

			bool isRemoved = false;
			foreach (Point coord in item.GetCoords.ToList())
			{
				if (RemoveCoordinatedItem(item, coord))
					isRemoved = true;
			}

			ConcurrentDictionary<Point, List<Item>> items = new ConcurrentDictionary<Point, List<Item>>();
			foreach (Point Tile in item.GetCoords.ToList())
			{
				Point point = new Point(Tile.X, Tile.Y);
				if (mCoordinatedItems.ContainsKey(point))
				{
					List<int> Ids = (List<int>)mCoordinatedItems[point];
					List<Item> __items = GetItemsFromIds(Ids);

					if (!items.ContainsKey(Tile))
						items.TryAdd(Tile, __items);
				}

				SetDefaultValue(Tile.X, Tile.Y);
			}

			foreach (Point Coord in items.Keys.ToList())
			{
				if (!items.ContainsKey(Coord))
					continue;

				List<Item> SubItems = (List<Item>)items[Coord];
				foreach (Item Item in SubItems.ToList())
				{
					ConstructMapForItem(Item, Coord);
				}
			}


			items.Clear();
			items = null;


			return isRemoved;
		}

		public bool RemoveFromMap(Item item)
		{
			return RemoveFromMap(item, true);
		}

		public bool AddItemToMap(Item Item, bool handleGameItem, bool NewItem = true)
		{

			if (handleGameItem)
			{
				AddSpecialItems(Item);

				switch (Item.GetBaseItem().InteractionType)
				{
					case InteractionType.FOOTBALL_GOAL_RED:
					case InteractionType.footballcounterred:
					case InteractionType.banzaiscorered:
					case InteractionType.banzaigatered:
					case InteractionType.freezeredcounter:
					case InteractionType.FREEZE_RED_GATE:
						{
							if (!_room.GetRoomItemHandler().GetFloor.Contains(Item))
								_room.GetGameManager().AddFurnitureToTeam(Item, TEAM.RED);
							break;
						}
					case InteractionType.FOOTBALL_GOAL_GREEN:
					case InteractionType.footballcountergreen:
					case InteractionType.banzaiscoregreen:
					case InteractionType.banzaigategreen:
					case InteractionType.freezegreencounter:
					case InteractionType.FREEZE_GREEN_GATE:
						{
							if (!_room.GetRoomItemHandler().GetFloor.Contains(Item))
								_room.GetGameManager().AddFurnitureToTeam(Item, TEAM.GREEN);
							break;
						}
					case InteractionType.FOOTBALL_GOAL_BLUE:
					case InteractionType.footballcounterblue:
					case InteractionType.banzaiscoreblue:
					case InteractionType.banzaigateblue:
					case InteractionType.freezebluecounter:
					case InteractionType.FREEZE_BLUE_GATE:
						{
							if (!_room.GetRoomItemHandler().GetFloor.Contains(Item))
								_room.GetGameManager().AddFurnitureToTeam(Item, TEAM.BLUE);
							break;
						}
					case InteractionType.FOOTBALL_GOAL_YELLOW:
					case InteractionType.footballcounteryellow:
					case InteractionType.banzaiscoreyellow:
					case InteractionType.banzaigateyellow:
					case InteractionType.freezeyellowcounter:
					case InteractionType.FREEZE_YELLOW_GATE:
						{
							if (!_room.GetRoomItemHandler().GetFloor.Contains(Item))
								_room.GetGameManager().AddFurnitureToTeam(Item, TEAM.YELLOW);
							break;
						}
					case InteractionType.freezeexit:
						{
							_room.GetFreeze().AddExitTile(Item);
							break;
						}
					case InteractionType.ROLLER:
						{
							if (!_room.GetRoomItemHandler().GetRollers().Contains(Item))
								_room.GetRoomItemHandler().TryAddRoller(Item.Id, Item);
							break;
						}
				}
			}

			if (Item.GetBaseItem().Type != 's')
				return true;

			foreach (Point coord in Item.GetCoords.ToList())
			{
				AddCoordinatedItem(Item, new Point(coord.X, coord.Y));
			}

			if (Item.GetX > (Model.MapSizeX - 1))
			{
				Model.AddX();
				GenerateMaps();
				return false;
			}

			if (Item.GetY > (Model.MapSizeY - 1))
			{
				Model.AddY();
				GenerateMaps();
				return false;
			}

			bool Return = true;

			foreach (Point coord in Item.GetCoords)
			{
				if (!ConstructMapForItem(Item, coord))
				{
					Return = false;
				}
				else
				{
					Return = true;
				}
			}



			return Return;
		}


		public bool CanWalk(int X, int Y, bool Override)
		{

			if (Override)
			{
				return true;
			}

			if (_room.GetRoomUserManager().GetUserForSquare(X, Y) != null && _room.RoomBlockingEnabled == 0)
				return false;

			return true;
		}

		public bool AddItemToMap(Item Item, bool NewItem = true)
		{
			return AddItemToMap(Item, true, NewItem);
		}

		public bool ItemCanMove(Item Item, Point MoveTo)
		{
			List<ThreeDCoord> Points = Gamemap.GetAffectedTiles(Item.GetBaseItem().Length, Item.GetBaseItem().Width, MoveTo.X, MoveTo.Y, Item.Rotation).Values.ToList();

			if (Points == null || Points.Count == 0)
				return true;

			foreach (ThreeDCoord Coord in Points)
			{

				if (Coord.X >= Model.MapSizeX || Coord.Y >= Model.MapSizeY)
					return false;

				if (!SquareIsOpen(Coord.X, Coord.Y, false))
					return false;

				continue;
			}

			return true;
		}

		public byte GetFloorStatus(Point coord)
		{
			if (coord.X > mGameMap.GetUpperBound(0) || coord.Y > mGameMap.GetUpperBound(1))
				return 1;

			return mGameMap[coord.X, coord.Y];
		}

		public void SetFloorStatus(int X, int Y, byte Status)
		{
			mGameMap[X, Y] = Status;
		}

		public double GetHeightForSquareFromData(Point coord)
		{
			if (coord.X > mDynamicModel.SqFloorHeight.GetUpperBound(0) ||
				coord.Y > mDynamicModel.SqFloorHeight.GetUpperBound(1))
				return 1;
			return mDynamicModel.SqFloorHeight[coord.X, coord.Y];
		}

		public bool CanRollItemHere(int x, int y)
		{
			if (!ValidTile(x, y))
				return false;

			if (Model.SqState[x, y] == SquareState.BLOCKED)
				return false;

			return true;
		}

		public bool SquareIsOpen(int x, int y, bool pOverride)
		{
			if ((mDynamicModel.MapSizeX - 1) < x || (mDynamicModel.MapSizeY - 1) < y)
				return false;

			return CanWalk(mGameMap[x, y], pOverride);
		}

		public bool GetHighestItemForSquare(Point Square, out Item Item)
		{
			List<Item> Items = GetAllRoomItemForSquare(Square.X, Square.Y);
			Item = null;
			double HighestZ = -1;

			if (Items != null && Items.Count() > 0)
			{
				foreach (Item uItem in Items.ToList())
				{
					if (uItem == null)
						continue;

					if (uItem.TotalHeight > HighestZ)
					{
						HighestZ = uItem.TotalHeight;
						Item = uItem;
						continue;
					}
					else
						continue;
				}
			}
			else
				return false;

			return true;
		}

		public double GetHeightForSquare(Point Coord)
		{
			Item rItem;

			if (GetHighestItemForSquare(Coord, out rItem))
				if (rItem != null)
					return rItem.TotalHeight;

			return 0.0;
		}

		public Point GetChaseMovement(Item Item)
		{
			int Distance = 99;
			Point Coord = new Point(0, 0);
			int iX = Item.GetX;
			int iY = Item.GetY;
			bool X = false;

			foreach (RoomUser User in _room.GetRoomUserManager().GetRoomUsers())
			{
				if (User.X == Item.GetX || Item.GetY == User.Y)
				{
					if (User.X == Item.GetX)
					{
						int Difference = Math.Abs(User.Y - Item.GetY);
						if (Difference < Distance)
						{
							Distance = Difference;
							Coord = User.Coordinate;
							X = false;
						}
						else
							continue;

					}
					else if (User.Y == Item.GetY)
					{
						int Difference = Math.Abs(User.X - Item.GetX);
						if (Difference < Distance)
						{
							Distance = Difference;
							Coord = User.Coordinate;
							X = true;
						}
						else
							continue;
					}
					else
						continue;
				}
			}

			if (Distance > 5)
				return Item.GetSides().OrderBy(x => Guid.NewGuid()).FirstOrDefault();
			if (X && Distance < 99)
			{
				if (iX > Coord.X)
				{
					iX--;
					return new Point(iX, iY);
				}
				else
				{
					iX++;
					return new Point(iX, iY);
				}
			}
			else if (!X && Distance < 99)
			{
				if (iY > Coord.Y)
				{
					iY--;
					return new Point(iX, iY);
				}
				else
				{
					iY++;
					return new Point(iX, iY);
				}
			}
			else
				return Item.Coordinate;
		}

		/*internal bool IsValidMovement(int CoordX, int CoordY)
        {
            if (CoordX < 0 || CoordY < 0 || CoordX >= Model.MapSizeX || CoordY >= Model.MapSizeY)
                return false;

            if (SquareHasUsers(CoordX, CoordY))
                return false;

            if (GetCoordinatedItems(new Point(CoordX, CoordY)).Count > 0 && !SquareIsOpen(CoordX, CoordY, false))
                return false;

            return Model.SqState[CoordX, CoordY] == SquareState.OPEN;
        }*/

		public bool IsValidStep2(RoomUser User, Vector2D From, Vector2D To, bool EndOfPath, bool Override)
		{
			if (User == null)
				return false;

			if (!ValidTile(To.X, To.Y))
				return false;

			if (Override)
				return true;

			/*
             * 0 = blocked
             * 1 = open
             * 2 = last step
             * 3 = door
             * */

			List<Item> Items = _room.GetGameMap().GetAllRoomItemForSquare(To.X, To.Y);
			if (Items.Count > 0)
			{
				bool HasGroupGate = Items.ToList().Where(x => x.GetBaseItem().InteractionType == InteractionType.GUILD_GATE).ToList().Count() > 0;
				if (HasGroupGate)
				{
					Item I = Items.FirstOrDefault(x => x.GetBaseItem().InteractionType == InteractionType.GUILD_GATE);
					if (I != null)
					{
						Group Group = null;
						if (!CloudServer.GetGame().GetGroupManager().TryGetGroup(I.GroupId, out Group))
							return false;

						if (User.GetClient() == null || User.GetClient().GetHabbo() == null)
							return false;

						if (Group.IsMember(User.GetClient().GetHabbo().Id))
						{
							I.InteractingUser = User.GetClient().GetHabbo().Id;
							I.ExtraData = "1";
							I.UpdateState(false, true);

							I.RequestUpdate(4, true);

							return true;
						}
						else
						{
							if (User.Path.Count > 0)
								User.Path.Clear();
							User.PathRecalcNeeded = false;
							return false;
						}
					}
				}
				bool HasHcGate = Items.ToList().Where(x => x.GetBaseItem().InteractionType == InteractionType.HCGATE).ToList().Count() > 0;
				if (HasHcGate)
				{
					Item I = Items.FirstOrDefault(x => x.GetBaseItem().InteractionType == InteractionType.HCGATE);
					if (I != null)
					{
						var IsHc = User.GetClient().GetHabbo().GetClubManager().HasSubscription("habbo_vip");
						if (!IsHc)
						{
							User.GetClient().SendMessage(new AlertNotificationHCMessageComposer(3));
							if (User.Path.Count > 0)
								User.Path.Clear();
							User.PathRecalcNeeded = false;
							return false;
						}



						if (User.GetClient() == null || User.GetClient().GetHabbo() == null)

							return false;

						if (User.GetClient().GetHabbo().GetClubManager().HasSubscription("habbo_vip"))
						{
							I.InteractingUser = User.GetClient().GetHabbo().Id;
							I.ExtraData = "1";
							I.UpdateState(false, true);
							I.RequestUpdate(4, true);

							return true;
						}
						else
						{
							User.GetClient().SendMessage(new AlertNotificationHCMessageComposer(3));
							if (User.Path.Count > 0)
								User.Path.Clear();
							User.PathRecalcNeeded = false;
							return false;
						}
					}
				}

			}

			bool HasVIPGate = Items.ToList().Where(x => x.GetBaseItem().InteractionType == InteractionType.VIPGATE).ToList().Count() > 0;
			if (HasVIPGate)
			{
				Item I = Items.FirstOrDefault(x => x.GetBaseItem().InteractionType == InteractionType.VIPGATE);
				if (I != null)
				{
					var IsVIP = User.GetClient().GetHabbo().GetClubManager().HasSubscription("club_vip");
					if (!IsVIP)
					{
						User.GetClient().SendMessage(new AlertNotificationHCMessageComposer(1));
						if (User.Path.Count > 0)
							User.Path.Clear();
						User.PathRecalcNeeded = false;
						return false;
					}



					if (User.GetClient() == null || User.GetClient().GetHabbo() == null)

						return false;

					if (User.GetClient().GetHabbo().GetClubManager().HasSubscription("club_vip"))
					{
						I.InteractingUser = User.GetClient().GetHabbo().Id;
						I.ExtraData = "1";
						I.UpdateState(false, true);
						I.RequestUpdate(4, true);

						return true;
					}
					else
					{
						User.GetClient().SendMessage(new AlertNotificationHCMessageComposer(1));
						if (User.Path.Count > 0)
							User.Path.Clear();
						User.PathRecalcNeeded = false;
						return false;
					}
				}
			}


			bool Chair = false;
			double HighestZ = -1;
			foreach (Item Item in Items.ToList())
			{
				if (Item == null)
					continue;

				if (Item.GetZ < HighestZ)
				{
					Chair = false;
					continue;
				}

				HighestZ = Item.GetZ;
				if (Item.GetBaseItem().IsSeat)
					Chair = true;
			}

			if ((mGameMap[To.X, To.Y] == 3 && !EndOfPath && !Chair) || (mGameMap[To.X, To.Y] == 0) || (mGameMap[To.X, To.Y] == 2 && !EndOfPath))
			{
				if (User.Path.Count > 0)
					User.Path.Clear();
				User.PathRecalcNeeded = true;
			}

			double HeightDiff = SqAbsoluteHeight(To.X, To.Y) - SqAbsoluteHeight(From.X, From.Y);
			if (HeightDiff > 1.5 && !User.RidingHorse)
				return false;

			//Check this last, because ya.
			RoomUser Userx = _room.GetRoomUserManager().GetUserForSquare(To.X, To.Y);
			if (Userx != null)
			{
				if (!Userx.IsWalking && EndOfPath)
					return false;
			}
			return true;
		}



		public bool IsValidStep(Vector2D From, Vector2D To, bool EndOfPath, bool Override, bool DiagMovement, bool Roller = false)
		{
			if (!ValidTile(To.X, To.Y))
				return false;

			if (Override)
				return true;

            /*
             * 0 = blocked
             * 1 = open
             * 2 = last step
             * 3 = door
             * */

            if (DiagMovement)
            {
                int XValue = To.X - From.X;
                int YValue = To.Y - From.Y;

                if (XValue == -1 && YValue == -1) // arriba izquierda
                {
                    if (mGameMap[To.X + 1, To.Y] != 1 && mGameMap[To.X, To.Y + 1] != 1)
                        return false;
                }
                else if (XValue == 1 && YValue == -1) // arriba derecha
                {
                    if (mGameMap[To.X - 1, To.Y] != 1 && mGameMap[To.X, To.Y + 1] != 1)
                        return false;
                }
                else if (XValue == 1 && YValue == 1) // abajo derecha
                {
                    if (mGameMap[To.X - 1, To.Y] != 1 && mGameMap[To.X, To.Y - 1] != 1)
                        return false;
                }
                else if (XValue == -1 && YValue == 1) // abajo izquierda
                {
                    if (mGameMap[To.X + 1, To.Y] != 1 && mGameMap[To.X, To.Y - 1] != 1)
                        return false;
                }
            }


            if (_room.RoomBlockingEnabled == 0 && SquareHasUsers(To.X, To.Y))
				return false;

			List<Item> Items = _room.GetGameMap().GetAllRoomItemForSquare(To.X, To.Y);
			if (Items.Count > 0)
			{
				bool HasGroupGate = Items.ToList().Where(x => x != null && x.GetBaseItem().InteractionType == InteractionType.GUILD_GATE).Count() > 0;
				if (HasGroupGate)
					return true;

				bool HasHcGate = Items.ToList().Where(x => x != null && x.GetBaseItem().InteractionType == InteractionType.HCGATE).Count() > 0;
				if (HasHcGate)
					return true;

				bool HasVIPGate = Items.ToList().Where(x => x != null && x.GetBaseItem().InteractionType == InteractionType.VIPGATE).Count() > 0;
				if (HasVIPGate)
					return true;
			}

			if ((mGameMap[To.X, To.Y] == 3 && !EndOfPath) || mGameMap[To.X, To.Y] == 0 || (mGameMap[To.X, To.Y] == 2 && !EndOfPath))
				return false;

			if (!Roller)
			{
				double HeightDiff = SqAbsoluteHeight(To.X, To.Y) - SqAbsoluteHeight(From.X, From.Y);
				if (HeightDiff > 1.5)
					return false;
			}

			return true;
		}

		public static bool CanWalk(byte pState, bool pOverride)
		{
			if (!pOverride)
			{
				if (pState == 3)
					return true;
				if (pState == 1)
					return true;

				return false;
			}
			return true;
		}

		public bool ItemCanBePlacedHere(int x, int y)
		{
			if (mDynamicModel.MapSizeX - 1 < x || mDynamicModel.MapSizeY - 1 < y ||
				(x == mDynamicModel.DoorX && y == mDynamicModel.DoorY))
				return false;

			return mGameMap[x, y] == 1;
		}

		public double SqAbsoluteHeight(int X, int Y)
		{
			Point Points = new Point(X, Y);

			List<int> Ids;

			if (mCoordinatedItems.TryGetValue(Points, out Ids))
			{
				List<Item> Items = GetItemsFromIds(Ids);

				return SqAbsoluteHeight(X, Y, Items);
			}
			else
				return mDynamicModel.SqFloorHeight[X, Y];

			#region Old
			/*
            if (mCoordinatedItems.ContainsKey(Points))
            {
                List<Item> Items = new List<Item>();
                foreach (Item Item in mCoordinatedItems[Points].ToArray())
                {
                    if (!Items.Contains(Item))
                        Items.Add(Item);
                }
                return SqAbsoluteHeight(X, Y, Items);
            }*/
			#endregion
		}

		public double SqAbsoluteHeight(int X, int Y, List<Item> ItemsOnSquare)
		{
			try
			{
				bool deduct = false;
				double HighestStack = 0;
				double deductable = 0.0;

				if (ItemsOnSquare != null && ItemsOnSquare.Count > 0)
				{
					foreach (Item Item in ItemsOnSquare.ToList())
					{
						if (Item == null)
							continue;

						if (Item.TotalHeight > HighestStack)
						{
							if (Item.GetBaseItem().IsSeat || Item.GetBaseItem().InteractionType == InteractionType.BED || Item.GetBaseItem().InteractionType == InteractionType.TENT_SMALL)
							{
								deduct = true;
								deductable = Item.GetBaseItem().Height;
							}
							else
								deduct = false;
							HighestStack = Item.TotalHeight;
						}
					}
				}

				double floorHeight = Model.SqFloorHeight[X, Y];
				double stackHeight = HighestStack - Model.SqFloorHeight[X, Y];

				if (deduct)
					stackHeight -= deductable;

				if (stackHeight < 0)
					stackHeight = 0;

				return (floorHeight + stackHeight);
			}
			catch (Exception e)
			{
				ExceptionLogger.LogException(e);
				return 0;
			}
		}

		public bool ValidTile(int X, int Y)
		{
			if (X < 0 || Y < 0 || X >= Model.MapSizeX || Y >= Model.MapSizeY)
			{
				return false;
			}

			return true;
		}

		public static Dictionary<int, ThreeDCoord> GetAffectedTiles(int Length, int Width, int PosX, int PosY, int Rotation)
		{
			int x = 0;

			var PointList = new Dictionary<int, ThreeDCoord>();

			if (Length > 1)
			{
				if (Rotation == 0 || Rotation == 4)
				{
					for (int i = 1; i < Length; i++)
					{
						PointList.Add(x++, new ThreeDCoord(PosX, PosY + i, i));

						for (int j = 1; j < Width; j++)
						{
							PointList.Add(x++, new ThreeDCoord(PosX + j, PosY + i, (i < j) ? j : i));
						}
					}
				}
				else if (Rotation == 2 || Rotation == 6)
				{
					for (int i = 1; i < Length; i++)
					{
						PointList.Add(x++, new ThreeDCoord(PosX + i, PosY, i));

						for (int j = 1; j < Width; j++)
						{
							PointList.Add(x++, new ThreeDCoord(PosX + i, PosY + j, (i < j) ? j : i));
						}
					}
				}
			}

			if (Width > 1)
			{
				if (Rotation == 0 || Rotation == 4)
				{
					for (int i = 1; i < Width; i++)
					{
						PointList.Add(x++, new ThreeDCoord(PosX + i, PosY, i));

						for (int j = 1; j < Length; j++)
						{
							PointList.Add(x++, new ThreeDCoord(PosX + i, PosY + j, (i < j) ? j : i));
						}
					}
				}
				else if (Rotation == 2 || Rotation == 6)
				{
					for (int i = 1; i < Width; i++)
					{
						PointList.Add(x++, new ThreeDCoord(PosX, PosY + i, i));

						for (int j = 1; j < Length; j++)
						{
							PointList.Add(x++, new ThreeDCoord(PosX + j, PosY + i, (i < j) ? j : i));
						}
					}
				}
			}

			return PointList;
		}

		public List<Item> GetItemsFromIds(List<int> Input)
		{
			if (Input == null || Input.Count == 0)
				return new List<Item>();

			List<Item> Items = new List<Item>();

			lock (Input)
			{
				foreach (int Id in Input.ToList())
				{
					Item Itm = _room.GetRoomItemHandler().GetItem(Id);
					if (Itm != null && !Items.Contains(Itm))
						Items.Add(Itm);
				}
			}

			return Items.ToList();
		}

		public List<Item> GetRoomItemForSquare(int pX, int pY, double minZ)
		{
			var itemsToReturn = new List<Item>();


			var coord = new Point(pX, pY);
			if (mCoordinatedItems.ContainsKey(coord))
			{
				var itemsFromSquare = GetItemsFromIds((List<int>)mCoordinatedItems[coord]);

				foreach (Item item in itemsFromSquare)
					if (item.GetZ > minZ)
						if (item.GetX == pX && item.GetY == pY)
							itemsToReturn.Add(item);
			}

			return itemsToReturn;
		}

		public List<Item> GetRoomItemForSquare(int pX, int pY)
		{
			var coord = new Point(pX, pY);
			//List<RoomItem> itemsFromSquare = new List<RoomItem>();
			var itemsToReturn = new List<Item>();

			if (mCoordinatedItems.ContainsKey(coord))
			{
				var itemsFromSquare = GetItemsFromIds((List<int>)mCoordinatedItems[coord]);

				foreach (Item item in itemsFromSquare)
				{
					if (item.Coordinate.X == coord.X && item.Coordinate.Y == coord.Y)
						itemsToReturn.Add(item);
				}
			}

			return itemsToReturn;
		}

		public List<Item> GetAllRoomItemForSquare(int pX, int pY)
		{
			Point Coord = new Point(pX, pY);

			List<Item> Items = new List<Item>();
			List<int> Ids;

			// CHANGED THIS ~  IF FAILED CHANGE BACK

			if (mCoordinatedItems.TryGetValue(Coord, out Ids))
				Items = GetItemsFromIds(Ids);
			else
				Items = new List<Item>();

			return Items;
		}

		public bool SquareHasUsers(int X, int Y)
		{
			return MapGotUser(new Point(X, Y));
		}

		public bool SquareHasUsers(int X, int Y, RoomUser MyUser)
		{
			List<RoomUser> users = GetRoomUsers(new Point(X, Y));
			if (users.Count > 0)
			{
				return !users.Exists(u => u.UserId == MyUser.UserId);
			}

			return false;
		}

		public static bool TilesTouching(int X1, int Y1, int X2, int Y2)
		{
			if (!(Math.Abs(X1 - X2) > 1 || Math.Abs(Y1 - Y2) > 1)) return true;
			if (X1 == X2 && Y1 == Y2) return true;
			return false;
		}

		public static int TileDistance(int X1, int Y1, int X2, int Y2)
		{
			return Math.Abs(X1 - X2) + Math.Abs(Y1 - Y2);
		}

		public DynamicRoomModel Model
		{
			get { return mDynamicModel; }
		}

		public RoomModel StaticModel
		{
			get { return mStaticModel; }
		}

		public byte[,] EffectMap
		{
			get { return mUserItemEffect; }
		}

		public byte[,] GameMap
		{
			get { return mGameMap; }
		}

		public void Dispose()
		{
			userMap.Clear();
			mDynamicModel.Destroy();
			mCoordinatedItems.Clear();

			Array.Clear(mGameMap, 0, mGameMap.Length);
			Array.Clear(mUserItemEffect, 0, mUserItemEffect.Length);
			Array.Clear(mItemHeightMap, 0, mItemHeightMap.Length);

			userMap = null;
			mGameMap = null;
			mUserItemEffect = null;
			mItemHeightMap = null;
			mCoordinatedItems = null;

			mDynamicModel = null;
			this._room = null;
			mStaticModel = null;
		}
	}
}