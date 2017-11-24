namespace Cloud.Communication.Packets.Outgoing.Rooms.Session
{
	class CloseConnectionComposer : ServerPacket
    {
        public CloseConnectionComposer()
            : base(ServerPacketHeader.CloseConnectionMessageComposer)
        {

        }
    }
}
