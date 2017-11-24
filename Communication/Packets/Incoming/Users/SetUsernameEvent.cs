namespace Cloud.Communication.Packets.Incoming.Users
{
	class SetUsernameEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            string Username = Packet.PopString();
        }
    }
}
