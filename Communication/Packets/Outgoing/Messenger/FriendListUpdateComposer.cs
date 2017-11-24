using System;
using System.Linq;

using Cloud.HabboHotel.GameClients;
using Cloud.HabboHotel.Users.Relationships;
using Cloud.HabboHotel.Users.Messenger;
using Cloud.HabboHotel.Groups;

namespace Cloud.Communication.Packets.Outgoing.Messenger
{
	class FriendListUpdateComposer : ServerPacket
    {
        public FriendListUpdateComposer(int FriendId)
            : base(ServerPacketHeader.FriendListUpdateMessageComposer)
        {
			WriteInteger(1);//Category Count
			WriteInteger(1);
			WriteString("Grupos");
			WriteInteger(1);//Updates Count
			WriteInteger(-1);//Update
			WriteInteger(FriendId);
        }

        public FriendListUpdateComposer(Group Group, int State)
           : base(ServerPacketHeader.FriendListUpdateMessageComposer)
        {
			WriteInteger(1);//Category Count
			WriteInteger(1);
			WriteString("Grupos");
			WriteInteger(1);//Updates Count

			WriteInteger(State);//Update
			WriteInteger(-Group.Id);
			WriteString(Group.Name);
			WriteInteger(0);

			WriteBoolean(true);
			WriteBoolean(false);

			WriteString(Group.Badge);//Habbo.IsOnline ? Habbo.Look : "");
			WriteInteger(1); // categoryid
			WriteString("");
			WriteString(string.Empty); // Facebook username
			WriteString(string.Empty);
			WriteBoolean(true); // Allows offline messaging
			WriteBoolean(false); // ?
			WriteBoolean(false); // Uses phone
			WriteShort(0);
        }

        public FriendListUpdateComposer(GameClient Session, MessengerBuddy Buddy)
            : base(ServerPacketHeader.FriendListUpdateMessageComposer)
        {
			WriteInteger(1);//Category Count
			WriteInteger(1);
			WriteString("Grupos");
			WriteInteger(1);//Updates Count
			WriteInteger(0);//Update

            Relationship Relationship = Session.GetHabbo().Relationships.FirstOrDefault(x => x.Value.UserId == Convert.ToInt32(Buddy.UserId)).Value;
            int y = Relationship == null ? 0 : Relationship.Type;

			WriteInteger(Buddy.UserId);
			WriteString(Buddy.mUsername);
			WriteInteger(1);
            if (!Buddy.mAppearOffline || Session.GetHabbo().GetPermissions().HasRight("mod_tool"))
				WriteBoolean(Buddy.IsOnline);
            else
				WriteBoolean(false);

            if (!Buddy.mHideInroom || Session.GetHabbo().GetPermissions().HasRight("mod_tool"))
				WriteBoolean(Buddy.InRoom);
            else
				WriteBoolean(false);

			WriteString(Buddy.mLook);//Habbo.IsOnline ? Habbo.Look : "");
			WriteInteger(0); // categoryid
			WriteString(Buddy.mMotto);
			WriteString(string.Empty); // Facebook username
			WriteString(string.Empty);
			WriteBoolean(true); // Allows offline messaging
			WriteBoolean(false); // ?
			WriteBoolean(false); // Uses phone
			WriteShort(y);
        }
    }
}