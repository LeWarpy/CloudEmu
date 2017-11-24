using Cloud.HabboHotel.Rooms.Polls;

namespace Cloud.Communication.Packets.Outgoing.Rooms.Polls
{
    class PollContentsComposer : ServerPacket
    {
        public PollContentsComposer(RoomPoll poll)
            : base(ServerPacketHeader.PollContentsMessageComposer)
        {
			WriteInteger(poll.Id);
			WriteString(poll.Headline);
			WriteString(poll.CompletionMessage);

			WriteInteger(poll.Questions.Count);
            foreach (RoomPollQuestion question in poll.Questions.Values)
            {
				WriteInteger(question.Id);
				WriteInteger(question.SeriesOrder);
				WriteInteger(RoomPollQuestionTypeUtility.GetQuestionType(question.Type));
				WriteString(question.Question);

				WriteInteger(0); // ??
				WriteInteger(question.MinimumSlections);// Min selections

				WriteInteger(question.Selections.Count);
                foreach (RoomPollQuestionSelection Selection in question.Selections.Values)
                {
					WriteString(Selection.Value);
					WriteString(Selection.Text);
					WriteInteger(Selection.Id);
                }

				WriteInteger(0);//??
            }
			WriteBoolean(true);//No idea
        }
    }
}