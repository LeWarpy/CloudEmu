using System;

namespace Cloud.Communication.Packets.Outgoing.Rooms.Poll
{
	class QuickPollResultMessageComposer : ServerPacket
    {
        public QuickPollResultMessageComposer(int UserId, String myVote, int yesVotesCount, int noVotesCount)
            : base(ServerPacketHeader.QuickPollResultMessageComposer)
        {
			WriteInteger(UserId);
			WriteString(myVote);
			WriteInteger(2);
			WriteString("1");
			WriteInteger(yesVotesCount);

			WriteString("0");
			WriteInteger(noVotesCount);
        }
    }
}