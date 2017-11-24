/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cloud.HabboHotel.GameClients;
using Cloud.Communication.Packets.Outgoing.LandingView;

namespace Cloud.Communication.Packets.Incoming.Quests
{
    class GetDailyQuestEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            int UsersOnline = CloudServer.GetGame().GetClientManager().Count;

            Session.SendMessage(new ConcurrentUsersGoalProgressComposer(UsersOnline));
        }
    }
}*/
