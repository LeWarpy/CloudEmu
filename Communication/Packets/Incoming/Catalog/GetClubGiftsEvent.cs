using Cloud.HabboHotel.GameClients;
using Cloud.Communication.Packets.Outgoing.Catalog;

namespace Cloud.Communication.Packets.Incoming.Catalog
{
    class GetClubGiftsEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {

            Session.SendMessage(new ClubGiftsComposer());
        }
    }
}
