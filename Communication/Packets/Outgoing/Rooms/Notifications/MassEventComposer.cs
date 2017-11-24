namespace Cloud.Communication.Packets.Outgoing.Rooms.Notifications
{
	class MassEventComposer : ServerPacket
    {
        public MassEventComposer(string Message)
            : base(ServerPacketHeader.MassEventComposer)

        {
			WriteString(Message);
        }
    }
}