using System.Collections.Generic;
using Cloud.HabboHotel.Achievements;

namespace Cloud.Communication.Packets.Outgoing.Inventory.Achievements
{
	class BadgeDefinitionsComposer: ServerPacket
    {
        public BadgeDefinitionsComposer(Dictionary<string, Achievement> Achievements)
            : base(ServerPacketHeader.BadgeDefinitionsMessageComposer)
        {
			WriteInteger(Achievements.Count);

            foreach (Achievement Achievement in Achievements.Values)
            {
				WriteString(Achievement.GroupName.Replace("ACH_", ""));
				WriteInteger(Achievement.Levels.Count);
                for (int i = 1; i < Achievement.Levels.Count + 1; i++)
                {
					WriteInteger(i);
					WriteInteger(Achievement.Levels[i].Requirement);
                }
            }
			WriteInteger(0);
        }
    }
}
