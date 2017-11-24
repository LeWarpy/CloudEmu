﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Cloud.Core;
using Cloud.HabboHotel.GameClients;
using System.Collections.Concurrent;
using Cloud.Database.Interfaces;
using log4net;

namespace Cloud.HabboHotel.Rooms
{
    public class RoomManager
    {
        private static readonly ILog log = LogManager.GetLogger("Cloud.HabboHotel.Rooms.RoomManager");
        private Dictionary<string, RoomModel> _roomModels;
        private ConcurrentDictionary<int, Room> _rooms;
        private ConcurrentDictionary<int, RoomData> _loadedRoomData;
        private DateTime _cycleLastExecution;
        private DateTime _purgeLastExecution;

        public RoomManager()
        {
            this._roomModels = new Dictionary<string, RoomModel>();
            this._rooms = new ConcurrentDictionary<int, Room>();
            this._loadedRoomData = new ConcurrentDictionary<int, RoomData>();
            this.LoadModels();
            this._purgeLastExecution = DateTime.Now.AddHours(3);
            log.Info("» Salas -> CARGADO");
        }

        public void OnCycle()
        {
            try
            {
                TimeSpan sinceLastTime = DateTime.Now - _cycleLastExecution;
                if (sinceLastTime.TotalMilliseconds >= 500)
                {
                    _cycleLastExecution = DateTime.Now;
                    foreach (Room room in this._rooms.Values.ToList())
                    {
                        if (room.isCrashed)
                            continue;

                        if (room.ProcessTask == null || room.ProcessTask.IsCompleted)
                        {
                            room.ProcessTask = new Task(room.ProcessRoom);
                            room.ProcessTask.Start();
                            room.IsLagging = 0;
                        }
                        else
                        {
                            room.IsLagging++;
                            if (room.IsLagging >= 30)
                            {
                                room.isCrashed = true;
                                UnloadRoom(room.Id);
                            }
                        }
                    }
                }
                CloudServer.GetGame().RoomManagerCycleEnded = true;
            }
            catch (Exception e)
            {
                ExceptionLogger.LogException(e);
            }
        }

        public int LoadedRoomDataCount
        {
            get { return this._loadedRoomData.Count; }
        }

        public int Count
        {
            get { return this._rooms.Count; }
        }

        public void LoadModels()
        {
            if (this._roomModels.Count > 0)
                _roomModels.Clear();

            using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT id,door_x,door_y,door_z,door_dir,heightmap,public_items,club_only,poolmap,`wall_height` FROM `room_models` WHERE `custom` = '0'");
                DataTable Data = dbClient.getTable();

                if (Data == null)
                    return;

                foreach (DataRow Row in Data.Rows)
                {
                    string Modelname = Convert.ToString(Row["id"]);
                    string staticFurniture = Convert.ToString(Row["public_items"]);

                    _roomModels.Add(Modelname, new RoomModel(Convert.ToInt32(Row["door_x"]), Convert.ToInt32(Row["door_y"]), (Double)Row["door_z"], Convert.ToInt32(Row["door_dir"]),
                        Convert.ToString(Row["heightmap"]), Convert.ToString(Row["public_items"]), CloudServer.EnumToBool(Row["club_only"].ToString()), Convert.ToString(Row["poolmap"]), Convert.ToInt32(Row["wall_height"])));
                }
            }
        }

