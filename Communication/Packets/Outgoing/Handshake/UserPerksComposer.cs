
using Cloud.HabboHotel.Users;

namespace Cloud.Communication.Packets.Outgoing.Handshake
{
	public class UserPerksComposer : ServerPacket
    {
        public UserPerksComposer(Habbo Habbo)
            : base(ServerPacketHeader.UserPerksMessageComposer)
        {
			WriteInteger(16); // Count
			WriteString("USE_GUIDE_TOOL");
			WriteString((Habbo.TalentStatus == "helper" && Habbo.CurrentTalentLevel >= 4) || (Habbo.Rank >= 4) ? "" : "requirement.unfulfilled.helper_level_4");
			WriteBoolean(true);
			WriteString("GIVE_GUIDE_TOURS");
			WriteString("requirement.unfulfilled.helper_le");
			WriteBoolean(true);
			WriteString("JUDGE_CHAT_REVIEWS");
			WriteString(""); // ??
			WriteBoolean(true);
			WriteString("VOTE_IN_COMPETITIONS");
			WriteString("requirement.unfulfilled.helper_level_2"); // ??
			WriteBoolean(true);
			WriteString("CALL_ON_HELPERS");
			WriteString("true"); // ??
			WriteBoolean(true);
			WriteString("CITIZEN");
			WriteString(""); // ??
			WriteBoolean(Habbo.TalentStatus == "helper" ||  Habbo.CurrentTalentLevel >= 4);
			WriteString("TRADE");
			WriteString(""); // ??
			WriteBoolean(true);
			WriteString("HEIGHTMAP_EDITOR_BETA");
			WriteString(""); // ??
			WriteBoolean(false);
			WriteString("EXPERIMENTAL_CHAT_BETA");
			WriteString("requirement.unfulfilled.helper_level_2");
			WriteBoolean(true);
			WriteString("EXPERIMENTAL_TOOLBAR");
			WriteString(""); // ??
			WriteBoolean(true);
			WriteString("BUILDER_AT_WORK");
			WriteString(""); // ??
			WriteBoolean(true);
			WriteString("NAVIGATOR_PHASE_ONE_2014");
			WriteString(""); // ??
			WriteBoolean(false);
			WriteString("CAMERA");
			WriteString(""); // ??
			WriteBoolean(true);
			WriteString("NAVIGATOR_PHASE_TWO_2014");
			WriteString(""); // ??
			WriteBoolean(true);
			WriteString("MOUSE_ZOOM");
			WriteString(""); // ??
			WriteBoolean(true);
			WriteString("NAVIGATOR_ROOM_THUMBNAIL_CAMERA");
			WriteString(""); // ??
			WriteBoolean(true);
			WriteString("HABBO_CLUB_OFFER_BETA");
			WriteString(""); // ??
			WriteBoolean(true);
        }
    }
}