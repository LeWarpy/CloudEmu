namespace Cloud.HabboHotel.Achievements
{
    public struct UserTalent
    {
        public int TalentId;
        public int State;

        public UserTalent(int talentId, int state)
        {
            TalentId = talentId;
            State = state;
        }
    }
}