

namespace Cloud.Communication.Packets.Outgoing.Rooms.Notifications
{
    class AlertNotificationHCMessageComposer : ServerPacket
    {
        public AlertNotificationHCMessageComposer(int type)
            : base(ServerPacketHeader.AlertNotificationHCMessageComposer)
        {
			WriteInteger(type);
        }
    }
}
