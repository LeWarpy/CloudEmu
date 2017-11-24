﻿
using System;
using System.Collections.Generic;
using System.Data;
using Cloud.Database.Interfaces;


namespace Cloud.HabboHotel.Users.Messenger
{
    public class SearchResultFactory
    {
        public static List<SearchResult> GetSearchResult(string query)
        {
            DataTable dTable;
            using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `id`,`username`,`motto`,`look`,`last_online` FROM users WHERE username LIKE @query LIMIT 50");

                dbClient.AddParameter("query", query + "%");
                dTable = dbClient.getTable();
            }

            List<SearchResult> results = new List<SearchResult>();

            if (dTable != null)
            {
                foreach (DataRow dRow in dTable.Rows)
                {
                    results.Add(new SearchResult(Convert.ToInt32(dRow[0]), Convert.ToString(dRow[1]), Convert.ToString(dRow[2]), Convert.ToString(dRow[3]), dRow[4].ToString()));
                }
            }

            return results;
        }
    }
}