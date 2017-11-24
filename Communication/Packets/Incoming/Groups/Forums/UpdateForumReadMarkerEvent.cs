using Cloud.HabboHotel.GameClients;

namespace Cloud.Communication.Packets.Incoming.Groups
{
    class UpdateForumReadMarkerEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            var length = Packet.PopInt();
            for (var i = 0; i < length; i++)
            {
                var forumid = Packet.PopInt(); //Forum ID
                var postid = Packet.PopInt(); //Post ID
                var readall = Packet.PopBoolean(); //Make all read

                var forum = CloudServer.GetGame().GetGroupForumManager().GetForum(forumid);
                if (forum == null)
                    continue;

                var post = forum.GetPost(postid);
                if (post == null)
                    continue;

                var thread = post.ParentThread;
                var index = thread.Posts.IndexOf(post);
                thread.AddView(Session.GetHabbo().Id, index + 1);

            }
            //Thread.AddView(Session.GetHabbo().Id);
        }
    }
}
