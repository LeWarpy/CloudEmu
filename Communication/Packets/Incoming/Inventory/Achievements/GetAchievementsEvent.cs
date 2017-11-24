
using System.Linq;


using Cloud.Communication.Packets.Outgoing.Inventory.Achievements;

namespace Cloud.Communication.Packets.Incoming.Inventory.Achievements
{
    class GetAchievementsEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            Session.SendMessage(new AchievementsComposer(Session, CloudServer.GetGame().GetAchievementManager()._achievements.Values.ToList()));
        }
    }
}
