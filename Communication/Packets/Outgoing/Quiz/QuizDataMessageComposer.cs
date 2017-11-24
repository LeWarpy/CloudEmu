using System;
using Cloud.HabboHotel.GameClients;

namespace Cloud.Communication.Packets.Outgoing.Quiz
{
	class QuizDataMessageComposer : ServerPacket
    {
        public QuizDataMessageComposer(GameClient Session) : base(ServerPacketHeader.QuizDataMessageComposer)
        {
            Random rnd = new Random();
            int domanda1 = rnd.Next(0, 3);
            int domanda2 = rnd.Next(0, 3);
            int domanda3 = rnd.Next(0, 3);
            int domanda4 = rnd.Next(0, 3);
            int domanda5 = rnd.Next(0, 3);
			WriteString("HabboWay1");
			WriteInteger(0);
			WriteInteger(domanda1);
			WriteInteger(domanda2);
			WriteInteger(domanda3);
			WriteInteger(domanda4);
			WriteInteger(domanda5);
            CloudServer.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_SafetyQuizGraduate", 1, false);
        }
    }
}