        public void LoadModel(string Id)
        {
            DataRow Row = null;
            using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT id,door_x,door_y,door_z,door_dir,heightmap,public_items,club_only,poolmap,`wall_height` FROM `room_models` WHERE `custom` = '1' AND `id` = '" + Id + "' LIMIT 1");
                Row = dbClient.getRow();

                if (Row == null)
                    return;

                string Modelname = Convert.ToString(Row["id"]);
                if (!this._roomModels.ContainsKey(Id))
                {
                    this._roomModels.Add(Modelname, new RoomModel(Convert.ToInt32(Row["door_x"]), Convert.ToInt32(Row["door_y"]), Convert.ToDouble(Row["door_z"]), Convert.ToInt32(Row["door_dir"]),
                      Convert.ToString(Row["heightmap"]), Convert.ToString(Row["public_items"]), CloudServer.EnumToBool(Row["club_only"].ToString()), Convert.ToString(Row["poolmap"]), Convert.ToInt32(Row["wall_height"])));
                }
            }
        }

        public void ReloadModel(string Id)
        {
            if (!this._roomModels.ContainsKey(Id))
            {
                this.LoadModel(Id);
                return;
            }

            this._roomModels.Remove(Id);
            this.LoadModel(Id);
        }

        public bool TryGetModel(string Id, out RoomModel Model)
        {
            return this._roomModels.TryGetValue(Id, out Model);
        }

        public void UnloadRoom(int roomId)
        {
            Room room = null;
            if (this._rooms.TryRemove(roomId, out room))
            {
                room.Dispose();
            }
        }

        public void UnloadRoom(Room Room, bool RemoveData = false)
        {
            if (Room == null)
                return;

            Room room = null;
            if (this._rooms.TryRemove(Room.RoomId, out room))
            {
                Room.Dispose();

                if (RemoveData)
                {
                    RoomData Data = null;
                    this._loadedRoomData.TryRemove(Room.Id, out Data);
                }
            }
            //Logging.WriteLine("[RoomMgr] Unloaded room: \"" + Room.Name + "\" (ID: " + Room.RoomId + ")");
        }

        public List<RoomData> SearchGroupRooms(string Query)
        {
            IEnumerable<RoomData> InstanceMatches =
                (from RoomInstance in this._loadedRoomData
                 where RoomInstance.Value.UsersNow >= 0 &&
                 RoomInstance.Value.Access != RoomAccess.INVISIBLE &&
                 RoomInstance.Value.Group != null &&
                 (RoomInstance.Value.OwnerName.StartsWith(Query) ||
                 RoomInstance.Value.Tags.Contains(Query) ||
                 RoomInstance.Value.Name.Contains(Query))
                 orderby RoomInstance.Value.UsersNow descending
                 select RoomInstance.Value).Take(50);
            return InstanceMatches.ToList();
        }

        public List<RoomData> SearchTaggedRooms(string Query)
        {
            IEnumerable<RoomData> InstanceMatches =
                (from RoomInstance in this._loadedRoomData
                 where RoomInstance.Value.UsersNow >= 0 &&
                 RoomInstance.Value.Access != RoomAccess.INVISIBLE &&
                 (RoomInstance.Value.Tags.Contains(Query))
                 orderby RoomInstance.Value.UsersNow descending
                 select RoomInstance.Value).Take(50);
            return InstanceMatches.ToList();
        }

        public List<RoomData> GetPopularRooms(int category, int Amount = 50)
        {
            IEnumerable<RoomData> rooms =
                (from RoomInstance in this._loadedRoomData
                 where RoomInstance.Value.UsersNow > 0 &&
                 (category == -1 || RoomInstance.Value.Category == category) &&
                 RoomInstance.Value.Access != RoomAccess.INVISIBLE
                 orderby RoomInstance.Value.Score descending
                 orderby RoomInstance.Value.UsersNow descending
                 select RoomInstance.Value).Take(Amount);
            return rooms.ToList();
        }

        public List<RoomData> GetRecommendedRooms(int Amount = 50, int CurrentRoomId = 0)
        {
            IEnumerable<RoomData> Rooms =
                (from RoomInstance in this._loadedRoomData
                 where RoomInstance.Value.UsersNow >= 0 &&
                 RoomInstance.Value.Score >= 0 &&
                 RoomInstance.Value.Access != RoomAccess.INVISIBLE &&
                 RoomInstance.Value.Id != CurrentRoomId
                 orderby RoomInstance.Value.Score descending
                 orderby RoomInstance.Value.UsersNow descending
                 select RoomInstance.Value).Take(Amount);
            return Rooms.ToList();
        }

        public List<RoomData> GetPopularRatedRooms(int Amount = 50)
        {
            IEnumerable<RoomData> rooms =
                (from RoomInstance in this._loadedRoomData
                 where RoomInstance.Value.Access != RoomAccess.INVISIBLE
                 orderby RoomInstance.Value.Score descending
                 select RoomInstance.Value).Take(Amount);
            return rooms.ToList();
        }

        public List<RoomData> GetRoomsByCategory(int Category, int Amount = 50)
        {
            IEnumerable<RoomData> rooms =
                (from RoomInstance in this._loadedRoomData
                 where RoomInstance.Value.Category == Category &&
                 RoomInstance.Value.UsersNow > 0 &&
                 RoomInstance.Value.Access != RoomAccess.INVISIBLE
                 orderby RoomInstance.Value.UsersNow descending
                 select RoomInstance.Value).Take(Amount);
            return rooms.ToList();
        }

        public List<RoomData> GetOnGoingRoomPromotions(int Mode, int Amount = 50)
        {
            IEnumerable<RoomData> Rooms = null;

            if (Mode == 17)
            {
                Rooms =
                    (from RoomInstance in this._loadedRoomData
                     where (RoomInstance.Value.HasActivePromotion) &&
                     RoomInstance.Value.Access != RoomAccess.INVISIBLE
                     orderby RoomInstance.Value.Promotion.TimestampStarted descending
                     select RoomInstance.Value).Take(Amount);
            }
            else
            {
                Rooms =
                    (from RoomInstance in this._loadedRoomData
                     where (RoomInstance.Value.HasActivePromotion) &&
                     RoomInstance.Value.Access != RoomAccess.INVISIBLE
                     orderby RoomInstance.Value.UsersNow descending
                     select RoomInstance.Value).Take(Amount);
            }

            return Rooms.ToList();
        }


        public List<RoomData> GetPromotedRooms(int CategoryId, int Amount = 50)
        {
            IEnumerable<RoomData> Rooms = null;

            Rooms =
                (from RoomInstance in this._loadedRoomData
                 where (RoomInstance.Value.HasActivePromotion) &&
                 RoomInstance.Value.Promotion.CategoryId == CategoryId &&
                 RoomInstance.Value.Access != RoomAccess.INVISIBLE
                 orderby RoomInstance.Value.Promotion.TimestampStarted descending
                 select RoomInstance.Value).Take(Amount);

            return Rooms.ToList();
        }

        public List<KeyValuePair<string, int>> GetPopularRoomTags()
        {
            IEnumerable<List<string>> Tags =
                (from RoomInstance in this._loadedRoomData
                 where RoomInstance.Value.UsersNow >= 0 &&
                 RoomInstance.Value.Access != RoomAccess.INVISIBLE
                 orderby RoomInstance.Value.UsersNow descending
                 orderby RoomInstance.Value.Score descending
                 select RoomInstance.Value.Tags).Take(50);

            Dictionary<string, int> TagValues = new Dictionary<string, int>();

            foreach (List<string> TagList in Tags)
            {
                foreach (string Tag in TagList)
                {
                    if (!TagValues.ContainsKey(Tag))
                    {
                        TagValues.Add(Tag, 1);
                    }
                    else
                    {
                        TagValues[Tag]++;
                    }
                }
            }

            List<KeyValuePair<string, int>> SortedTags = new List<KeyValuePair<string, int>>(TagValues);
            SortedTags.Sort((FirstPair, NextPair) =>
            {
                return FirstPair.Value.CompareTo(NextPair.Value);
            });

            SortedTags.Reverse();
            return SortedTags;
        }

        public List<RoomData> GetGroupRooms(int Amount = 50)
        {
            IEnumerable<RoomData> rooms =
                (from RoomInstance in this._loadedRoomData
                 where RoomInstance.Value.Group != null &&
                 RoomInstance.Value.Access != RoomAccess.INVISIBLE
                 orderby RoomInstance.Value.Score descending
                 select RoomInstance.Value).Take(Amount);
            return rooms.ToList();
        }

        public Room TryGetRandomLoadedRoom()
        {
            IEnumerable<Room> room =
                (from RoomInstance in this._rooms
                 where (RoomInstance.Value.RoomData.UsersNow > 0 &&
                 RoomInstance.Value.RoomData.Access == RoomAccess.OPEN &&
                 RoomInstance.Value.RoomData.UsersNow < RoomInstance.Value.RoomData.UsersMax)
                 orderby RoomInstance.Value.RoomData.UsersNow descending
                 select RoomInstance.Value).Take(1);

            if (room.Count() > 0)
                return room.First();
            else
                return null;
        }

        public RoomModel GetModel(string Model)
        {
            if (_roomModels.ContainsKey(Model))
                return (RoomModel)_roomModels[Model];

            return null;
        }

        public RoomData GenerateRoomData(int RoomId)
        {
            if (_loadedRoomData.ContainsKey(RoomId))
                return (RoomData)_loadedRoomData[RoomId];

            RoomData Data = new RoomData();

            Room Room;

            if (TryGetRoom(RoomId, out Room))
                return Room.RoomData;

            DataRow Row = null;
            using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM rooms WHERE id = " + RoomId + " LIMIT 1");
                Row = dbClient.getRow();
            }

            if (Row == null)
                return null;

            Data.Fill(Row);

            if (!_loadedRoomData.ContainsKey(RoomId))
                _loadedRoomData.TryAdd(RoomId, Data);

            return Data;
        }

        public bool RoomExist(int RoomId)
        {

            DataRow Row = null;
            using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT id FROM rooms WHERE id = " + RoomId + " LIMIT 1");
                Row = dbClient.getRow();
            }

            if (Row == null)
                return false;



            return true;
        }

        public RoomData FetchRoomData(int RoomId, DataRow dRow)
        {
            if (_loadedRoomData.ContainsKey(RoomId))
                return (RoomData)_loadedRoomData[RoomId];
            else
            {
                RoomData data = new RoomData();

                data.Fill(dRow);

                if (!_loadedRoomData.ContainsKey(RoomId))
                    _loadedRoomData.TryAdd(RoomId, data);
                return data;
            }
        }

        public Room LoadRoom(int Id)
        {
            Room Room = null;

            if (TryGetRoom(Id, out Room))
                return Room;

            RoomData Data = GenerateRoomData(Id);
            if (Data == null)
                return null;

            Room = new Room(Data);

            if (!_rooms.ContainsKey(Room.RoomId))
                _rooms.TryAdd(Room.RoomId, Room);

            return Room;
        }

        public bool TryGetRoom(int RoomId, out Room Room)
        {
            return this._rooms.TryGetValue(RoomId, out Room);
        }

        public RoomData CreateRoom(GameClient Session, string Name, string Description, string Model, int Category, int MaxVisitors, int TradeSettings,
            string wallpaper = "0.0", string floor = "0.0", string landscape = "0.0", int wallthick = 0, int floorthick = 0)
        {
            if (!_roomModels.ContainsKey(Model))
            {
                Session.SendNotification(CloudServer.GetGame().GetLanguageManager().TryGetValue("room.creation.model.not_found"));
                return null;
            }

            if (Name.Length < 3)
            {
                Session.SendNotification(CloudServer.GetGame().GetLanguageManager().TryGetValue("room.creation.name.too_short"));
                return null;
            }

            int RoomId = 0;

            using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("INSERT INTO `rooms` (`roomtype`,`caption`,`description`,`owner`,`model_name`,`category`,`users_max`,`trade_settings`,`wallpaper`,`floor`,`landscape`,`floorthick`,`wallthick`) VALUES ('private',@caption,@description,@UserId,@model,@category,@usersmax,@tradesettings,@wallpaper,@floor,@landscape,@floorthick,@wallthick)");
                dbClient.AddParameter("caption", Name);
                dbClient.AddParameter("description", Description);
                dbClient.AddParameter("UserId", Session.GetHabbo().Id);
                dbClient.AddParameter("model", Model);
                dbClient.AddParameter("category", Category);
                dbClient.AddParameter("usersmax", MaxVisitors);
                dbClient.AddParameter("tradesettings", TradeSettings);
                dbClient.AddParameter("wallpaper", wallpaper);
                dbClient.AddParameter("floor", floor);
                dbClient.AddParameter("landscape", landscape);
                dbClient.AddParameter("floorthick", floorthick);
                dbClient.AddParameter("wallthick", wallthick);

                RoomId = Convert.ToInt32(dbClient.InsertQuery());
            }

            RoomData newRoomData = GenerateRoomData(RoomId);
            Session.GetHabbo().UsersRooms.Add(newRoomData);
            return newRoomData;
        }

        public ICollection<Room> GetRooms()
        {
            return this._rooms.Values;
        }

        public void Dispose()
        {
            int length = _rooms.Count;
            int i = 0;
            foreach (Room Room in this._rooms.Values.ToList())
            {
                if (Room == null)
                    continue;

                CloudServer.GetGame().GetRoomManager().UnloadRoom(Room.Id);
                Console.Clear();
                log.Info("<<- SERVER SHUTDOWN ->> ROOM ITEM SAVE: " + String.Format("{0:0.##}", ((double)i / length) * 100) + "%");
                i++;
            }
            log.Info("Quartos salvos com sucesso!");
        }
    }
}