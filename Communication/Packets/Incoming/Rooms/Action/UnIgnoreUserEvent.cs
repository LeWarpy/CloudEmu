using Cloud.Database.Interfaces;
using Cloud.HabboHotel.Rooms;
using Cloud.HabboHotel.Users;
using Cloud.Communication.Packets.Outgoing.Rooms.Action;

namespace Cloud.Communication.Packets.Incoming.Rooms.Action
{
    class UnIgnoreUserEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient session, ClientPacket packet)
        {
            if (!session.GetHabbo().InRoom)
                return;

            Room Room = session.GetHabbo().CurrentRoom;
            if (Room == null)
                return;

            string Username = packet.PopString();

            Habbo Player = CloudServer.GetHabboByUsername(Username);
            if (Player == null)
                return;

            if (!session.GetHabbo().GetIgnores().TryGet(Player.Id))
                return;

            if (session.GetHabbo().GetIgnores().TryRemove(Player.Id))
            {
                using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("DELETE FROM `user_ignores` WHERE `user_id` = @uid AND `ignore_id` = @ignoreId");
                    dbClient.AddParameter("uid", session.GetHabbo().Id);
                    dbClient.AddParameter("ignoreId", Player.Id);
                    dbClient.RunQuery();
                }

                session.SendMessage(new IgnoreStatusComposer(3, Player.Username));
            }
        }
    }
}
