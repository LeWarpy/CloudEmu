using System;
using System.Linq;
using System.Collections.Generic;
using Cloud.Communication.Packets.Outgoing.Rooms.Notifications;
using Cloud.Core;
using Cloud.HabboHotel.Catalog;
using Cloud.HabboHotel.GameClients;
using Cloud.HabboHotel.Items;
using Cloud.HabboHotel.Users.Effects;
using Cloud.HabboHotel.Items.Utilities;
using Cloud.HabboHotel.Users.Inventory.Bots;
using Cloud.HabboHotel.Rooms.AI;
using Cloud.Communication.Packets.Outgoing.Catalog;
using Cloud.Communication.Packets.Outgoing.Inventory.Bots;
using Cloud.Communication.Packets.Outgoing.Inventory.Pets;
using Cloud.Communication.Packets.Outgoing.Inventory.Purse;
using Cloud.Communication.Packets.Outgoing.Inventory.Furni;
using Cloud.Communication.Packets.Outgoing.Inventory.AvatarEffects;
using Cloud.Database.Interfaces;
using Cloud.HabboHotel.Groups;
using Cloud.Communication.Packets.Outgoing.Navigator;
using Cloud.Utilities;
using Cloud.HabboHotel.Groups.Forums;
using Cloud.Communication.Packets.Outgoing.Moderation;
using Cloud.Communication.Packets.Outgoing.Users;

namespace Cloud.Communication.Packets.Incoming.Catalog
{
    public class PurchaseFromCatalogEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            ICollection<Item> FloorItems = Session.GetHabbo().GetInventoryComponent().GetFloorItems();
            ICollection<Item> WallItems = Session.GetHabbo().GetInventoryComponent().GetWallItems();

            if (CloudServer.GetGame().GetSettingsManager().TryGetValue("catalog.enabled") != "1")
            {
                Session.SendNotification("Os gerentes do Hotel desabilitaram o catalogo!");
                return;
            }

            int PageId = Packet.PopInt();
            int ItemId = Packet.PopInt();
            string ExtraData = Packet.PopString();
            int Amount = Packet.PopInt();


			if (!CloudServer.GetGame().GetCatalog().TryGetPage(PageId, out CatalogPage Page))
				return;

			if (!Page.Enabled || !Page.Visible || Page.MinimumRank  >  Session.GetHabbo().Rank || (Page.MinimumVIP > Session.GetHabbo().VIPRank && Session.GetHabbo().Rank == 1))
                return;

			if (!Page.Items.TryGetValue(ItemId, out CatalogItem Item))
			{
				if (Page.ItemOffers.ContainsKey(ItemId))
				{
					Item = Page.ItemOffers[ItemId];
					if (Item == null)
						return;
				}
				else
					return;
			}

			ItemData baseItem = Item.GetBaseItem(Item.ItemId);
            if (baseItem != null)
            {
                if (baseItem.InteractionType == InteractionType.club_1_month || baseItem.InteractionType == InteractionType.club_3_month || baseItem.InteractionType == InteractionType.club_6_month)
                {
                    int Months = 0;

                    switch (baseItem.InteractionType)
                    {
                        case InteractionType.club_1_month:
                            Months = 1;
                            break;

                        case InteractionType.club_3_month:
                            Months = 3;
                            break;

                        case InteractionType.club_6_month:
                            Months = 6;
                            break;
                    }

                    int num = num = 31 * Months;

                    if (Item.CostCredits > Session.GetHabbo().Credits || Item.CostPixels > Session.GetHabbo().Duckets || Item.CostDiamonds > Session.GetHabbo().Diamonds || Item.CostGotw > Session.GetHabbo().GOTWPoints)
                        return;

                    if (Item.CostCredits > 0)
                    {
                        Session.GetHabbo().Credits -= Item.CostCredits;
                        Session.SendMessage(new CreditBalanceComposer(Session.GetHabbo().Credits));
                    }

                    if (Item.CostPixels > 0)
                    {
                        Session.GetHabbo().Duckets -= Item.CostPixels;
                        Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().Duckets, Session.GetHabbo().Duckets));//Love you, Tom.
                    }

