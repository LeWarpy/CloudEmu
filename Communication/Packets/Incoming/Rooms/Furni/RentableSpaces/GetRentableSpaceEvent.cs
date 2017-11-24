using Cloud.Communication.Packets.Outgoing.Rooms.Furni.RentableSpaces;

namespace Cloud.Communication.Packets.Incoming.Rooms.Furni.RentableSpaces
{
	class GetRentableSpaceEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            int Something = Packet.PopInt();
            Session.SendMessage(new RentableSpaceComposer());
        }
    }
}
