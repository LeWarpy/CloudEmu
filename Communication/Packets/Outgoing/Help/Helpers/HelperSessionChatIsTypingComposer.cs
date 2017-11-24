namespace Cloud.Communication.Packets.Outgoing.Help.Helpers
{
	class HelperSessionChatIsTypingComposer : ServerPacket
    {
        public HelperSessionChatIsTypingComposer(bool typing)
            : base(ServerPacketHeader.HelperSessionChatIsTypingMessageComposer)
        {
			WriteBoolean(typing);
        }
    }
}
