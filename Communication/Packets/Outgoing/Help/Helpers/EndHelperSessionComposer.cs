namespace Cloud.Communication.Packets.Outgoing.Help.Helpers
{
	class EndHelperSessionComposer : ServerPacket
    {
        public EndHelperSessionComposer(int closeCode = 0)
            : base(ServerPacketHeader.EndHelperSessionMessageComposer)
        {
			WriteInteger(closeCode);
        }
    }
}
