using Cloud.Communication.Packets.Outgoing.Misc;

namespace Cloud.Communication.Packets.Incoming.Misc
{
    class LatencyTestEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            Session.SendMessage(new LatencyTestComposer(Session, Packet.PopInt()));
        }
    }
}
