using System;
using System.Data;
using Cloud.Database.Interfaces;

namespace Cloud.HabboHotel.Items.Data.Toner
{
    public class TonerData
    {
        public int ItemId;
        public int Hue;
        public int Saturation;
        public int Lightness;
        public int Enabled;

        public TonerData(int Item)
        {
            ItemId = Item;

            DataRow Row;

            using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT enabled,data1,data2,data3 FROM room_items_toner WHERE id=" + ItemId +" LIMIT 1");
                Row = dbClient.getRow();
            }

            if (Row == null)
            {
                using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.runFastQuery("INSERT INTO `room_items_toner` VALUES (" + ItemId + ",'0',0,0,0)");
                    dbClient.SetQuery("SELECT enabled,data1,data2,data3 FROM room_items_toner WHERE id=" + ItemId + " LIMIT 1");
                    Row = dbClient.getRow();
                }
            }

            Enabled = int.Parse(Row[0].ToString());
            Hue = Convert.ToInt32(Row[1]);
            Saturation = Convert.ToInt32(Row[2]);
            Lightness = Convert.ToInt32(Row[3]);
        }
    }
}