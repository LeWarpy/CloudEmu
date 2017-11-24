
using Cloud.Communication.Packets.Outgoing;
using Cloud.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace Cloud.Communication.Packets.Incoming.LandingView
{
    public class GetHallOfFame
    {
        public static GetHallOfFame instance = new GetHallOfFame();
        public static GetHallOfFame GetInstance() => instance;
        public Dictionary<uint, UserRank> ranks;
        public List<UserCompetition> usersWithRank;

        public GetHallOfFame()
        {
            ranks = new Dictionary<uint, UserRank>();
            usersWithRank = new List<UserCompetition>();
        }

        public void Load()
        {
            usersWithRank = new List<UserCompetition>();
            usersWithRank.Clear();

            using (IQueryAdapter dbQuery = CloudServer.GetDatabaseManager().GetQueryReactor())
            {
                ranks = new Dictionary<uint, UserRank>();
                usersWithRank = new List<UserCompetition>();

                dbQuery.SetQuery("SELECT * FROM `users` WHERE `gotw_points` >= '1' AND `rank` = '1' ORDER BY `gotw_points` DESC LIMIT 16");
                DataTable gUsersTable = dbQuery.getTable();

                foreach (DataRow Row in gUsersTable.Rows)
                {
                    var staff = new UserCompetition(Row);
                    if (!usersWithRank.Contains(staff))
                        usersWithRank.Add(staff);
                }
            }

        }

        public void Serialize(ServerPacket message)
        {
            message.WriteInteger(usersWithRank.Count);
            foreach (UserCompetition user in usersWithRank)
            {
                message.WriteInteger(user.userId); //userID
                message.WriteString(user.userName);//userName
                message.WriteString(user.Look);//Look
                message.WriteInteger(user.Rank); //rank
                message.WriteInteger(user.Score);//?
            }
        }
    }

    public class UserCompetition
    {
        public int userId, Rank, Score;
        public string userName, Look;

        public UserCompetition(DataRow row)
        {
			userId = (int)row["id"];
			userName = (string)row["username"];
			Look = (string)row["look"];
			Rank = Convert.ToInt32(row["rank"].ToString());
			Score = Convert.ToInt32(row["gotw_points"].ToString());
        }
    }

    public class UserRank
    {
        public string Name, BadgeId;

        public UserRank(string Name, string BadgeId)
        {
            this.Name = Name;
            this.BadgeId = BadgeId;
        }
    }
}
