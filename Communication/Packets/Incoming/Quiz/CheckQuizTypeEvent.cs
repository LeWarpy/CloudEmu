using Cloud.HabboHotel.GameClients;

namespace Cloud.Communication.Packets.Incoming.Quiz
{
	class CheckQuizTypeEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            CloudServer.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_SafetyQuizGraduate", 1, false);
        }
    }
}
