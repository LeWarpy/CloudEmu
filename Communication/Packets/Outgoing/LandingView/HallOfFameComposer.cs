using Cloud.Communication.Packets.Incoming.LandingView;

namespace Cloud.Communication.Packets.Outgoing.LandingView
{
	class HallOfFameComposer : ServerPacket
    {
        public HallOfFameComposer() : base(ServerPacketHeader.UpdateHallOfFameListMessageComposer)
        {
			WriteString("halloffame.staff");
            GetHallOfFame.GetInstance().Serialize(this);
            return;
        }
    }
}
