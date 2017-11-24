using System;
using System.Data;
using Cloud.Database.Interfaces;
using Cloud.HabboHotel.GameClients;

namespace Cloud.Communication.Packets.Outgoing.Avatar
{
    class WardrobeComposer : ServerPacket
    {
        public WardrobeComposer(GameClient Session)
            : base(ServerPacketHeader.WardrobeMessageComposer)
        {
			WriteInteger(1);
            using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `slot_id`,`look`,`gender` FROM `user_wardrobe` WHERE `user_id` = '" + Session.GetHabbo().Id + "'");
                DataTable WardrobeData = dbClient.getTable();

                if (WardrobeData == null)
					WriteInteger(0);
                else
                {
					WriteInteger(WardrobeData.Rows.Count);
                    foreach (DataRow Row in WardrobeData.Rows)
                    {
						WriteInteger(Convert.ToInt32(Row["slot_id"]));
						WriteString(Convert.ToString(Row["look"]));
						WriteString(Row["gender"].ToString().ToUpper());
                    }
                }
            }
        }
    }
}
