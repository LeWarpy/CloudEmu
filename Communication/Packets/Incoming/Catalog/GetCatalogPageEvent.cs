using Cloud.Communication.Packets.Outgoing.Catalog;
using Cloud.HabboHotel.Catalog;
using Cloud.HabboHotel.GameClients;

namespace Cloud.Communication.Packets.Incoming.Catalog
{
	public class GetCatalogPageEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            int PageId = Packet.PopInt();
            int Something = Packet.PopInt();
            string CataMode = Packet.PopString();

            CatalogPage Page = null;
            if (!CloudServer.GetGame().GetCatalog().TryGetPage(PageId, out Page))
                return;

            if (!Page.Enabled || !Page.Visible || Page.MinimumRank  >  Session.GetHabbo().Rank || (Page.MinimumVIP > Session.GetHabbo().VIPRank && Session.GetHabbo().Rank == 1))
                return;

            Session.GetHabbo().lastLayout = Page.Template;

            Session.SendMessage(new CatalogPageComposer(Page, CataMode));
        }
    }
}