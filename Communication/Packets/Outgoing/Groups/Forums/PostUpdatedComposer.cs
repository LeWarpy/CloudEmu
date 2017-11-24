using Cloud.HabboHotel.GameClients;
using Cloud.HabboHotel.Groups.Forums;

namespace Cloud.Communication.Packets.Outgoing.Groups
{
	class PostUpdatedComposer : ServerPacket
    {
        public PostUpdatedComposer(GameClient Session, GroupForumThreadPost Post)
            : base(ServerPacketHeader.PostUpdatedMessageComposer)
        {
			WriteInteger(Post.ParentThread.ParentForum.Id);
			WriteInteger(Post.ParentThread.Id);

            Post.SerializeData(this);
        }
    }
}
