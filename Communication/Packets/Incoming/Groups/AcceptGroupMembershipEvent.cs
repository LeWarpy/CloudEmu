
using Cloud.HabboHotel.Groups;
using Cloud.Communication.Packets.Outgoing.Groups;

using Cloud.Communication.Packets.Outgoing.Messenger;
using Cloud.HabboHotel.Users;

namespace Cloud.Communication.Packets.Incoming.Groups
{
    class AcceptGroupMembershipEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            int GroupId = Packet.PopInt();
            int UserId = Packet.PopInt();

            Group Group = null;
            if (!CloudServer.GetGame().GetGroupManager().TryGetGroup(GroupId, out Group))
                return;

            if ((Session.GetHabbo().Id != Group.CreatorId && !Group.IsAdmin(Session.GetHabbo().Id)) && !Session.GetHabbo().GetPermissions().HasRight("fuse_group_accept_any"))
                return;

            if (!Group.HasRequest(UserId))
                return;

            Habbo Habbo = CloudServer.GetHabboById(UserId);
            if (Habbo == null)
            {
                Session.SendNotification("Ocorreu um erro ao procurar esse usuário!");
                return;
            }

            Group.HandleRequest(UserId, true);

            if (Group.HasChat)
            {
                var Client = CloudServer.GetGame().GetClientManager().GetClientByUserID(UserId);
                if (Client != null)
                {
                    Client.SendMessage(new FriendListUpdateComposer(Group, 1));
                }
            }

            Session.SendMessage(new GroupMemberUpdatedComposer(GroupId, Habbo, 4));
        }
    }
}