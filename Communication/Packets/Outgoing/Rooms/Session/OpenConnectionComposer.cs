namespace Cloud.Communication.Packets.Outgoing.Rooms.Session
{
	class OpenConnectionComposer : ServerPacket
    {
        public OpenConnectionComposer()
            : base(ServerPacketHeader.OpenConnectionMessageComposer)
        {

        }
    }
}
