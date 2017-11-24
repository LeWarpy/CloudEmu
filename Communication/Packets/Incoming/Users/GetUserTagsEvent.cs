
using Cloud.Communication.Packets.Outgoing.Users;

namespace Cloud.Communication.Packets.Incoming.Users
{
	class GetUserTagsEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            int UserId = Packet.PopInt();

            Session.SendMessage(new UserTagsComposer(Session, UserId));
        }
    }
}
