
using Cloud.HabboHotel.GameClients;


namespace Cloud.Communication.Packets.Incoming.Handshake
{
    public class SSOTicketEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.RC4Client == null || Session.GetHabbo() != null)
                return;

            Session.TryAuthenticate(Packet.PopString());
        }
    }
}