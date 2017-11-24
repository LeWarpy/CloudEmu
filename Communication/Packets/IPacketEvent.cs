using Cloud.Communication.Packets.Incoming;
using Cloud.HabboHotel.GameClients;

namespace Cloud.Communication.Packets
{
    public interface IPacketEvent
    {
        void Parse(GameClient Session, ClientPacket Packet);
    }
}