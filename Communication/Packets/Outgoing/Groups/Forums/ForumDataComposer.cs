using Cloud.HabboHotel.GameClients;
using Cloud.HabboHotel.Groups.Forums;

namespace Cloud.Communication.Packets.Outgoing.Groups
{
	class ForumDataComposer : ServerPacket
    {
        public ForumDataComposer(GroupForum Forum, GameClient Session)
            : base(ServerPacketHeader.GroupForumDataMessageComposer)
        {
			WriteInteger(Forum.Id);
			WriteString(Forum.Group.Name); //Group Name
			WriteString(Forum.Group.Description); // idk
			WriteString(Forum.Group.Badge); //Group Badge code

			WriteInteger(Forum.Threads.Count); //Forum Thread Count
			WriteInteger(0); //Last Author ID
			WriteInteger(0); //Score ?
			WriteInteger(0); //Last Thread Mark
			WriteInteger(0);
			WriteInteger(0);
			WriteString("not_member");
			WriteInteger(0);
			//end fillFromMEssage func

			WriteInteger(Forum.Settings.WhoCanRead); //Who can read 
			WriteInteger(Forum.Settings.WhoCanPost); // Who can post
			WriteInteger(Forum.Settings.WhoCanInitDiscussions); //Who can make threads
			WriteInteger(Forum.Settings.WhoCanModerate); //Who can MOD

			//Permissions i think
			WriteString(Forum.Settings.GetReasonForNot(Session, Forum.Settings.WhoCanRead)); //Forum disabled reason//base.WriteString(Forum.Settings.GetReasonForNot(Session, Forum.Settings.WhoCanRead)); 
			WriteString(Forum.Settings.GetReasonForNot(Session, Forum.Settings.WhoCanPost)); //Can't reply reason
			WriteString(Forum.Settings.GetReasonForNot(Session, Forum.Settings.WhoCanInitDiscussions));// Can't Post reason
			WriteString(Forum.Settings.GetReasonForNot(Session, Forum.Settings.WhoCanModerate)); //Can't moderate thread posts reason
			WriteString("-System");

			WriteBoolean(Forum.Group.CreatorId == Session.GetHabbo().Id); // Is Owner
			WriteBoolean(Forum.Group.IsAdmin(Session.GetHabbo().Id) && Forum.Settings.GetReasonForNot(Session, Forum.Settings.WhoCanModerate) == ""); // Is admin

        }
    }
}
