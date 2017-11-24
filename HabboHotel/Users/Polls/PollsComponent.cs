using System;
using System.Data;
using System.Collections.Generic;

using Cloud.Database.Interfaces;

namespace Cloud.HabboHotel.Users.Polls
{
    public sealed class PollsComponent
    {
        private List<int> _completedPolls;

        public PollsComponent()
        {
            this._completedPolls = new List<int>();
        }

        public bool Init(Habbo habbo)
        {
            if (this._completedPolls.Count > 0)
                return false;

            DataTable GetPolls = null;
            using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `poll_id` FROM `user_room_poll_results` WHERE `user_id` = @uid GROUP BY `poll_id`;");
                dbClient.AddParameter("uid", habbo.Id);
                GetPolls = dbClient.getTable();

                if (GetPolls != null)
                {
                    foreach (DataRow Row in GetPolls.Rows)
                    {
                        if (!this._completedPolls.Contains(Convert.ToInt32(Row["poll_id"])))
                            this._completedPolls.Add(Convert.ToInt32(Row["poll_id"]));
                    }
                }
            }
            return true;
        }

        public bool TryAdd(int PollId)
        {
            if (this._completedPolls.Contains(PollId))
                return false;

            this._completedPolls.Add(PollId);
            return true;
        }

        public ICollection<int> CompletedPolls
        {
            get { return this._completedPolls; }
        }

        public void Dispose()
        {
            this._completedPolls.Clear();
        }
    }
}