using System.Collections.Generic;
using Cloud.HabboHotel.GameClients;
using Cloud.HabboHotel.Quests;
using Cloud.Communication.Packets.Incoming;

namespace Cloud.Communication.Packets.Incoming.Quests
{
    public class GetQuestListEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            CloudServer.GetGame().GetQuestManager().GetList(Session, null);
        }
    }
}