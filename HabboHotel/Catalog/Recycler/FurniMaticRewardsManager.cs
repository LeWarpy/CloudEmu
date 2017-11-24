using Cloud.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Cloud.HabboHotel.Catalog.FurniMatic
{
    public class FurniMaticRewardsManager
    {
        private List<FurniMaticRewards> Rewards;
        public List<FurniMaticRewards> GetRewards() { return Rewards; }
        public List<FurniMaticRewards> GetRewardsByLevel(int level)
        {
            var rewards = new List<FurniMaticRewards>();
            foreach (var reward in Rewards.Where(furni => furni.Level == level)) rewards.Add(reward);
            return rewards;
        }
        
        public FurniMaticRewards GetRandomReward()
        {
            var level = 0;
            var rand = new Random().Next(0, 300);
            if (rand >= 285) level = 5;                   // 005% de probabilidad de que salga nivel 5
            else if (rand >= 250 && rand < 285) level = 4; // 010% de probabilidad de que salga nivel 4
            else if (rand >= 150 && rand < 250) level = 3; // 020% de probabilidad de que salga nivel 3
            else if (rand >= 75 && rand < 150) level = 2; // 030% de probabilidad de que salga nivel 2
            else level = 1;                              // 035% de probabilidad de que salga nivel 1
                                                         // 100%
            var possibleRewards = GetRewardsByLevel(level);
            if (possibleRewards != null && possibleRewards.Count >= 1) return possibleRewards[new Random().Next(0, (possibleRewards.Count - 1))];
            else return new FurniMaticRewards(0, 470, 0);
        }

        public void Initialize(IQueryAdapter dbClient)
        {
            Rewards = new List<FurniMaticRewards>();
            dbClient.SetQuery("SELECT display_id, item_id, reward_level FROM catalog_ecotron_rewards");
            var table = dbClient.getTable();
            if (table == null) return;
            foreach (DataRow row in table.Rows) Rewards.Add(new FurniMaticRewards(Convert.ToInt32(row["display_id"]), Convert.ToInt32(row["item_id"]), Convert.ToInt32(row["reward_level"])));
        }
    }
}