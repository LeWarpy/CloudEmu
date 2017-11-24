namespace Cloud.Communication.Packets.Outgoing.Navigator
{
	class FlatAccessDeniedComposer : ServerPacket
    {
        public FlatAccessDeniedComposer(string Username)
            : base(ServerPacketHeader.FlatAccessDeniedMessageComposer)
        {
			WriteString(Username);
        }
    }
}
