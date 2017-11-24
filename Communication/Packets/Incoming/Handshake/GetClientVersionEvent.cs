using Cloud.HabboHotel.GameClients;


namespace Cloud.Communication.Packets.Incoming.Handshake
{
    public class GetClientVersionEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            string Build = Packet.PopString();

            if (CloudServer.SWFRevision != Build)
                CloudServer.SWFRevision = Build;
        }
    }
}