using System.Collections.Generic;
using Cloud.HabboHotel.Users;
using Cloud.Communication.Packets.Outgoing.Users;

namespace Cloud.Communication.Packets.Incoming.Users
{
    class GetIgnoredUsersEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient session, ClientPacket packet)
        {
            List<string> ignoredUsers = new List<string>();

            foreach (int userId in new List<int>(session.GetHabbo().GetIgnores().IgnoredUserIds()))
            {
                Habbo player = CloudServer.GetHabboById(userId);
                if (player != null)
                {
                    if (!ignoredUsers.Contains(player.Username))
                        ignoredUsers.Add(player.Username);
                }
            }

            session.SendMessage(new IgnoredUsersComposer(ignoredUsers));
        }
    }
}