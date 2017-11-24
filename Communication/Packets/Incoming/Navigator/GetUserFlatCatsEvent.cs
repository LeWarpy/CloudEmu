using System.Collections.Generic;
using Cloud.HabboHotel.Navigator;
using Cloud.HabboHotel.GameClients;
using Cloud.Communication.Packets.Outgoing.Navigator;

namespace Cloud.Communication.Packets.Incoming.Navigator
{
    public class GetUserFlatCatsEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session == null)
                return;

            ICollection<SearchResultList> Categories = CloudServer.GetGame().GetNavigator().GetFlatCategories();

            Session.SendMessage(new UserFlatCatsComposer(Categories, Session.GetHabbo().Rank));
        }
    }
}