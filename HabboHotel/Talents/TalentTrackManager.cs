﻿using System;

using System.Data;
using System.Collections.Generic;

using log4net;
using Cloud.Database.Interfaces;



namespace Cloud.HabboHotel.Talents
{
    public class TalentTrackManager
    {
        private static ILog log = LogManager.GetLogger("Cloud.HabboHotel.Talents.TalentManager");

        private readonly Dictionary<int, TalentTrackLevel> _citizenshipLevels;

        public TalentTrackManager()
        {
            this._citizenshipLevels = new Dictionary<int, TalentTrackLevel>();

            this.Init();
        }

        public void Init()
        {
            DataTable GetTable = null;
            using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `type`,`level`,`data_actions`,`data_gifts` FROM `talents`");
                GetTable = dbClient.getTable();
            }

            if (GetTable != null)
            {
                foreach (DataRow Row in GetTable.Rows)
                {
                    this._citizenshipLevels.Add(Convert.ToInt32(Row["level"]), new TalentTrackLevel(Convert.ToString(Row["type"]), Convert.ToInt32(Row["level"]), Convert.ToString(Row["data_actions"]), Convert.ToString(Row["data_gifts"])));
                }
            }
        }

        public ICollection<TalentTrackLevel> GetLevels()
        {
            return this._citizenshipLevels.Values;
        }
    }
}
