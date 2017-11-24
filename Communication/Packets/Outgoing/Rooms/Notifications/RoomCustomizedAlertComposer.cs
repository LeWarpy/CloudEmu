namespace Cloud.Communication.Packets.Outgoing.Rooms.Notifications
{
	class RoomAlertComposer : ServerPacket
    {
        public RoomAlertComposer(string Message)
            : base(ServerPacketHeader.RoomAlertComposer)

        {
			WriteInteger(1);
			WriteString(Message);
        }
    }
}