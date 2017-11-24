

namespace Cloud.HabboHotel.Subscriptions
{
    public class Subscription
    {
        private readonly string Caption;
        private int TimeExpire;
        private int ActivateTimes;

        internal string SubscriptionId
        {
            get
            {
                return Caption;
            }
        }

        internal int ExpireTime
        {
            get
            {
                return TimeExpire;
            }
        }


        internal int ActivateTime
        {
            get
            {
                return ActivateTimes;
            }
        }

        internal Subscription(string Caption, int TimeExpire, int ActivateTimes)
        {
            this.Caption = Caption;
            this.TimeExpire = TimeExpire;
            this.ActivateTimes = ActivateTimes;
        }

        internal bool IsValid()
        {
            return TimeExpire > CloudServer.GetUnixTimestamp();
        }

        internal void SetEndTime(int time)
        {
            TimeExpire = time;
        }

        internal void ExtendSubscription(int Time)
        {
            try
            {
                TimeExpire = (TimeExpire + Time);
            }
            catch
            {
            }
        }
    }
}
