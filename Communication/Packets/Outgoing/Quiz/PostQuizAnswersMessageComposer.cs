using System;
using Cloud.HabboHotel.GameClients;

namespace Cloud.Communication.Packets.Outgoing.Quiz
{
	class PostQuizAnswersMessageComposer : ServerPacket
    {
        public PostQuizAnswersMessageComposer(GameClient Session) : base(ServerPacketHeader.PostQuizAnswersMessageComposer)
        {
            Random rnd = new Random();
            int risposta1 = rnd.Next(0, 3);
            int risposta2 = rnd.Next(0, 3);
            int risposta3 = rnd.Next(0, 3);
            int risposta4 = rnd.Next(0, 3);
            int risposta5 = rnd.Next(0, 3);
			WriteString("HabboWay1");
			WriteInteger(5);
			WriteInteger(risposta1);
			WriteInteger(risposta2);
			WriteInteger(risposta3);
			WriteInteger(risposta4);
			WriteInteger(risposta5);
            if (risposta5 > rnd.Next(0, 3))
            {
                CloudServer.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_SafetyQuizGraduate", 1, false);
            }
        }
    }
}
