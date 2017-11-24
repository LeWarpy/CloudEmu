using System.Collections.Generic;

using Cloud.HabboHotel.GameClients;
using Cloud.HabboHotel.Achievements;

namespace Cloud.Communication.Packets.Outgoing.Inventory.Achievements
{
	class AchievementsComposer : ServerPacket
    {
        public AchievementsComposer(GameClient Session, List<Achievement> Achievements)
            : base(ServerPacketHeader.AchievementsMessageComposer)
        {
			WriteInteger(Achievements.Count);
            foreach (Achievement Achievement in Achievements)
            {
                UserAchievement UserData = Session.GetHabbo().GetAchievementData(Achievement.GroupName);
                int TargetLevel = (UserData != null ? UserData.Level + 1 : 1);
                int TotalLevels = Achievement.Levels.Count;

                TargetLevel = (TargetLevel > TotalLevels ? TotalLevels : TargetLevel);
                int i = UserData != null ? (UserData.Level + 1) : 1;

                int count = Achievement.Levels.Count;
                if (i > count)
                    i = count;
                AchievementLevel TargetLevelData = Achievement.Levels[TargetLevel];
                AchievementLevel achievementLevel = Achievement.Levels[i];
                AchievementLevel oldLevel = (Achievement.Levels.ContainsKey(i - 1)) ? Achievement.Levels[i - 1] : achievementLevel;
				WriteInteger(Achievement.Id); // Unknown (ID?)
				WriteInteger(i); // Target level
				WriteString(string.Format("{0}{1}", Achievement.GroupName, i)); // Target name/desc/badge
				WriteInteger(oldLevel.Requirement);
				WriteInteger(TargetLevelData.Requirement); // Progress req/target          
				WriteInteger(TargetLevelData.RewardPixels);
				WriteInteger(0); // Type of reward
				WriteInteger(UserData != null ? UserData.Progress : 0); // Current progress
                if (UserData == null)
					WriteBoolean(false);
                else if (UserData.Level >= TotalLevels)
					WriteBoolean(true);
                else
					WriteBoolean(false);
				WriteString(Achievement.Category); // Category
				WriteString(string.Empty);
				WriteInteger(count); // Total amount of levels 
				WriteInteger(0);
            }
			WriteString("");
        }
    }
}