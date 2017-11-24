using System;
using System.Data;
using System.Collections.Generic;

using Cloud.Database.Interfaces;

namespace Cloud.HabboHotel.Rooms.Polls
{
    public class RoomPollQuestion
    {
        public int Id { get; set; }
        public int PollId { get; set; }
        public string Question { get; set; }
        public RoomPollQuestionType Type { get; set; }
        public int SeriesOrder { get; set; }
        public int MinimumSlections { get; set; }

        private Dictionary<int, RoomPollQuestionSelection> _selections;


        public RoomPollQuestion(int id, int pollId, string question, string type, int seriesOrder, int minimumSlections)
        {
            this.Id = id;
            this.PollId = pollId;
            this.Question = question;
            this.Type = RoomPollQuestionTypeUtility.GetQuestionType(type);
            this.SeriesOrder = seriesOrder;
            this.MinimumSlections = minimumSlections;

            this._selections = new Dictionary<int, RoomPollQuestionSelection>();

            DataTable GetSelections = null;
            using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `room_poll_questions_selections` WHERE `question_id` = @QuestionId");
                dbClient.AddParameter("QuestionId", this.Id);
                GetSelections = dbClient.getTable();

                if (GetSelections != null)
                {
                    foreach (DataRow Row in GetSelections.Rows)
                    {
                        if (!this._selections.ContainsKey(Convert.ToInt32(Row["id"])))
                            this._selections.Add(Convert.ToInt32(Row["id"]), new RoomPollQuestionSelection(Convert.ToInt32(Row["id"]), this.Id, Convert.ToString(Row["text"]), Convert.ToString(Row["value"])));
                    }
                }
            }
        }

        public Dictionary<int, RoomPollQuestionSelection> Selections
        {
            get { return this._selections; }
            set { this._selections = value; }
        }
    }
}