                    if (Item.CostDiamonds > 0)
                    {
                        Session.GetHabbo().Diamonds -= Item.CostDiamonds;
                        Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().Diamonds, 0, 5));
                    }

                    if (Item.CostGotw > 0)
                    {
                        Session.GetHabbo().GOTWPoints -= Item.CostGotw;
                        Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().GOTWPoints, 0, 103));
                    }

                    Session.GetHabbo().GetClubManager().AddOrExtendSubscription("habbo_vip", num * 24 * 3600, Session);
                    Session.GetHabbo().GetBadgeComponent().GiveBadge("HC1", true, Session);

                    Session.SendMessage(new ScrSendUserInfoComposer(Session.GetHabbo()));
                    Session.SendMessage(new PurchaseOKComposer(Item, Item.Data, Item.Items));
                    Session.SendMessage(new FurniListUpdateComposer());
                    return;
                }
            }

            if (baseItem.InteractionType == InteractionType.namecolor)
            {
                if (Item.CostCredits > Session.GetHabbo().Credits || Item.CostPixels > Session.GetHabbo().Duckets || Item.CostDiamonds > Session.GetHabbo().Diamonds || Item.CostGotw > Session.GetHabbo().GOTWPoints)
                    return;

                if (Item.CostCredits > 0)
                {
                    Session.GetHabbo().Credits -= Item.CostCredits;
                    Session.SendMessage(new CreditBalanceComposer(Session.GetHabbo().Credits));
                }

                if (Item.CostPixels > 0)
                {
                    Session.GetHabbo().Duckets -= Item.CostPixels;
                    Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().Duckets, Session.GetHabbo().Duckets));//Love you, Tom.
                }

                if (Item.CostDiamonds > 0)
                {
                    Session.GetHabbo().Diamonds -= Item.CostDiamonds;
                    Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().Diamonds, 0, 5));
                }

                if (Item.CostGotw > 0)
                {
                    Session.GetHabbo().GOTWPoints -= Item.CostGotw;
                    Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().GOTWPoints, 0, 103));
                }

                using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.runFastQuery("UPDATE `users` SET `name_color` = '#" + Item.Name + "' WHERE `id` = '" + Session.GetHabbo().Id + "' LIMIT 1");
                }

                Session.GetHabbo().chatHTMLColour = "#" + Item.Name;
                Session.SendMessage(new ScrSendUserInfoComposer(Session.GetHabbo()));
                Session.SendMessage(new PurchaseOKComposer(Item, Item.Data, Item.Items));
                Session.SendMessage(new FurniListUpdateComposer());
                return;
            }

            if (baseItem.InteractionType == InteractionType.prefixname)
            { 
                if (ExtraData.Length > 6 || ExtraData.Length == 0 || ExtraData.Length < 0)
                {
                    Session.SendMessage(new PurchaseOKComposer(Item, Item.Data, Item.Items));
                    Session.SendWhisper("Você deve digitar um código de 1 a 6 caracteres para adquirir.", 34);
                    return;
                }

                if (Session.GetHabbo().Rank < 3)
                {
                    if (ExtraData.ToUpper().Contains("ADM") || ExtraData.ToUpper().Contains("ADMIN") || ExtraData.ToUpper().Contains("GER") || ExtraData.ToUpper().Contains("DONO") || ExtraData.ToUpper().Contains("RANK") || ExtraData.ToUpper().Contains("MNG") || ExtraData.ToUpper().Contains("MOD") || ExtraData.ToUpper().Contains("STAFF") || ExtraData.ToUpper().Contains("ALFA") || ExtraData.ToUpper().Contains("ALPHA") || ExtraData.ToUpper().Contains("HELPER") || ExtraData.ToUpper().Contains("GM") || ExtraData.ToUpper().Contains("CEO") || ExtraData.ToUpper().Contains("ROOKIE") || ExtraData.ToUpper().Contains("M0D") || ExtraData.ToUpper().Contains("DEV") || ExtraData.ToUpper().Contains("OWNR") || ExtraData.ToUpper().Contains("FUNDADOR"))
                    {
                        Session.SendMessage(new PurchaseOKComposer(Item, Item.Data, Item.Items));
                        Session.SendWhisper("O que você está tentando? Não coloque um prefixo administrativa ou você será punido", 34);
                        return;
                    }
                }

                if (ExtraData == "off" || ExtraData == "")
                {
                    Session.GetHabbo()._NamePrefix = "";
                    Session.SendNotification("você tem prefixos de nomes eficientes para o seu " + CloudServer.HotelName + "'s");
                }

				ExtraData = CloudServer.GetGame().GetChatManager().GetFilter().IsUnnaceptableWord(ExtraData, out string character) ? "" : ExtraData;

				if (string.IsNullOrEmpty(ExtraData))
                {
                    Session.SendMessage(new PurchaseOKComposer(Item, Item.Data, Item.Items));
                    Session.SendWhisper(character.ToUpper() + "Não é uma palavra apropriada, para que você não possa colocar este prefixo.", 34);
                    return;
                }

                if (Item.CostCredits > Session.GetHabbo().Credits || Item.CostPixels > Session.GetHabbo().Duckets || Item.CostDiamonds > Session.GetHabbo().Diamonds || Item.CostGotw > Session.GetHabbo().GOTWPoints)
                    return;

                if (Item.CostCredits > 0)
                {
                    Session.GetHabbo().Credits -= Item.CostCredits;
                    Session.SendMessage(new CreditBalanceComposer(Session.GetHabbo().Credits));
                }

                if (Item.CostPixels > 0)
                {
                    Session.GetHabbo().Duckets -= Item.CostPixels;
                    Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().Duckets, Session.GetHabbo().Duckets));//Love you, Tom.
                }

                if (Item.CostDiamonds > 0)
                {
                    Session.GetHabbo().Diamonds -= Item.CostDiamonds;
                    Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().Diamonds, 0, 5));
                }

                if (Item.CostGotw > 0)
                {
                    Session.GetHabbo().GOTWPoints -= Item.CostGotw;
                    Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().GOTWPoints, 0, 103));
                }

                using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.runFastQuery("UPDATE `users` SET `prefix_name` = '" + ExtraData + "' WHERE `id` = '" + Session.GetHabbo().Id + "' LIMIT 1");
                }

                Session.GetHabbo()._NamePrefix = ExtraData;
                Session.SendMessage(new ScrSendUserInfoComposer(Session.GetHabbo()));
                Session.SendMessage(new PurchaseOKComposer(Item, Item.Data, Item.Items));
                Session.SendMessage(new FurniListUpdateComposer());
                return;
            }

            if (baseItem.InteractionType == InteractionType.prefixcolor)
            {
                if (Item.CostCredits > Session.GetHabbo().Credits || Item.CostPixels > Session.GetHabbo().Duckets || Item.CostDiamonds > Session.GetHabbo().Diamonds || Item.CostGotw > Session.GetHabbo().GOTWPoints)
                    return;

                if (Item.CostCredits > 0)
                {
                    Session.GetHabbo().Credits -= Item.CostCredits;
                    Session.SendMessage(new CreditBalanceComposer(Session.GetHabbo().Credits));
                }

                if (Item.CostPixels > 0)
                {
                    Session.GetHabbo().Duckets -= Item.CostPixels;
                    Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().Duckets, Session.GetHabbo().Duckets));//Love you, Tom.
                }

                if (Item.CostDiamonds > 0)
                {
                    Session.GetHabbo().Diamonds -= Item.CostDiamonds;
                    Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().Diamonds, 0, 5));
                }

                if (Item.CostGotw > 0)
                {
                    Session.GetHabbo().GOTWPoints -= Item.CostGotw;
                    Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().GOTWPoints, 0, 103));
                }

                using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("UPDATE `users` SET `prefix_name_color` = @prefixn WHERE `id` = '" + Session.GetHabbo().Id + "' LIMIT 1");
                    dbClient.AddParameter("prefixn", "#" + Item.Name);
                    dbClient.RunQuery();
                }

                Session.GetHabbo()._NamePrefixColor = "#" + Item.Name;
                Session.SendMessage(new ScrSendUserInfoComposer(Session.GetHabbo()));
                Session.SendMessage(new PurchaseOKComposer(Item, Item.Data, Item.Items));
                Session.SendMessage(new FurniListUpdateComposer());
                return;
            }

            if (baseItem.InteractionType == InteractionType.CLUB_VIP || baseItem.InteractionType == InteractionType.CLUB_VIP2)
          {
               // int Months = 0;

                switch (baseItem.InteractionType)
                {
                    case InteractionType.CLUB_VIP:
                       /// Months = 1;
                           break;

                   case InteractionType.CLUB_VIP2:
                    //    Months = 3;
                       break;
               }

                if (Item.CostCredits > Session.GetHabbo().Credits || Item.CostPixels > Session.GetHabbo().Duckets || Item.CostDiamonds > Session.GetHabbo().Diamonds || Item.CostGotw > Session.GetHabbo().GOTWPoints)
                    return;

                if (Item.CostCredits > 0)
                {
                    Session.GetHabbo().Credits -= Item.CostCredits;
                    Session.SendMessage(new CreditBalanceComposer(Session.GetHabbo().Credits));
                }

                if (Item.CostPixels > 0)
                {
                    Session.GetHabbo().Duckets -= Item.CostPixels;
                    Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().Duckets, Session.GetHabbo().Duckets));//Love you, Tom.
                }

                if (Item.CostDiamonds > 0)
                {
                    Session.GetHabbo().Diamonds -= Item.CostDiamonds;
                    Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().Diamonds, 0, 5));
                }

                if (Item.CostGotw > 0)
                {
                    Session.GetHabbo().GOTWPoints -= Item.CostGotw;
                    Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().GOTWPoints, 0, 103));
                }
               

                Session.GetHabbo().GetClubManager().AddOrExtendSubscription("club_vip", 1 * 24 * 3600, Session);
                Session.GetHabbo().GetBadgeComponent().GiveBadge("DVIP", true, Session);

                CloudServer.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_VipClub", 1);
                Session.SendMessage(new ScrSendUserInfoComposer(Session.GetHabbo()));
                Session.SendMessage(new PurchaseOKComposer(Item, Item.Data, Item.Items));
                Session.SendMessage(new FurniListUpdateComposer());

                if (Session.GetHabbo().Rank > 2)
                {
                    Session.SendWhisper("Ops! Deu ruim ai!");
                    return;
                }

                else if (Session.GetHabbo().Rank < 2)
                {
                    using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.runFastQuery("UPDATE `users` SET `rank` = '2' WHERE `id` = '" + Session.GetHabbo().Id + "'");
                        dbClient.runFastQuery("UPDATE `users` SET `rank_vip` = '1' WHERE `id` = '" + Session.GetHabbo().Id + "'");
                        Session.GetHabbo().Rank = 2;
                        Session.GetHabbo().VIPRank = 1;
                    }
                }

                return;
            }

            if (Amount < 1 || Amount > 100 || !Item.HaveOffer)
                Amount = 1;

            int AmountPurchase = Item.Amount > 1 ? Item.Amount : Amount;

            int TotalCreditsCost = Amount > 1 ? ((Item.CostCredits * Amount) - ((int)Math.Floor((double)Amount / 6) * Item.CostCredits)) : Item.CostCredits;
            int TotalPixelCost = Amount > 1 ? ((Item.CostPixels * Amount) - ((int)Math.Floor((double)Amount / 6) * Item.CostPixels)) : Item.CostPixels;
            int TotalDiamondCost = Amount > 1 ? ((Item.CostDiamonds * Amount) - ((int)Math.Floor((double)Amount / 6) * Item.CostDiamonds)) : Item.CostDiamonds;
            int TotalGotwCost = Amount > 1 ? ((Item.CostGotw * Amount) - ((int)Math.Floor((double)Amount / 6) * Item.CostGotw)) : Item.CostGotw;
			
            if (Session.GetHabbo().Credits < TotalCreditsCost || Session.GetHabbo().Duckets < TotalPixelCost || Session.GetHabbo().Diamonds < TotalDiamondCost || Session.GetHabbo().GOTWPoints < TotalGotwCost)
                return;

            int LimitedEditionSells = 0;
            int LimitedEditionStack = 0;

            #region PREDESIGNED_ROOM
            if (Item.PredesignedId > 0 && CloudServer.GetGame().GetCatalog().GetPredesignedRooms().predesignedRoom.ContainsKey((uint)Item.PredesignedId))
            {
                #region SELECT ROOM AND CREATE NEW
                var predesigned = CloudServer.GetGame().GetCatalog().GetPredesignedRooms().predesignedRoom[(uint)Item.PredesignedId];
                var decoration = predesigned.RoomDecoration;

                var createRoom = CloudServer.GetGame().GetRoomManager().CreateRoom(Session, "Sala de "+Session.GetHabbo().Username, "Una Sala pre-decorada :)", predesigned.RoomModel, 1, 25, 1);

                createRoom.FloorThickness = int.Parse(decoration[0]);
                createRoom.WallThickness = int.Parse(decoration[1]);
                createRoom.Model.WallHeight = int.Parse(decoration[2]);
                createRoom.Hidewall = ((decoration[3] == "True") ? 1 : 0);
                createRoom.Wallpaper = decoration[4];
                createRoom.Landscape = decoration[5];
                createRoom.Floor = decoration[6];
                var newRoom = CloudServer.GetGame().GetRoomManager().LoadRoom(createRoom.Id);
                #endregion

                #region CREATE FLOOR ITEMS
                if (predesigned.FloorItems != null)
                    foreach (var floorItems in predesigned.FloorItemData)
                        using (var dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
                            dbClient.runFastQuery("INSERT INTO items VALUES (null, " + Session.GetHabbo().Id + ", " + newRoom.RoomId + ", " + floorItems.BaseItem + ", '" + floorItems.ExtraData + "', " +
                                floorItems.X + ", " + floorItems.Y + ", " + TextHandling.GetString(floorItems.Z) + ", " + floorItems.Rot + ", '', 0, 0);");
                #endregion

                #region CREATE WALL ITEMS
                if (predesigned.WallItems != null)
                    foreach (var wallItems in predesigned.WallItemData)
                        using (var dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
                            dbClient.runFastQuery("INSERT INTO items VALUES (null, " + Session.GetHabbo().Id + ", " + newRoom.RoomId + ", " + wallItems.BaseItem + ", '" + wallItems.ExtraData +
                                "', 0, 0, 0, 0, '" + wallItems.WallCoord + "', 0, 0);");
                #endregion

                #region VERIFY IF CONTAINS BADGE AND GIVE
                if (Item.Badge != string.Empty) Session.GetHabbo().GetBadgeComponent().GiveBadge(Item.Badge, true, Session);
                #endregion

                #region GENERATE ROOM AND SEND PACKET
                Session.SendMessage(new PurchaseOKComposer());
                Session.GetHabbo().GetInventoryComponent().UpdateItems(false);
                CloudServer.GetGame().GetRoomManager().LoadRoom(newRoom.Id).GetRoomItemHandler().LoadFurniture();
                var newFloorItems = newRoom.GetRoomItemHandler().GetFloor;
                foreach (var roomItem in newFloorItems) newRoom.GetRoomItemHandler().SetFloorItem(roomItem, roomItem.GetX, roomItem.GetY, roomItem.GetZ);
                var newWallItems = newRoom.GetRoomItemHandler().GetWall;
                foreach (var roomItem in newWallItems) newRoom.GetRoomItemHandler().SetWallItem(Session, roomItem);
                Session.SendMessage(new FlatCreatedComposer(newRoom.Id, newRoom.Name));
                #endregion
                return;
            }
            #endregion

            #region Create the extradata
            switch (Item.Data.InteractionType)
            {
                case InteractionType.NONE:
                    ExtraData = "";
                    break;

                case InteractionType.GUILD_FORUM:
                    Group Gp;
                    GroupForum Gf;
                    int GpId;
                    if (!int.TryParse(ExtraData, out GpId))
                    {
                        Session.SendNotification("Ops! Ocorreu algum erro ao obter o ID do grupo");
                        Session.SendMessage(new PurchaseOKComposer());
                        return;
                    }
                    if (!CloudServer.GetGame().GetGroupManager().TryGetGroup(GpId, out Gp))
                    {
                        Session.SendNotification("Ops! Esse ID não existe!");
                        Session.SendMessage(new PurchaseOKComposer());
                        return;
                    }

                    if (Gp.CreatorId != Session.GetHabbo().Id)
                    {
                        Session.SendNotification("Ops! Você não é o proprietário do grupo para que você criar o fórum.\n\nPrimero o fórum deve ser criado pelo grupo proprietário...");
                        Session.SendMessage(new PurchaseOKComposer());
                        return;
                    }

                    Gf = CloudServer.GetGame().GetGroupForumManager().CreateGroupForum(Gp);
                    Session.SendMessage(new RoomNotificationComposer("forums.delivered", new Dictionary<string, string>
                            { { "groupId", Gp.Id.ToString() },  { "groupName", Gp.Name } }));
                    break;

                case InteractionType.GUILD_ITEM:
                case InteractionType.GUILD_GATE:
                case InteractionType.HCGATE:
                case InteractionType.VIPGATE:
                    break;

                case InteractionType.PINATA:
                case InteractionType.PINATATRIGGERED:
                case InteractionType.MAGICEGG:
                case InteractionType.MAGICCHEST:
                    ExtraData = "0";
                    break;

                #region Pet handling

                case InteractionType.PET:
                    try
                    {
                        string[] Bits = ExtraData.Split('\n');
                        string PetName = Bits[0];
                        string Race = Bits[1];
                        string Color = Bits[2];

                        int.Parse(Race); // to trigger any possible errors

                        if (!PetUtility.CheckPetName(PetName))
                            return;

                        if (Race.Length > 2)
                            return;

                        if (Color.Length != 6)
                            return;

                        CloudServer.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_PetLover", 1);
                    }
                    catch (Exception e)
                    {
                        ExceptionLogger.LogException(e);
                        return;
                    }

                    break;

                #endregion

                case InteractionType.FLOOR:
                case InteractionType.WALLPAPER:
                case InteractionType.LANDSCAPE:

                    Double Number = 0;

                    try
                    {
                        if (string.IsNullOrEmpty(ExtraData))
                            Number = 0;
                        else
                            Number = Double.Parse(ExtraData, CloudServer.CultureInfo);
                    }
                    catch (Exception e)
                    {
                        ExceptionLogger.LogException(e);
                    }

                    ExtraData = Number.ToString().Replace(',', '.');
                    break; // maintain extra data // todo: validate

                case InteractionType.POSTIT:
                    ExtraData = "FFFF33";
                    break;

                case InteractionType.MOODLIGHT:
                    ExtraData = "1,1,1,#000000,255";
                    break;

                case InteractionType.TROPHY:
                    ExtraData = Session.GetHabbo().Username + Convert.ToChar(9) + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year + Convert.ToChar(9) + ExtraData;
                    break;

                case InteractionType.MANNEQUIN:
                    ExtraData = "m" + Convert.ToChar(5) + ".ch-210-1321.lg-285-92" + Convert.ToChar(5) + "Default Mannequin";
                    break;

                case InteractionType.FOOTBALL_GATE:
                    ExtraData = "hd-99999-99999.lg-270-62;hd-99999-99999.ch-630-62.lg-695-62";
                    break;

                case InteractionType.vikingtent:
                    ExtraData = "0";
                    break;

                case InteractionType.BADGE_DISPLAY:
                    if (!Session.GetHabbo().GetBadgeComponent().HasBadge(ExtraData))
                    {
                        Session.SendMessage(new BroadcastMessageAlertComposer("Parece que você não é dono desse Emblema!"));
                        return;
                    }

                    ExtraData = ExtraData + Convert.ToChar(9) + Session.GetHabbo().Username + Convert.ToChar(9) + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year;
                    break;

                case InteractionType.BADGE:
                    {
                        if (Session.GetHabbo().GetBadgeComponent().HasBadge(Item.Data.ItemName))
                        {
                            Session.SendMessage(new PurchaseErrorComposer(1));
                            return;
                        }
                        break;
                    }
                default:
                    ExtraData = "";
                    break;
            }
            #endregion


            if (Item.IsLimited)
            {
                if (Item.LimitedEditionStack <= Item.LimitedEditionSells)
                {
                    Session.SendNotification("Essa noticia expirou!\n\n" + "Por favor, note que você não tenha recebido um outro artigo (você também não foi cobrado por ele!)");
                    Session.SendMessage(new CatalogUpdatedComposer());
                    Session.SendMessage(new PurchaseOKComposer());
                    return;
                }

                Item.LimitedEditionSells++;
                using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("UPDATE `catalog_items` SET `limited_sells` = @limitSells WHERE `id` = @itemId LIMIT 1");
                    dbClient.AddParameter("limitSells", Item.LimitedEditionSells);
                    dbClient.AddParameter("itemId", Item.Id);
                    dbClient.RunQuery();

                    LimitedEditionSells = Item.LimitedEditionSells;
                    LimitedEditionStack = Item.LimitedEditionStack;
                }
            }

            if (Item.CostCredits > 0)
            {
                Session.GetHabbo().Credits -= TotalCreditsCost;
                Session.SendMessage(new CreditBalanceComposer(Session.GetHabbo().Credits));
            }

            if (Item.CostPixels > 0)
            {
                Session.GetHabbo().Duckets -= TotalPixelCost;
                Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().Duckets, Session.GetHabbo().Duckets));//Love you, Tom.
            }

            if (Item.CostDiamonds > 0)
            {
                Session.GetHabbo().Diamonds -= TotalDiamondCost;
                Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().Diamonds, 0, 5));
            }

            if (Item.CostGotw > 0)
            {
                Session.GetHabbo().GOTWPoints -= TotalGotwCost;
                Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().GOTWPoints, 0, 103));
            }

            Item NewItem = null;
            switch (Item.Data.Type.ToString().ToLower())
            {
                default:
                    List<Item> GeneratedGenericItems = new List<Item>();

                    switch (Item.Data.InteractionType)
                    {
                        default:
                            if (AmountPurchase > 1)
                            {
                                List<Item> Items = ItemFactory.CreateMultipleItems(Item.Data, Session.GetHabbo(), ExtraData, AmountPurchase);

                                if (Items != null)
                                {
                                    GeneratedGenericItems.AddRange(Items);
                                }
                            }
                            else
                            {
                                NewItem = ItemFactory.CreateSingleItemNullable(Item.Data, Session.GetHabbo(), ExtraData, ExtraData, 0, LimitedEditionSells, LimitedEditionStack);

                                if (NewItem != null)
                                {
                                    GeneratedGenericItems.Add(NewItem);
                                }
                            }
                            break;

                        case InteractionType.GUILD_GATE:
                        case InteractionType.GUILD_ITEM:
                        case InteractionType.GUILD_FORUM:
                            int groupId = 0;
                            int.TryParse(ExtraData, out groupId);
                            if (AmountPurchase > 1)
                            {
                                List<Item> Items = ItemFactory.CreateMultipleItems(Item.Data, Session.GetHabbo(), ExtraData, AmountPurchase, groupId);

								if (Items != null)
								{
									GeneratedGenericItems.AddRange(Items);
								}
							}
							else
							{
								NewItem = ItemFactory.CreateSingleItemNullable(Item.Data, Session.GetHabbo(), ExtraData, ExtraData, groupId);

								if (NewItem != null)
								{
									GeneratedGenericItems.Add(NewItem);
								}
							}
							break;

						case InteractionType.MUSIC_DISC:
                            string flags = Convert.ToString(Item.ExtradataInt);
                            if (AmountPurchase > 1)
                            {
                                List<Item> Items = ItemFactory.CreateMultipleItems(Item.Data, Session.GetHabbo(), flags, AmountPurchase);

                                if (Items != null)
                                {
                                    GeneratedGenericItems.AddRange(Items);
                                }
                            }
                            else
                            {
                                NewItem = ItemFactory.CreateSingleItemNullable(Item.Data, Session.GetHabbo(), flags, flags);

                                if (NewItem != null)
                                {
                                    GeneratedGenericItems.Add(NewItem);
                                }
                            }
                            break;

                        case InteractionType.ARROW:
                        case InteractionType.TELEPORT:
                            for (int i = 0; i < AmountPurchase; i++)
                            {
                                List<Item> TeleItems = ItemFactory.CreateTeleporterItems(Item.Data, Session.GetHabbo());

                                if (TeleItems != null)
                                {
                                    GeneratedGenericItems.AddRange(TeleItems);
                                }
                            }
                            break;

                        case InteractionType.MOODLIGHT:
                            {
                                if (AmountPurchase > 1)
                                {
                                    List<Item> Items = ItemFactory.CreateMultipleItems(Item.Data, Session.GetHabbo(), ExtraData, AmountPurchase);

                                    if (Items != null)
                                    {
                                        GeneratedGenericItems.AddRange(Items);
                                        foreach (Item I in Items)
                                        {
                                            ItemFactory.CreateMoodlightData(I);
                                        }
                                    }
                                }
                                else
                                {
                                    NewItem = ItemFactory.CreateSingleItemNullable(Item.Data, Session.GetHabbo(), ExtraData, ExtraData);

                                    if (NewItem != null)
                                    {
                                        GeneratedGenericItems.Add(NewItem);
                                        ItemFactory.CreateMoodlightData(NewItem);
                                    }
                                }
                            }
                            break;
                

                        case InteractionType.TONER:
                            {
                                if (AmountPurchase > 1)
                                {
                                    List<Item> Items = ItemFactory.CreateMultipleItems(Item.Data, Session.GetHabbo(), ExtraData, AmountPurchase);

                                    if (Items != null)
                                    {
                                        GeneratedGenericItems.AddRange(Items);
                                        foreach (Item I in Items)
                                        {
                                            ItemFactory.CreateTonerData(I);
                                        }
                                    }
                                }
                                else
                                {
                                    NewItem = ItemFactory.CreateSingleItemNullable(Item.Data, Session.GetHabbo(), ExtraData, ExtraData);

                                    if (NewItem != null)
                                    {
                                        GeneratedGenericItems.Add(NewItem);
                                        ItemFactory.CreateTonerData(NewItem);
                                    }
                                }
                            }
                            break;

                        case InteractionType.DEAL:
                            {
                                //Fetch the deal where the ID is this items ID.
                                var DealItems = (from d in Page.Deals.Values.ToList() where d.Id == Item.Id select d);

                                //This bit, iterating ONE item? How can I make this simpler
                                foreach (CatalogDeal DealItem in DealItems)
                                {
                                    //Here I loop the DealItems ItemDataList.
                                    foreach (CatalogItem CatalogItem in DealItem.ItemDataList.ToList())
                                    {
                                        List<Item> Items = ItemFactory.CreateMultipleItems(CatalogItem.Data, Session.GetHabbo(), "", AmountPurchase);

                                        if (Items != null)
                                        {
                                            GeneratedGenericItems.AddRange(Items);
                                        }
                                    }
                                }
                                break;
                            }

                    }

                    foreach (Item PurchasedItem in GeneratedGenericItems)
                    {
                        if (Session.GetHabbo().GetInventoryComponent().TryAddItem(PurchasedItem))
                        {
                            //Session.SendMessage(new FurniListAddComposer(PurchasedItem));
                            Session.SendMessage(new FurniListNotificationComposer(PurchasedItem.Id, 1));
                        }
                    }
                    break;

                case "e":
                    AvatarEffect Effect = null;

                    if (Session.GetHabbo().Effects().HasEffect(Item.Data.SpriteId))
                    {
                        Effect = Session.GetHabbo().Effects().GetEffectNullable(Item.Data.SpriteId);

                        if (Effect != null)
                        {
                            Effect.AddToQuantity();
                        }
                    }
                    else
                        Effect = AvatarEffectFactory.CreateNullable(Session.GetHabbo(), Item.Data.SpriteId, 3600);

                    if (Effect != null)// && Session.GetHabbo().Effects().TryAdd(Effect))
                    {
                        Session.SendMessage(new AvatarEffectAddedComposer(Item.Data.SpriteId, 3600));
                    }
                    break;

                case "r":
                    Bot Bot = BotUtility.CreateBot(Item.Data, Session.GetHabbo().Id);
                    if (Bot != null)
                    {
                        Session.GetHabbo().GetInventoryComponent().TryAddBot(Bot);
                        Session.SendMessage(new BotInventoryComposer(Session.GetHabbo().GetInventoryComponent().GetBots()));
                        Session.SendMessage(new FurniListNotificationComposer(Bot.Id, 5));
                    }
                    else
                        Session.SendNotification("Houve um erro ao comprar isso!");
                    break;

                case "b":
                    {
                        Session.GetHabbo().GetBadgeComponent().GiveBadge(Item.Data.ItemName, true, Session);
                        Session.SendMessage(new FurniListNotificationComposer(0, 4));
                        break;
                    }

                case "p":
                    {
                        string[] PetData = ExtraData.Split('\n');

                        Pet GeneratedPet = PetUtility.CreatePet(Session.GetHabbo().Id, PetData[0], Item.Data.BehaviourData, PetData[1], PetData[2]);
                        if (GeneratedPet != null)
                        {
                            Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet);

                            Session.SendMessage(new FurniListNotificationComposer(GeneratedPet.PetId, 3));
                            Session.SendMessage(new PetInventoryComposer(Session.GetHabbo().GetInventoryComponent().GetPets()));

							if (CloudServer.GetGame().GetItemManager().GetItem(320, out ItemData PetFood))
							{
								Item Food = ItemFactory.CreateSingleItemNullable(PetFood, Session.GetHabbo(), "", "");
								if (Food != null)
								{
									Session.GetHabbo().GetInventoryComponent().TryAddItem(Food);
									Session.SendMessage(new FurniListNotificationComposer(Food.Id, 1));
								}
							}
						}
                        break;
                    }
            }

            if (Item.Badge != string.Empty) Session.GetHabbo().GetBadgeComponent().GiveBadge(Item.Badge, true, Session);
            Session.SendMessage(new PurchaseOKComposer(Item, Item.Data, Item.Items));
            Session.SendMessage(new FurniListUpdateComposer());
           
            }
        }
    }