

namespace Cloud.Communication.Packets.Outgoing.Notifications
{
    class MOTDNotificationComposer : ServerPacket
    {
        public MOTDNotificationComposer(string Message)
            : base(ServerPacketHeader.MOTDNotificationMessageComposer)
        {
			WriteInteger(1);
			WriteString(Message);
        }
    }
}
