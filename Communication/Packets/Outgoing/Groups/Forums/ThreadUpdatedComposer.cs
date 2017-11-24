using Cloud.HabboHotel.GameClients;
using Cloud.HabboHotel.Groups.Forums;

namespace Cloud.Communication.Packets.Outgoing.Groups
{
	class ThreadUpdatedComposer : ServerPacket
    {
        public ThreadUpdatedComposer(GameClient Session, GroupForumThread Thread)
            : base(ServerPacketHeader.ThreadUpdatedMessageComposer)
        {
			WriteInteger(Thread.ParentForum.Id);

            Thread.SerializeData(Session, this);
        }
    }
}
