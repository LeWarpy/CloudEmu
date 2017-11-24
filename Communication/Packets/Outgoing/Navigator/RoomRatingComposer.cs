namespace Cloud.Communication.Packets.Outgoing.Navigator
{
	class RoomRatingComposer : ServerPacket
    {
        public RoomRatingComposer(int Score, bool CanVote)
            : base(ServerPacketHeader.RoomRatingMessageComposer)
        {
			WriteInteger(Score);
			WriteBoolean(CanVote);
        }
    }
}
