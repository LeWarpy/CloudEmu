using System;
using System.Collections.Generic;
using Cloud.HabboHotel.Users;
using Cloud.HabboHotel.Groups;
using Cloud.HabboHotel.GameClients;

namespace Cloud.Communication.Packets.Outgoing.Users
{
	class ProfileInformationComposer : ServerPacket
    {
        public ProfileInformationComposer(Habbo Data, GameClient Session, List<Group> Groups, int friendCount)
            : base(ServerPacketHeader.ProfileInformationMessageComposer)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(Data.AccountCreated);

            WriteInteger(Data.Id);
            WriteString(Data.Username);
            WriteString(Data.Look);
            WriteString(Data.Motto);
            WriteString(origin.ToString("dd/MM/yyyy"));
            WriteInteger(Data.GetStats().AchievementPoints);
            WriteInteger(friendCount); // Friend Count
            WriteBoolean(Data.Id != Session.GetHabbo().Id && Session.GetHabbo().GetMessenger().FriendshipExists(Data.Id)); //  Is friend
            WriteBoolean(Data.Id != Session.GetHabbo().Id && !Session.GetHabbo().GetMessenger().FriendshipExists(Data.Id) && Session.GetHabbo().GetMessenger().RequestExists(Data.Id)); // Sent friend request
            WriteBoolean((CloudServer.GetGame().GetClientManager().GetClientByUserID(Data.Id)) != null);

            WriteInteger(Groups.Count);
            foreach (Group Group in Groups)
            {
                WriteInteger(Group.Id);
                WriteString(Group.Name);
                WriteString(Group.Badge);
                WriteString(CloudServer.GetGame().GetGroupManager().GetColourCode(Group.Colour1, true));
                WriteString(CloudServer.GetGame().GetGroupManager().GetColourCode(Group.Colour2, false));
                WriteBoolean(Data.GetStats().FavouriteGroupId == Group.Id); // todo favs
                WriteInteger(0);//what the fuck
                WriteBoolean(Group != null ? Group.ForumEnabled : true);//HabboTalk
            }

            WriteInteger(Convert.ToInt32(CloudServer.GetUnixTimestamp() - Data.LastOnline)); // Last online
            WriteBoolean(true); // Show the profile
        }
    }
}