using System.Linq;
using Cloud.HabboHotel.Rooms.Trading;
using Cloud.HabboHotel.Items;


namespace Cloud.Communication.Packets.Outgoing.Inventory.Trading
{
    class TradingUpdateComposer : ServerPacket
    {
        public TradingUpdateComposer(Trade Trade)
            : base(ServerPacketHeader.TradingUpdateMessageComposer)
        {
            if (Trade.Users.Count() < 2)
                return;
            
            foreach(TradeUser user in Trade.Users)
            {
				WriteInteger(user.GetClient().GetHabbo().Id);
				WriteInteger(user.OfferedItems.Count);

                SerializeUserItems(user);

				WriteInteger(user.OfferedItems.Count);
				WriteInteger(0);

            }
            

        }
        private void SerializeUserItems(TradeUser User)
        {
            //base.WriteInteger(User.OfferedItems.Count);//While
            foreach (Item Item in User.OfferedItems.ToList())
            {
				WriteInteger(Item.Id);
				WriteString(Item.Data.Type.ToString().ToUpper());
				WriteInteger(Item.Id);
				WriteInteger(Item.Data.SpriteId);
				WriteInteger(1);
				WriteBoolean(true);

				//Func called _SafeStr_15990
				WriteInteger(0);
				WriteString("");

				//end Func called
				WriteInteger(0);
				WriteInteger(0);
				WriteInteger(0);
                if (Item.Data.Type.ToString().ToUpper() == "S")
					WriteInteger(0);
            }

        }
    }
}