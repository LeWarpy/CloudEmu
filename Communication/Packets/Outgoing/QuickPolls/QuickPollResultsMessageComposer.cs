namespace Cloud.Communication.Packets.Outgoing.Rooms.Poll
{
	class QuickPollResultsMessageComposer : ServerPacket
    {
        public QuickPollResultsMessageComposer(int yesVotesCount, int noVotesCount)
            : base(ServerPacketHeader.QuickPollResultsMessageComposer)
        {
			WriteInteger(-1);
			WriteInteger(2);
			WriteString("1");
			WriteInteger(yesVotesCount);

			WriteString("0");
			WriteInteger(noVotesCount);
        }
    }
}