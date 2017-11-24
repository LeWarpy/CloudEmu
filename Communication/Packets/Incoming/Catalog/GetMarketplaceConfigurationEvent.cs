using Cloud.Communication.Packets.Outgoing.Catalog;
using Cloud.HabboHotel.GameClients;

namespace Cloud.Communication.Packets.Incoming.Catalog
{
    public class GetMarketplaceConfigurationEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            Session.SendMessage(new MarketplaceConfigurationComposer());
        }
    }
}