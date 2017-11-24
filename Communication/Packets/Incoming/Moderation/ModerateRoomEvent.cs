using System.Linq;
using Cloud.HabboHotel.Rooms;
using Cloud.Communication.Packets.Outgoing.Navigator;
using Cloud.Communication.Packets.Outgoing.Rooms.Settings;
using Cloud.Database.Interfaces;


namespace Cloud.Communication.Packets.Incoming.Moderation
{
    class ModerateRoomEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().GetPermissions().HasRight("mod_tool"))
                return;

            Room Room = null;
            if(!CloudServer.GetGame().GetRoomManager().TryGetRoom(Packet.PopInt(), out Room))
                return;

            bool SetLock = Packet.PopInt() == 1;
            bool SetName = Packet.PopInt() == 1;
            bool KickAll = Packet.PopInt() == 1;

            if (SetName)
            {
                Room.RoomData.Name = "Inadequado para o Hotel Management";
                Room.RoomData.Description = "Inadequado para o Hotel Management";
            }

            if (SetLock)
                Room.RoomData.Access = RoomAccess.DOORBELL;

            if (Room.Tags.Count > 0)
                Room.ClearTags();

            if (Room.RoomData.HasActivePromotion)
                Room.RoomData.EndPromotion();

            using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
            {
                if (SetName && SetLock)
                    dbClient.runFastQuery("UPDATE `rooms` SET `caption` = 'Inadequado para o Hotel Management', `description` = 'Inadequado para o Hotel Management', `tags` = '', `state` = '1' WHERE `id` = '" + Room.RoomId + "' LIMIT 1");
                else if (SetName && !SetLock)
                    dbClient.runFastQuery("UPDATE `rooms` SET `caption` = 'Inadequado para o Hotel Management', `description` = 'Inadequado para o Hotel Management', `tags` = '' WHERE `id` = '" + Room.RoomId + "' LIMIT 1");
                else if (!SetName && SetLock)
                    dbClient.runFastQuery("UPDATE `rooms` SET `state` = '1', `tags` = '' WHERE `id` = '" + Room.RoomId + "' LIMIT 1");
            }

            Room.SendMessage(new RoomSettingsSavedComposer(Room.RoomId));
            Room.SendMessage(new RoomInfoUpdatedComposer(Room.RoomId));

            if (KickAll)
            {
                foreach (RoomUser RoomUser in Room.GetRoomUserManager().GetUserList().ToList())
                {
                    if (RoomUser == null || RoomUser.IsBot)
                        continue;

                    if (RoomUser.GetClient() == null || RoomUser.GetClient().GetHabbo() == null)
                        continue;

                    if (RoomUser.GetClient().GetHabbo().Rank >= Session.GetHabbo().Rank || RoomUser.GetClient().GetHabbo().Id == Session.GetHabbo().Id)
                        continue;

                    Room.GetRoomUserManager().RemoveUserFromRoom(RoomUser.GetClient(), true, false);
                }
            }
        }
    }
}
