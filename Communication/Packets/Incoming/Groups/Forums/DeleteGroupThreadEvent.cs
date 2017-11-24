using Cloud.Communication.Packets.Outgoing.Groups;
using Cloud.Communication.Packets.Outgoing.Rooms.Notifications;
using Cloud.HabboHotel.GameClients;

namespace Cloud.Communication.Packets.Incoming.Groups
{
    class DeleteGroupThreadEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            var int1 = Packet.PopInt();
            var int2 = Packet.PopInt();
            var int3 = Packet.PopInt();

            var forum = CloudServer.GetGame().GetGroupForumManager().GetForum(int1);

            if (forum == null)
            {
                Session.SendNotification("O Forum não existe!");
                return;
            }

            if (forum.Settings.GetReasonForNot(Session, forum.Settings.WhoCanModerate) != "")
            {
                Session.SendNotification("Não tem direito para eliminar esse forum!");
                return;
            }

            var thread = forum.GetThread(int2);
            if (thread == null)
            {
                Session.SendNotification("O tema não existe!");
                return;
            }

            thread.DeletedLevel = int3 / 10;

            thread.DeleterUserId = thread.DeletedLevel != 0 ? Session.GetHabbo().Id : 0;

            thread.Save();

            Session.SendMessage(new ThreadsListDataComposer(forum, Session));

            if (thread.DeletedLevel != 0)
                Session.SendMessage(new RoomNotificationComposer("forums.thread.hidden"));
            else
                Session.SendMessage(new RoomNotificationComposer("forums.thread.restored"));
        }
    }
}
