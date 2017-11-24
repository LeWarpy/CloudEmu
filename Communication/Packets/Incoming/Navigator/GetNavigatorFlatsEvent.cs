using System.Collections.Generic;
using Cloud.HabboHotel.GameClients;
using Cloud.Communication.Packets.Outgoing.Navigator;
using Cloud.HabboHotel.Navigator;

namespace Cloud.Communication.Packets.Incoming.Navigator
{
    class GetNavigatorFlatsEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            ICollection<SearchResultList> Categories = CloudServer.GetGame().GetNavigator().GetEventCategories();

            Session.SendMessage(new NavigatorFlatCatsComposer(Categories, Session.GetHabbo().Rank));
        }
    }
}