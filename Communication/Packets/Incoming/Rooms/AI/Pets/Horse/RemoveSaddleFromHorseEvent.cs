﻿using Cloud.HabboHotel.Rooms;
using Cloud.Communication.Packets.Outgoing.Rooms.Engine;
using Cloud.Communication.Packets.Outgoing.Rooms.AI.Pets;
using Cloud.HabboHotel.Catalog.Utilities;
using Cloud.HabboHotel.Items;
using Cloud.Communication.Packets.Outgoing.Inventory.Furni;
using Cloud.Communication.Packets.Outgoing.Catalog;
using Cloud.Database.Interfaces;
using System.Drawing;


namespace Cloud.Communication.Packets.Incoming.Rooms.AI.Pets.Horse
{
	class RemoveSaddleFromHorseEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().InRoom)
                return;


            Room Room = null;


            if (!CloudServer.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out Room))
                return;


            RoomUser PetUser = null;


            if (!Room.GetRoomUserManager().TryGetPet(Packet.PopInt(), out PetUser))
                return;


            //Fetch the furniture Id for the pets current saddle.
            int SaddleId = ItemUtility.GetSaddleId(PetUser.PetData.Saddle);


            //Remove the saddle from the pet.
            PetUser.PetData.Saddle = 0;


            using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.runFastQuery("UPDATE `bots_petdata` SET `have_saddle` = 0 WHERE `id` = '" + PetUser.PetData.PetId + "' LIMIT 1");
            }




            //When removing Saddle From Horse the user gets down the horse
            if (PetUser.RidingHorse)
            {
                RoomUser UserRiding = Room.GetRoomUserManager().GetRoomUserByVirtualId(PetUser.HorseID);
                if (UserRiding != null)
                {
                    UserRiding.RidingHorse = false;
                    PetUser.RidingHorse = false;
                    UserRiding.ApplyEffect(-1);
                    UserRiding.MoveTo(new Point(UserRiding.X + 1, UserRiding.Y + 1));
                }
                else
                    PetUser.RidingHorse = false;
            }


            ItemData ItemData = null;


            if (!CloudServer.GetGame().GetItemManager().GetItem(SaddleId, out ItemData))
                return;


            //Creates the item for the user
            Item Item = ItemFactory.CreateSingleItemNullable(ItemData, Session.GetHabbo(), "", "", 0, 0, 0);


            if (Item != null)
            {
                Session.GetHabbo().GetInventoryComponent().TryAddItem(Item);
                Session.SendMessage(new FurniListNotificationComposer(Item.Id, 1));
                Session.SendMessage(new PurchaseOKComposer());
                Session.SendMessage(new FurniListAddComposer(Item));
                Session.SendMessage(new FurniListUpdateComposer());
            }


            Room.SendMessage(new UsersComposer(PetUser));
            Room.SendMessage(new PetHorseFigureInformationComposer(PetUser));
        }
    }
}