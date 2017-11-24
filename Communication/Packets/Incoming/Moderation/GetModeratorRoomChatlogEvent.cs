using System;
using System.Data;
using System.Collections.Generic;
using Cloud.HabboHotel.Rooms;
using Cloud.Communication.Packets.Outgoing.Moderation;
using Cloud.Database.Interfaces;
using Cloud.HabboHotel.Users;
using Cloud.HabboHotel.Rooms.Chat.Logs;

namespace Cloud.Communication.Packets.Incoming.Moderation
{
    class GetModeratorRoomChatlogEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null)
                return;

            if (!Session.GetHabbo().GetPermissions().HasRight("mod_tool"))
                return;

            int Junk = Packet.PopInt();
            int RoomId = Packet.PopInt();

            Room Room = null;
            if (!CloudServer.GetGame().GetRoomManager().TryGetRoom(RoomId, out Room))
            {
                return;
            }

            CloudServer.GetGame().GetChatManager().GetLogs().FlushAndSave();

            List<ChatlogEntry> Chats = new List<ChatlogEntry>();

            DataTable Data = null;
            using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `chatlogs` WHERE `room_id` = @id ORDER BY `id` DESC LIMIT 100");
                dbClient.AddParameter("id", RoomId);
                Data = dbClient.getTable();

                if (Data != null)
                {
                    foreach (DataRow Row in Data.Rows)
                    {
                        Habbo Habbo = CloudServer.GetHabboById(Convert.ToInt32(Row["user_id"]));

                        if (Habbo != null)
                        {
                            Chats.Add(new ChatlogEntry(Convert.ToInt32(Row["user_id"]), RoomId, Convert.ToString(Row["message"]), Convert.ToDouble(Row["timestamp"]), Habbo));
                        }
                    }
                }
            }

            Session.SendMessage(new ModeratorRoomChatlogComposer(Room, Chats));
        }
    }
}