

namespace Cloud.Communication.Packets.Outgoing.Notifications
{
    class NewRoomNotifComposer : ServerPacket
    {
        public NewRoomNotifComposer(string Type, string Key, string Value)
            : base(ServerPacketHeader.RoomNotificationMessageComposer)
        {
			WriteString(Type);
			WriteInteger(1);//Count
			WriteString(Key);//Type of message
			WriteString(Value);
        }
    }
}
