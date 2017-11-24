using Cloud.HabboHotel.GameClients;
using Cloud.HabboHotel.Groups.Forums;
using System.Collections.Generic;

namespace Cloud.Communication.Packets.Outgoing.Groups
{
	class ForumsListDataComposer : ServerPacket
    {
        public ForumsListDataComposer(ICollection<GroupForum> Forums, GameClient Session, int ViewOrder = 0, int StartIndex = 0, int MaxLength = 20)
            : base(ServerPacketHeader.GroupForumListingsMessageComposer)
        {
			WriteInteger(ViewOrder);
			WriteInteger(StartIndex);
			WriteInteger(StartIndex);

			WriteInteger(Forums.Count); // Forum List Count

            foreach (var Forum in Forums)
            {
                var lastpost = Forum.GetLastPost();
                var isn = lastpost == null;
				WriteInteger(Forum.Id); //Maybe ID
				WriteString(Forum.Name); //Forum name
				WriteString(Forum.Description); //idk
				WriteString(Forum.Group.Badge); // Group Badge
				WriteInteger(0);//Idk
				WriteInteger(0);// Score
				WriteInteger(Forum.MessagesCount);//Message count
				WriteInteger(Forum.UnreadMessages(Session.GetHabbo().Id));//unread message count
				WriteInteger(0);//Idk
				WriteInteger(!isn ? lastpost.GetAuthor().Id : 0);// Las user to message id
				WriteString(!isn ? lastpost.GetAuthor().Username : ""); //Last user to message name
				WriteInteger(!isn ? (int)CloudServer.GetUnixTimestamp() - lastpost.Timestamp : 0); //Last message timestamp
            }
        }
    }
}
