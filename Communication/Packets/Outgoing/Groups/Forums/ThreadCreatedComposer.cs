using Cloud.HabboHotel.GameClients;
using Cloud.HabboHotel.Groups.Forums;

namespace Cloud.Communication.Packets.Outgoing.Groups
{
	public class ThreadCreatedComposer : ServerPacket
    {
        public ThreadCreatedComposer(GameClient Session, GroupForumThread Thread)
            : base(ServerPacketHeader.ThreadCreatedMessageComposer)
        {

			WriteInteger(Thread.ParentForum.Id); //Thread ID
			WriteInteger(Thread.Id); //Thread ID
			WriteInteger(Thread.GetAuthor().Id);
			WriteString(Thread.GetAuthor().Username); //Thread Author
			WriteString(Thread.Caption); //Thread Title
			WriteBoolean(false); //Pinned
			WriteBoolean(false); //Locked
			WriteInteger((int)(CloudServer.GetUnixTimestamp() - Thread.Timestamp)); //Created Secs Ago
			WriteInteger(Thread.Posts.Count); //Message count
			WriteInteger(Thread.GetUnreadMessages(Session.GetHabbo().Id)); //Unread message count
			WriteInteger(0); // idk
			WriteInteger(0); // idk

			WriteString("Unknown");// Last User Post Username
			WriteInteger(65); // Last User Post time ago [Sec]

			WriteByte(0); //idk
			WriteInteger(10);// idk
			WriteString("Str4"); //idk
			WriteInteger(11);//idk
        }
    }
}
