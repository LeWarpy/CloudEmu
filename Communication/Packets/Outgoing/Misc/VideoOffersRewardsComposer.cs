namespace Cloud.Communication.Packets.Outgoing.Handshake
{
	class VideoOffersRewardsComposer : ServerPacket
    {
        public VideoOffersRewardsComposer(int Id, string Type, string Message)
            : base(ServerPacketHeader.VideoOffersRewardsMessageComposer)
        {
			WriteString(Type);
			WriteInteger(Id);
			WriteString(Message);
			WriteString("");
        }
    }
}

 