using Cloud.HabboHotel.Groups;
using Cloud.Communication.Packets.Outgoing.Groups;

namespace Cloud.Communication.Packets.Incoming.Groups
{
    class GetGroupInfoEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            int GroupId = Packet.PopInt();
            bool NewWindow = Packet.PopBoolean();

            Group Group = null;
            if (!CloudServer.GetGame().GetGroupManager().TryGetGroup(GroupId, out Group))
                return;

            Session.SendMessage(new GroupInfoComposer(Group, Session, NewWindow));     
        }
    }
}
