using Cloud.HabboHotel.GameClients;
using Cloud.Communication.Packets.Outgoing.Catalog;
using Cloud.Communication.Packets.Outgoing.BuildersClub;

namespace Cloud.Communication.Packets.Incoming.Catalog
{
    public class GetCatalogIndexEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            /*int Sub = 0;

            if (Session.GetHabbo().GetSubscriptionManager().HasSubscription)
            {
                Sub = Session.GetHabbo().GetSubscriptionManager().GetSubscription().SubscriptionId;
            }*/

            Session.SendMessage(new CatalogIndexComposer(Session, CloudServer.GetGame().GetCatalog().GetPages()));//, Sub));
            Session.SendMessage(new CatalogItemDiscountComposer());
            Session.SendMessage(new BCBorrowedItemsComposer());
        }
    }
}