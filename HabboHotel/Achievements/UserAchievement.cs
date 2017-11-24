namespace Cloud.HabboHotel.Achievements
{
    public class UserAchievement
    {
        public readonly string AchievementGroup;
        public int Level;
        public int Progress;

        public UserAchievement(string achievementGroup, int level, int progress)
        {
			AchievementGroup = achievementGroup;
			Level = level;
			Progress = progress;
        }
    }
}