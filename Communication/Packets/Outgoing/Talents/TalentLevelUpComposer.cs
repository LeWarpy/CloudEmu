using Cloud.HabboHotel.Achievements;

namespace Cloud.Communication.Packets.Outgoing.Talents
{
    class TalentLevelUpComposer : ServerPacket
    {
        public TalentLevelUpComposer(Talent talent)
            : base(ServerPacketHeader.TalentLevelUpMessageComposer)
        {
			WriteString(talent.Type);
			WriteInteger(talent.Level);
			WriteInteger(0);

            if (talent.Type == "citizenship" && talent.Level == 4)
            {
				WriteInteger(2);
				WriteString("HABBO_CLUB_VIP_7_DAYS");
				WriteInteger(7);
				WriteString(talent.Prize);
				WriteInteger(0);
            }
            else
            {
				WriteInteger(1);
				WriteString(talent.Prize);
				WriteInteger(0);
            }
        }
    }
}
