using Cloud.Communication.Packets.Outgoing.Groups;
using Cloud.HabboHotel.GameClients;

namespace Cloud.Communication.Packets.Incoming.Groups
{
    class PostGroupContentEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            var ForumId = Packet.PopInt();
            var ThreadId = Packet.PopInt();
            var Caption = Packet.PopString();
            var Message = Packet.PopString();

            var Forum = CloudServer.GetGame().GetGroupForumManager().GetForum(ForumId);
            if (Forum == null)
            {
                Session.SendNotification("Ops! Este forum não existe!");
                return;
            }

            var IsNewThread = ThreadId == 0;
            if (IsNewThread)
            {
                var Thread = Forum.CreateThread(Session.GetHabbo().Id, Caption);
                var Post = Thread.CreatePost(Session.GetHabbo().Id, Message);

                Session.SendMessage(new ThreadCreatedComposer(Session, Thread));
                //Session.SendMessage(new PostUpdatedComposer(Session, Post));
                //Session.SendMessage(new ThreadReplyComposer(Session, Post));

            }
            else
            {
                var Thread = Forum.GetThread(ThreadId);
                if (Thread == null)
                {
                    Session.SendNotification("Ops! O tópico desse forum não existe!");
                    return;
                }

                var Post = Thread.CreatePost(Session.GetHabbo().Id, Message);
                Session.SendMessage(new ThreadReplyComposer(Session, Post));
            }


        }
    }
}
