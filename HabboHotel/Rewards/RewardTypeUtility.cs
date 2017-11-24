namespace Cloud.HabboHotel.Rewards
{
    public class RewardTypeUtility
    {
        public static RewardType GetType(string Type)
        {
            switch (Type.ToLower())
            {
                case "badge":
                    return RewardType.BADGE;

                case "credits":
                    return RewardType.CREDITS;

                case "duckets":
                    return RewardType.DUCKETS;

                case "diamonds":
                    return RewardType.DIAMONDS;

                default:
                case "none":
                    return RewardType.NONE;
            }
        }
    }
}
