using Cloud.HabboHotel.GameClients;
using Cloud.Communication.Packets.Outgoing.Help;

namespace Cloud.Communication.Packets.Incoming.Help
{
    class GetSanctionStatusEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            Session.SendMessage(new SanctionStatusComposer());
        }
    }
}
