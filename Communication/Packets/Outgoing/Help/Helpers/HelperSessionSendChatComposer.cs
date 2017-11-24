namespace Cloud.Communication.Packets.Outgoing.Help.Helpers
{
	class HelperSessionSendChatComposer : ServerPacket
    {
        public HelperSessionSendChatComposer(int senderId, string message)
            : base(ServerPacketHeader.HelperSessionSendChatMessageComposer)
        {
			WriteString(message);
			WriteInteger(senderId);
        }
    }
}
