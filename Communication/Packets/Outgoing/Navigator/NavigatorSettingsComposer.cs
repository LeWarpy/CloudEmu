namespace Cloud.Communication.Packets.Outgoing.Navigator
{
	class NavigatorSettingsComposer : ServerPacket
    {
        public NavigatorSettingsComposer(int Homeroom)
            : base(ServerPacketHeader.NavigatorSettingsMessageComposer)
        {
			WriteInteger(Homeroom);
			WriteInteger(Homeroom);
        }
    }
}
