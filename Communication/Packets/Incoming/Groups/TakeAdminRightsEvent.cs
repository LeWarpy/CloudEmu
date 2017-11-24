
using Cloud.HabboHotel.Users;
using Cloud.HabboHotel.Rooms;
using Cloud.HabboHotel.Groups;
using Cloud.Communication.Packets.Outgoing.Groups;
using Cloud.Communication.Packets.Outgoing.Rooms.Permissions;



namespace Cloud.Communication.Packets.Incoming.Groups
{
    class TakeAdminRightsEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            int GroupId = Packet.PopInt();
            int UserId = Packet.PopInt();

            Group Group = null;
            if (!CloudServer.GetGame().GetGroupManager().TryGetGroup(GroupId, out Group))
                return;

            if (Session.GetHabbo().Id != Group.CreatorId || !Group.IsMember(UserId))
                return;

            Habbo Habbo = CloudServer.GetHabboById(UserId);
            if (Habbo == null)
            {
                Session.SendNotification("Ops! Ocorreu um erro ao encontrar esse usuário.");
                return;
            }

            Group.TakeAdmin(UserId);

            Room Room = null;
            if (CloudServer.GetGame().GetRoomManager().TryGetRoom(Group.RoomId, out Room))
            {
                RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(UserId);
                if (User != null)
                {
                    if (User.Statusses.ContainsKey("flatctrl 3"))
                        User.RemoveStatus("flatctrl 3");
                    User.UpdateNeeded = true;
                    if (User.GetClient() != null)
                        User.GetClient().SendMessage(new YouAreControllerComposer(0));
                }
            }

            Session.SendMessage(new GroupMemberUpdatedComposer(GroupId, Habbo, 2));
        }
    }
}
