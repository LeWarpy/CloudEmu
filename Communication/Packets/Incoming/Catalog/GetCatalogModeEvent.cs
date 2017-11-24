namespace Cloud.Communication.Packets.Incoming.Catalog
{
	class GetCatalogModeEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            string PageMode = Packet.PopString();
        }
    }
}
