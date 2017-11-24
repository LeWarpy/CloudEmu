using Cloud.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using log4net;

namespace Cloud.HabboHotel.Subscriptions
{
    public class SubscriptionManager
    {
        private static ILog log = LogManager.GetLogger("Cloud.HabboHotel.Subscriptions.SubscriptionManager");
        private readonly Dictionary<int, SubscriptionData> _subscriptions = new Dictionary<int, SubscriptionData>();

        public SubscriptionManager()
        {
        }

        public void Init()
        {
            if (_subscriptions.Count > 0)
                _subscriptions.Clear();

            using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `subscriptions`;");
                DataTable GetSubscriptions = dbClient.getTable();

                if (GetSubscriptions != null)
                {
                    foreach (DataRow Row in GetSubscriptions.Rows)
                    {
                        if (!_subscriptions.ContainsKey(Convert.ToInt32(Row["id"])))
                             _subscriptions.Add(Convert.ToInt32(Row["id"]), new SubscriptionData(Convert.ToInt32(Row["id"]), Convert.ToString(Row["name"]), Convert.ToString(Row["badge_code"]), Convert.ToInt32(Row["credits"]), Convert.ToInt32(Row["duckets"]), Convert.ToInt32(Row["respects"]), Convert.ToInt32(Row["diamonds"])));
                    }
                }
            }
            log.Info("» Cargadas: " + _subscriptions.Count + " subscripciones.");
        }

        public bool TryGetSubscriptionData(int Id, out SubscriptionData Data)
        {
            return _subscriptions.TryGetValue(Id, out Data);
        }
    }
}
