using System;
using Cloud.HabboHotel.Navigator;
using Cloud.HabboHotel.Rooms;
using Cloud.HabboHotel.GameClients;

namespace Cloud.Communication.Packets.Outgoing.Navigator
{
    class GetGuestRoomResultComposer : ServerPacket
    {
        public GetGuestRoomResultComposer(GameClient Session, RoomData Data, Boolean isLoading, Boolean checkEntry)
            : base(ServerPacketHeader.GetGuestRoomResultMessageComposer)
        {
			WriteBoolean(isLoading);
			WriteInteger(Data.Id);
			WriteString(Data.Name);
			WriteInteger(Data.OwnerId);
			WriteString(Data.OwnerName);
			WriteInteger(RoomAccessUtility.GetRoomAccessPacketNum(Data.Access));
			WriteInteger(Data.UsersNow);
			WriteInteger(Data.UsersMax);
			WriteString(Data.Description);
			WriteInteger(Data.TradeSettings);
			WriteInteger(Data.Score);
			WriteInteger(0);//Top rated room rank.
			WriteInteger(Data.Category);

			WriteInteger(Data.Tags.Count);
            foreach (string Tag in Data.Tags)
            {
				WriteString(Tag);
            }

            if (Data.Group != null && Data.Promotion != null)
            {
				WriteInteger(62);//What?

				WriteInteger(Data.Group == null ? 0 : Data.Group.Id);
				WriteString(Data.Group == null ? "" : Data.Group.Name);
				WriteString(Data.Group == null ? "" : Data.Group.Badge);

				WriteString(Data.Promotion != null ? Data.Promotion.Name : "");
				WriteString(Data.Promotion != null ? Data.Promotion.Description : "");
				WriteInteger(Data.Promotion != null ? Data.Promotion.MinutesLeft : 0);
            }
            else if (Data.Group != null && Data.Promotion == null)
            {
				WriteInteger(58);//What?
				WriteInteger(Data.Group == null ? 0 : Data.Group.Id);
				WriteString(Data.Group == null ? "" : Data.Group.Name);
				WriteString(Data.Group == null ? "" : Data.Group.Badge);
            }
            else if (Data.Group == null && Data.Promotion != null)
            {
				WriteInteger(60);//What?
				WriteString(Data.Promotion != null ? Data.Promotion.Name : "");
				WriteString(Data.Promotion != null ? Data.Promotion.Description : "");
				WriteInteger(Data.Promotion != null ? Data.Promotion.MinutesLeft : 0);
            }
            else
            {
				WriteInteger(56);//What?
            }


			WriteBoolean(checkEntry);
            StaffPick staffPick = null;
            if (!CloudServer.GetGame().GetNavigator().TryGetStaffPickedRoom(Data.Id, out staffPick))
				WriteBoolean(false);
            else
				WriteBoolean(true);
			WriteBoolean(false);
			WriteBoolean(false);

			WriteInteger(Data.WhoCanMute);
			WriteInteger(Data.WhoCanKick);
			WriteInteger(Data.WhoCanBan);

			WriteBoolean(Session.GetHabbo().GetPermissions().HasRight("mod_tool") || Data.OwnerName == Session.GetHabbo().Username);//Room muting.
			WriteInteger(Data.chatMode);
			WriteInteger(Data.chatSize);
			WriteInteger(Data.chatSpeed);
			WriteInteger(Data.extraFlood);//Hearing distance
			WriteInteger(Data.chatDistance);//Flood!!
        }
    }
}