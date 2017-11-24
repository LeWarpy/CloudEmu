using Cloud.HabboHotel.GameClients;

namespace Cloud.Communication.Packets.Outgoing.Talents
{
    class TalentTrackLevelComposer : ServerPacket
    {
        public TalentTrackLevelComposer(GameClient Session, string packet)
            : base(ServerPacketHeader.TalentTrackLevelMessageComposer)
        {
			WriteString(packet);
			WriteInteger(1);
			WriteInteger(4);
        }
    }
}