using System;
using System.Linq;
using System.Data;
using System.Collections.Generic;

using log4net;
using Cloud.Database.Interfaces;

namespace Cloud.HabboHotel.Rooms.Polls
{
    public class PollManager
    {
        private static readonly ILog log = LogManager.GetLogger("Cloud.HabboHotel.Polls.PollManager");

        private readonly Dictionary<int, RoomPoll> _polls;
        private readonly Dictionary<int, Dictionary<int, RoomPollQuestion>> _questions;

        public PollManager()
        {
            this._polls = new Dictionary<int, RoomPoll>();
            this._questions = new Dictionary<int, Dictionary<int, RoomPollQuestion>>();
        }

        public void Init()
        {
            if (this._questions.Count > 0)
                this._questions.Clear();

            int QuestionsLoaded = 0;

            DataTable GetQuestions = null;
            using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `room_poll_questions`;");
                GetQuestions = dbClient.getTable();

                if (GetQuestions != null)
                {
                    foreach (DataRow Row in GetQuestions.Rows)
                    {
                        int PollId = Convert.ToInt32(Row["poll_id"]);

                        if (!this._questions.ContainsKey(PollId))
                        {
                            this._questions[PollId] = new Dictionary<int, RoomPollQuestion>();
                        }

                        RoomPollQuestion CatalogItem = new RoomPollQuestion(Convert.ToInt32(Row["id"]), Convert.ToInt32(Row["poll_id"]), Convert.ToString(Row["question"]), Convert.ToString(Row["question_type"]), Convert.ToInt32(Row["series_order"]), Convert.ToInt32(Row["minimum_selections"]));

                        this._questions[CatalogItem.PollId].Add(CatalogItem.Id, CatalogItem);

                        QuestionsLoaded++;
                    }
                }
            }

            if (this._polls.Count > 0)
                this._polls.Clear();

            DataTable GetPolls = null;

            int PollsLoaded = 0;
            using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `room_polls` WHERE `enabled` = 'Y'");
                GetPolls = dbClient.getTable();

                if (GetPolls != null)
                {
                    foreach (DataRow Row in GetPolls.Rows)
                    {
                        this._polls.Add(Convert.ToInt32(Row["id"]), new RoomPoll(Convert.ToInt32(Row["id"]), Convert.ToInt32(Row["room_id"]),
                              Convert.ToString(Row["type"]), Convert.ToString(Row["headline"]), Convert.ToString(Row["summary"]), Convert.ToString(Row["completion_message"]),
                              Convert.ToInt32(Row["credit_reward"]), Convert.ToInt32(Row["pixel_reward"]), Convert.ToString(Row["badge_reward"]), Convert.ToDouble(Row["expiry"]),
                              this._questions.ContainsKey(Convert.ToInt32(Row["id"])) ? this._questions[Convert.ToInt32(Row["id"])] : new Dictionary<int, RoomPollQuestion>()));

                        PollsLoaded++;
                    }
                }

                log.Info("Loaded " + PollsLoaded + " room polls & " + QuestionsLoaded + " poll questions");
            }
        }

        public bool TryGetPoll(int pollId, out RoomPoll roomPoll)
        {
            return this._polls.TryGetValue(pollId, out roomPoll);
        }

        public bool TryGetPollForRoom(int roomId, out RoomPoll roomPoll)
        {
            roomPoll = null;
            if (this._polls.Count(x => x.Value.RoomId == roomId) == 0)
                return false;

            return this._polls.TryGetValue(this._polls.FirstOrDefault(x => x.Value.RoomId == roomId).Value.Id, out roomPoll);
        }
    }
}