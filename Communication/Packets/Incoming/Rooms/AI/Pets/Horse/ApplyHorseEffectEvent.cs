using Cloud.Communication.Packets.Outgoing.Catalog;
using Cloud.HabboHotel.Rooms;
using Cloud.HabboHotel.Items;
using Cloud.HabboHotel.Catalog.Utilities;
using Cloud.Communication.Packets.Outgoing.Inventory.Furni;
using Cloud.Communication.Packets.Outgoing.Rooms.Engine;
using Cloud.Communication.Packets.Outgoing.Rooms.AI.Pets;

using Cloud.Database.Interfaces;

namespace Cloud.Communication.Packets.Incoming.Rooms.AI.Pets.Horse
{
    class ApplyHorseEffectEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().InRoom)
                return;

            Room Room;

            if (!CloudServer.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out Room))
                return;

            int ItemId = Packet.PopInt();
            Item Item = Room.GetRoomItemHandler().GetItem(ItemId);
            if (Item == null)
                return;

            int PetId = Packet.PopInt();

            RoomUser PetUser = null;
            if (!Room.GetRoomUserManager().TryGetPet(PetId, out PetUser))
                return;

            if (PetUser.PetData == null || PetUser.PetData.OwnerId != Session.GetHabbo().Id)
                return;

            //If the horse already had a saddle on his back, it will replace this current saddle, so we put back it on the inventory.
            if (Item.Data.InteractionType == InteractionType.HORSE_SADDLE_1 || Item.Data.InteractionType == InteractionType.HORSE_SADDLE_2)
            {
                if (PetUser.PetData.Saddle > 0)
                {
                    //Fetch the furniture Id for the pets current saddle.
                    int SaddleId = ItemUtility.GetSaddleId(PetUser.PetData.Saddle);
                    //Give the saddle back to the user.
                    ItemData ItemData = null;
                    if (!CloudServer.GetGame().GetItemManager().GetItem(SaddleId, out ItemData))
                        return;
                    Item NewSaddle = ItemFactory.CreateSingleItemNullable(ItemData, Session.GetHabbo(), "", "", 0, 0, 0);
                    if (NewSaddle != null)
                    {
                        Session.GetHabbo().GetInventoryComponent().TryAddItem(NewSaddle);
                        Session.SendMessage(new FurniListNotificationComposer(NewSaddle.Id, 1));
                        Session.SendMessage(new PurchaseOKComposer());
                        Session.SendMessage(new FurniListAddComposer(NewSaddle));
                        Session.SendMessage(new FurniListUpdateComposer());
                    }
                }
            }

            if (Item.Data.InteractionType == InteractionType.HORSE_SADDLE_1)
            {
                PetUser.PetData.Saddle = 9;
                using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.runFastQuery("UPDATE `bots_petdata` SET `have_saddle` = '9' WHERE `id` = '" + PetUser.PetData.PetId + "' LIMIT 1");
                    dbClient.runFastQuery("DELETE FROM `items` WHERE `id` = '" + Item.Id + "' LIMIT 1");
                }

                //We only want to use this if we're successful. 
                Room.GetRoomItemHandler().RemoveFurniture(Session, Item.Id, false);
            }
            else if (Item.Data.InteractionType == InteractionType.HORSE_SADDLE_2)
            {
                PetUser.PetData.Saddle = 10;
                using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.runFastQuery("UPDATE `bots_petdata` SET `have_saddle` = '10' WHERE `id` = '" + PetUser.PetData.PetId + "' LIMIT 1");
                    dbClient.runFastQuery("DELETE FROM `items` WHERE `id` = '" + Item.Id + "' LIMIT 1");
                }

                //We only want to use this if we're successful. 
                Room.GetRoomItemHandler().RemoveFurniture(Session, Item.Id, false);
            }
            else if (Item.Data.InteractionType == InteractionType.HORSE_HAIRSTYLE)
            {
                int DefaultHairType = 100;
                int HairType = int.Parse(Item.GetBaseItem().ItemName.Split('_')[2]);
                if (HairType == 0)
                    PetUser.PetData.PetHair = -1;
                else
                    PetUser.PetData.PetHair = DefaultHairType + HairType;
                using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.runFastQuery("UPDATE `bots_petdata` SET `pethair` = '" + PetUser.PetData.PetHair + "' WHERE `id` = '" + PetUser.PetData.PetId + "' LIMIT 1");
                    dbClient.runFastQuery("DELETE FROM `items` WHERE `id` = '" + Item.Id + "' LIMIT 1");
                }
                //We only want to use this if we're successful. 
                Room.GetRoomItemHandler().RemoveFurniture(Session, Item.Id, false);
            }
            else if (Item.Data.InteractionType == InteractionType.HORSE_HAIR_DYE)
            {
                int DefaultHairDye = 48;
                int HairDye = int.Parse(Item.GetBaseItem().ItemName.Split('_')[2]);
                if (HairDye == 1)
                    PetUser.PetData.HairDye = 1;
                else if (HairDye >= 13)
                    PetUser.PetData.HairDye = DefaultHairDye + HairDye + 20;
                else
                    PetUser.PetData.HairDye = DefaultHairDye + HairDye;
                using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.runFastQuery("UPDATE `bots_petdata` SET `hairdye` = '" + PetUser.PetData.HairDye + "' WHERE `id` = '" + PetUser.PetData.PetId + "' LIMIT 1");
                    dbClient.runFastQuery("DELETE FROM `items` WHERE `id` = '" + Item.Id + "' LIMIT 1");
                }
                //We only want to use this if we're successful. 
                Room.GetRoomItemHandler().RemoveFurniture(Session, Item.Id, false);
            }
            else if (Item.Data.InteractionType == InteractionType.HORSE_BODY_DYE)
            {
                int Race = int.Parse(Item.GetBaseItem().ItemName.Split('_')[2]);
                int RaceType = (Race * 4) - 2;
                if (Race >= 13 && Race <= 18)
                {
                    RaceType = ((2 + Race) * 4) + 1;
                }
                if (Race == 0)
                    RaceType = 0;
                PetUser.PetData.Race = RaceType.ToString();
                using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.runFastQuery("UPDATE `bots_petdata` SET `race` = '" + PetUser.PetData.Race + "' WHERE `id` = '" + PetUser.PetData.PetId + "' LIMIT 1");
                    dbClient.runFastQuery("DELETE FROM `items` WHERE `id` = '" + Item.Id + "' LIMIT 1");
                }
                //We only want to use this if we're successful. 
                Room.GetRoomItemHandler().RemoveFurniture(Session, Item.Id, false);
            }

            //Update the Pet and the Pet figure information.
            Room.SendMessage(new UsersComposer(PetUser));
            Room.SendMessage(new PetHorseFigureInformationComposer(PetUser));
        }
    }
}