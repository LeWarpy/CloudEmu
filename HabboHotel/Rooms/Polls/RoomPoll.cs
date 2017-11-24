using System.Linq;
using System.Collections.Generic;

namespace Cloud.HabboHotel.Rooms.Polls
{
    public class RoomPoll
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public RoomPollType Type { get; set; }
        public string Headline { get; set; }
        public string Summary { get; set; }
        public string CompletionMessage { get; set; }
        public int CreditReward { get; set; }
        public int PixelReward { get; set; }
        public string BadgeReward { get; set; }
        public double Expiry { get; set; }

        private Dictionary<int, RoomPollQuestion> _questions;
        public int LastQuestionId { get; set; }

        public RoomPoll(int id, int roomId, string type, string headline, string summary, string completionMessage, int creditReward, int pixelReward, string badgeReward, double expiry, Dictionary<int, RoomPollQuestion> questions)
        {
            this.Id = id;
            this.RoomId = roomId;
            this.Type = RoomPollTypeUtility.GetRoomPollType(type);
            this.Headline = headline;
            this.Summary = summary;
            this.CompletionMessage = completionMessage;
            this.CreditReward = creditReward;
            this.PixelReward = pixelReward;
            this.BadgeReward = badgeReward;
            this.Expiry = expiry;

            this._questions = questions.Values.OrderBy(x => x.SeriesOrder).ToDictionary(t => t.Id);
            this.LastQuestionId = this._questions.Count > 0 ? this._questions.Values.OrderByDescending(x => x.SeriesOrder).FirstOrDefault().SeriesOrder : 0;
        }

        public Dictionary<int, RoomPollQuestion> Questions
        {
            get { return this._questions; }
            set { this._questions = value; }
        }
    }
}