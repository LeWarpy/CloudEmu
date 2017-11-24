using System.Collections.Generic;

using Cloud.HabboHotel.Groups;

namespace Cloud.Communication.Packets.Outgoing.Users
{
	class HabboGroupBadgesComposer : ServerPacket
    {
        public HabboGroupBadgesComposer(Dictionary<int, string> Badges)
            : base(ServerPacketHeader.HabboGroupBadgesMessageComposer)
        {
			WriteInteger(Badges.Count);
            foreach (KeyValuePair<int, string> Badge in Badges)
            {
				WriteInteger(Badge.Key);
				WriteString(Badge.Value);
            }
        }

        public HabboGroupBadgesComposer(Group Group)
            : base(ServerPacketHeader.HabboGroupBadgesMessageComposer)
        {
			WriteInteger(1);//count
            {
				WriteInteger(Group.Id);
				WriteString(Group.Badge);
            }
        }
    }
}
