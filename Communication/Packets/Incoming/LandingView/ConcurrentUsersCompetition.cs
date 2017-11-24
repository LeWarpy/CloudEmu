using Cloud.Communication.Packets.Outgoing.LandingView;
using Cloud.HabboHotel.GameClients;
using System.Linq;


namespace Cloud.Communication.Packets.Incoming.LandingView
{
    class ConcurrentUsersCompetition : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            int goal = int.Parse(CloudServer.GetGame().GetSettingsManager().TryGetValue("usersconcurrent_goal")); ;
            int UsersOnline = CloudServer.GetGame().GetClientManager().Count;
            foreach (GameClient Target in CloudServer.GetGame().GetClientManager().GetClients.ToList())
            {
                if (UsersOnline < goal)
                {
                    int type = 1;
                    Target.SendMessage(new ConcurrentUsersGoalProgressComposer(UsersOnline, type, goal));
                }
                else if (!Target.GetHabbo().GetStats().PurchaseUsersConcurrent && UsersOnline >= goal)
                {
                    int type = 2;
                    Target.SendMessage(new ConcurrentUsersGoalProgressComposer(UsersOnline, type, goal));
                }
                else if (Target.GetHabbo().GetStats().PurchaseUsersConcurrent && UsersOnline >= goal)
                {
                    int type = 3;
                    Target.SendMessage(new ConcurrentUsersGoalProgressComposer(UsersOnline, type, goal));
                }
                else
                {
                    int type = 0;
                    Target.SendMessage(new ConcurrentUsersGoalProgressComposer(UsersOnline, type, goal));
                }
            }
        }
    }
}
