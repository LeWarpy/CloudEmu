namespace Cloud.Communication.Packets.Outgoing.Navigator
{
    class NavigatorPreferencesComposer : ServerPacket
    {
        public NavigatorPreferencesComposer()
            : base(ServerPacketHeader.NavigatorPreferencesMessageComposer)
        {
			WriteInteger(68);
			WriteInteger(42);
			WriteInteger(425);//Width
			WriteInteger(592);//Height
			WriteBoolean(false);
			WriteInteger(0);
        }
    }
}

