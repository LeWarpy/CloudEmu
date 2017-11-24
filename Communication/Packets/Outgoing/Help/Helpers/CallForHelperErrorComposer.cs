namespace Cloud.Communication.Packets.Outgoing.Help.Helpers
{
    class CallForHelperErrorComposer : ServerPacket
    {
        public CallForHelperErrorComposer(int errorCode)
            : base(ServerPacketHeader.CallForHelperErrorMessageComposer)
        {
			WriteInteger(errorCode);
        }
    }
}
