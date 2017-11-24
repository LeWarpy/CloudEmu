using System;
using Cloud.HabboHotel.GameClients;

namespace Cloud.Communication.Packets.Outgoing.Misc
{
	class LatencyTestComposer : ServerPacket
    {
        public LatencyTestComposer(GameClient Session, int testResponce)
            : base(ServerPacketHeader.LatencyResponseMessageComposer)
        {
            if (Session == null)
                return;

            Session.TimePingedReceived = DateTime.Now;

			WriteInteger(testResponce);
            CloudServer.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_AllTimeHotelPresence", 1);
        }
    }
}
