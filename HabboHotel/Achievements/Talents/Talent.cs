using System;

namespace Cloud.HabboHotel.Achievements
{
    public class Talent
    {
        public int Id;
        public string Type;
        public int ParentCategory;
        public int Level;
        public string AchievementGroup;
        public int AchievementLevel;
        public string Prize;
        public int PrizeBaseItem;

        public Talent(int id, string type, int parentCategory, int level, string achId, int achLevel, string prize,
            int prizeBaseItem)
        {
            Id = id;
            Type = type;
            ParentCategory = parentCategory;
            Level = level;
            AchievementGroup = achId;
            AchievementLevel = achLevel;
            Prize = prize;
            PrizeBaseItem = prizeBaseItem;
        }
        public Achievement GetAchievement()
        {
            if (string.IsNullOrEmpty(AchievementGroup) || ParentCategory == -1)
                return null;

            return CloudServer.GetGame().GetAchievementManager().GetAchievement(AchievementGroup);
        }
    }
}