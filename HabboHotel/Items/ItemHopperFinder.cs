using System;
using Cloud.Database.Interfaces;

namespace Cloud.HabboHotel.Items
{
    public static class ItemHopperFinder
    {
        public static int GetAHopper(int CurRoom)
        {
            using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
            {
                int RoomId = 0;
                dbClient.SetQuery("SELECT room_id FROM items_hopper WHERE room_id <> @room ORDER BY room_id ASC LIMIT 1");
                dbClient.AddParameter("room", CurRoom);
                RoomId = dbClient.getInteger();
                return RoomId;
            }
        }

        public static int GetHopperId(int NextRoom)
        {
            using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT hopper_id FROM items_hopper WHERE room_id = @room LIMIT 1");
                dbClient.AddParameter("room", NextRoom);
                string Row = dbClient.getString();

                if (Row == null)
                    return 0;

                return Convert.ToInt32(Row);
            }
        }
    }
}