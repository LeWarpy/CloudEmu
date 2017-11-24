namespace Cloud.Communication.Packets.Outgoing.Users
{
	class RespectNotificationComposer : ServerPacket
    {
        public RespectNotificationComposer(int userID, int Respect)
            : base(ServerPacketHeader.RespectNotificationMessageComposer)
        {
            WriteInteger(userID);
            WriteInteger(Respect);
        }
    }
}
