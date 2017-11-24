using System.Collections.Generic;
using Cloud.HabboHotel.LandingView.Promotions;
using Cloud.Communication.Packets.Outgoing.LandingView;

namespace Cloud.Communication.Packets.Incoming.LandingView
{
    class GetPromoArticlesEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            ICollection<Promotion> LandingPromotions = CloudServer.GetGame().GetLandingManager().GetPromotionItems();
            Session.SendMessage(new PromoArticlesComposer(LandingPromotions));
        }
    }
}
