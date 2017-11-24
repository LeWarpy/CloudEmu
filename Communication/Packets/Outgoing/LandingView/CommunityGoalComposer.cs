namespace Cloud.Communication.Packets.Outgoing.LandingView
{
	class CommunityGoalComposer : ServerPacket
    {
        public CommunityGoalComposer()
            : base(ServerPacketHeader.CommunityGoalComposer)
        {
			WriteBoolean(true); //Achieved?
			WriteInteger(0); //User Amount
			WriteInteger(1); //User Rank
			WriteInteger(2); //Total Amount
			WriteInteger(3); //Community Highest Achieved
			WriteInteger(4); //Community Score Untill Next Level
			WriteInteger(5); //Percent Completed Till Next Level
			WriteString("WORLDCUP01");
			WriteInteger(6); //Timer
			WriteInteger(1); //Rank Count
			WriteInteger(1); //Rank level
        }
    }
}
