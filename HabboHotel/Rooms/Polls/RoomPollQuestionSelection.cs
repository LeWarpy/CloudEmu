namespace Cloud.HabboHotel.Rooms.Polls
{
    public class RoomPollQuestionSelection
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public string Text { get; set; }
        public string Value { get; set; }

        public RoomPollQuestionSelection(int id, int questionId, string text, string value)
        {
            this.Id = id;
            this.QuestionId = questionId;
            this.Text = text;
            this.Value = value;
        }
    }
}