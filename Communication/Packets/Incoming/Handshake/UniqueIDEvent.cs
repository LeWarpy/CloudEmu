
using Cloud.HabboHotel.GameClients;
using Cloud.Communication.Packets.Outgoing.Handshake;

namespace Cloud.Communication.Packets.Incoming.Handshake
{
    public class UniqueIDEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            string Junk = Packet.PopString();
            string MachineId = Packet.PopString();

            Session.MachineId = MachineId;

            Session.SendMessage(new SetUniqueIdComposer(MachineId));
        }
    }
}