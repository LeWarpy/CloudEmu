
using Cloud.Communication.Packets.Outgoing.Handshake;
using Cloud.Database.Interfaces;
using Cloud.HabboHotel.GameClients;
using System.Collections.Generic;
using Cloud.HabboHotel.Users.UserData;

namespace Cloud.HabboHotel.Subscriptions
{
    internal class ClubManager
    {
        private readonly int UserId;
        private readonly Dictionary<string, Subscription> Subscriptions;

        internal ClubManager(int userID, UserData userData)
        {
           UserId = userID;
           Subscriptions = userData.subscriptions;
        }

        internal void Clear()
        {
            Subscriptions.Clear();
        }

        internal Subscription GetSubscription(string SubscriptionId)
        {
            if (Subscriptions.ContainsKey(SubscriptionId))
            {
                return Subscriptions[SubscriptionId];
            }
            else
            {
                return null;
            }
        }

        internal bool HasSubscription(string SubscriptionId)
        {
            if (!Subscriptions.ContainsKey(SubscriptionId))
            {
                return false;
            }

            Subscription subscription = Subscriptions[SubscriptionId];
            return subscription.IsValid();
        }

        internal void AddOrExtendSubscription(string SubscriptionId, int DurationSeconds, GameClient Session)
        {
            SubscriptionId = SubscriptionId.ToLower();

            var clientByUserId = CloudServer.GetGame().GetClientManager().GetClientByUserID(UserId);
            if (Subscriptions.ContainsKey(SubscriptionId))
            {
                Subscription subscription = Subscriptions[SubscriptionId];

                if (subscription.IsValid())
                {
                    subscription.ExtendSubscription(DurationSeconds);
                }
                else
                {
                    subscription.SetEndTime((int)CloudServer.GetUnixTimestamp() + DurationSeconds);
                }

                using (IQueryAdapter adapter = CloudServer.GetDatabaseManager().GetQueryReactor())
                {
                    adapter.SetQuery(string.Concat(new object[] { "UPDATE user_subscriptions SET timestamp_expire = ", subscription.ExpireTime, " WHERE user_id = ", this.UserId, " AND subscription_id = '", subscription.SubscriptionId, "'" }));
                    adapter.RunQuery();
                }
                CloudServer.GetGame().GetAchievementManager().TryProgressHabboClubAchievements(clientByUserId);
            }
            else
            {
                int unixTimestamp = (int)CloudServer.GetUnixTimestamp();
                int timeExpire = (int)CloudServer.GetUnixTimestamp() + DurationSeconds;
                string SubscriptionType = SubscriptionId;
                Subscription subscription2 = new Subscription(SubscriptionId, timeExpire, unixTimestamp);

                using (IQueryAdapter adapter = CloudServer.GetDatabaseManager().GetQueryReactor())
                {
                    adapter.SetQuery(string.Concat(new object[] { "INSERT INTO user_subscriptions (user_id,subscription_id,timestamp_activated,timestamp_expire) VALUES (", this.UserId, ",'", SubscriptionType, "',", unixTimestamp, ",", timeExpire, ")" }));
                    adapter.RunQuery();
                }

                Subscriptions.Add(subscription2.SubscriptionId.ToLower(), subscription2);
                CloudServer.GetGame().GetAchievementManager().TryProgressHabboClubAchievements(clientByUserId);
            }
        }


        internal void ReloadSubscription(GameClient Session)
        {
            Session.SendMessage(new UserRightsComposer(Session.GetHabbo()));
        }
    }
}