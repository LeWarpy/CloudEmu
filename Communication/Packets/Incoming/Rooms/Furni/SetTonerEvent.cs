﻿
using Cloud.HabboHotel.Rooms;
using Cloud.HabboHotel.Items;
using Cloud.Communication.Packets.Outgoing.Rooms.Engine;
using Cloud.Database.Interfaces;


namespace Cloud.Communication.Packets.Incoming.Rooms.Furni
{
	class SetTonerEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            Room Room;

            if (!CloudServer.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out Room))
                return;

            if (!Room.CheckRights(Session, true))
                return;

            if (Room.TonerData == null)
                return;

            Item Item = Room.GetRoomItemHandler().GetItem(Room.TonerData.ItemId);

            if (Item == null || Item.GetBaseItem().InteractionType != InteractionType.TONER)
                return;

            int Id = Packet.PopInt();
            int Int1 = Packet.PopInt();
            int Int2 = Packet.PopInt();
            int Int3 = Packet.PopInt();

            if (Int1 > 255 || Int2 > 255 || Int3 > 255)
                return;

            using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `room_items_toner` SET `enabled` = '1', `data1` = @data1, `data2` = @data2, `data3` = @data3 WHERE `id` = @itemId LIMIT 1");
                dbClient.AddParameter("itemId", Item.Id);
                dbClient.AddParameter("data1", Int1);
                dbClient.AddParameter("data3", Int3);
                dbClient.AddParameter("data2", Int2);
                dbClient.RunQuery();
            }

            Room.TonerData.Hue = Int1;
            Room.TonerData.Saturation = Int2;
            Room.TonerData.Lightness = Int3;
            Room.TonerData.Enabled = 1;

            Room.SendMessage(new ObjectUpdateComposer(Item, Room.OwnerId));
            Item.UpdateState();
        }
    }
}
