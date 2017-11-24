using System.Collections.Generic;

namespace Cloud.HabboHotel.Achievements
{
    public class Achievement
    {
        public int Id;
        public string Category;
        public string GroupName;
        public int GameId;
        public Dictionary<int, AchievementLevel> Levels;

        public Achievement(int Id, string GroupName, string Category, int GameId)
        {
            this.Id = Id;
            this.GroupName = GroupName;
            this.Category = Category;
            this.GameId = GameId;
			Levels = new Dictionary<int, AchievementLevel>();
        }

        public void AddLevel(AchievementLevel Level)
        {
            Levels.Add(Level.Level, Level);
        }
    }
}