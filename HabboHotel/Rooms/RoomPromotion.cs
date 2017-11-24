using System;

namespace Cloud.HabboHotel.Rooms
{
    public class RoomPromotion
    {

        private string _name;
        private string _description;
        private double _timestampExpires;
        private double _timestampStarted;
        private int _categoryId;

        public RoomPromotion(string Name, string Desc, int CategoryId)
        {
            _name = Name;
            _description = Desc;
            _timestampStarted = CloudServer.GetUnixTimestamp();
            _timestampExpires = (CloudServer.GetUnixTimestamp()) + (Convert.ToInt32(CloudServer.GetGame().GetSettingsManager().TryGetValue("room.promotion.lifespan")) * 60);
            _categoryId = CategoryId;
        }

        public RoomPromotion(string Name, string Desc, double Started, double Expires, int CategoryId)
        {
            _name = Name;
            _description = Desc;
            _timestampStarted = Started;
            _timestampExpires = Expires;
            _categoryId = CategoryId;
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }
        public double TimestampStarted
        {
            get { return _timestampStarted; }
        }

        public double TimestampExpires
        {
            get { return _timestampExpires; }
            set { _timestampExpires = value; }
        }

        public bool HasExpired
        {
            get { return (TimestampExpires - CloudServer.GetUnixTimestamp()) < 0; }
        }

        public int MinutesLeft
        {
            get { return Convert.ToInt32(Math.Ceiling((TimestampExpires - CloudServer.GetUnixTimestamp()) / 60)); }
        }

        public int CategoryId
        {
            get { return _categoryId; }
            set { _categoryId = value; }
        }
    }
}