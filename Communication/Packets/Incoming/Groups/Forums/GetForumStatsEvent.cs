using Cloud.Communication.Packets.Outgoing.Groups;
using Cloud.Communication.Packets.Outgoing.Rooms.Notifications;
using Cloud.HabboHotel.GameClients;
using Cloud.HabboHotel.Groups.Forums;

namespace Cloud.Communication.Packets.Incoming.Groups
{
    class GetForumStatsEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            var GroupForumId = Packet.PopInt();

            GroupForum Forum;
            if (!CloudServer.GetGame().GetGroupForumManager().TryGetForum(GroupForumId, out Forum))
            {
                CloudServer.GetGame().GetClientManager().SendMessage(RoomNotificationComposer.SendBubble("forums_thread_hidden", "O fórum que você está tentando acessar não existe mais.", ""));
                return;
            }

            Session.SendMessage(new ForumDataComposer(Forum, Session));

        }
    }
}
