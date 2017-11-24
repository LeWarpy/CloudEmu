using Cloud.HabboHotel.GameClients;

namespace Cloud.Communication.Packets.Incoming.Groups
{
    public class UpdateForumThreadStatusEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            var ForumID = Packet.PopInt();
            var ThreadID = Packet.PopInt();
            var Pinned = Packet.PopBoolean();
            var Locked = Packet.PopBoolean();


            var forum = CloudServer.GetGame().GetGroupForumManager().GetForum(ForumID);
            var thread = forum.GetThread(ThreadID);

            if (forum.Settings.GetReasonForNot(Session, forum.Settings.WhoCanModerate) != "")
            {
               // Session.SendNotification(LanguageLocale.Value("forums.thread.update.error.rights"));
                return;
            }

            bool isPining = thread.Pinned != Pinned,
                isLocking = thread.Locked != Locked;

            thread.Pinned = Pinned;
            thread.Locked = Locked;

            thread.Save();

            Session.SendMessage(new Outgoing.Groups.ThreadUpdatedComposer(Session, thread));

            if (isPining)
                if (Pinned)
                    Session.SendMessage(new Outgoing.Rooms.Notifications.RoomNotificationComposer("forums.thread.pinned"));
                else
                    Session.SendMessage(new Outgoing.Rooms.Notifications.RoomNotificationComposer("forums.thread.unpinned"));

            if (isLocking)
                if (Locked)
                    Session.SendMessage(new Outgoing.Rooms.Notifications.RoomNotificationComposer("forums.thread.locked"));
                else
                    Session.SendMessage(new Outgoing.Rooms.Notifications.RoomNotificationComposer("forums.thread.unlocked"));

        }
    }
}
