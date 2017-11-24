using Cloud.HabboHotel.Catalog;
using Cloud.HabboHotel.Items;
using System.Collections.Generic;

namespace Cloud.Communication.Packets.Outgoing.Catalog
{
    public class PurchaseOKComposer : ServerPacket
    {
        public PurchaseOKComposer(CatalogItem Item, ItemData BaseItem, Dictionary<ItemData, int> items)
            : base(ServerPacketHeader.PurchaseOKMessageComposer)
        {
			WriteInteger(BaseItem.Id);
			WriteString(BaseItem.ItemName);
			WriteBoolean(false);
			WriteInteger(Item.CostCredits);
			WriteInteger(Item.CostDiamonds);
			WriteInteger(Item.CostPixels);
			WriteBoolean(true);
			WriteInteger(items == null ? 0 : items.Count);
			WriteString(BaseItem.Type.ToString().ToLower());
			WriteInteger(BaseItem.SpriteId);
			WriteString("");
			WriteInteger(1);
			WriteInteger(0);
			WriteString("");
			WriteInteger(1);
        }

        public PurchaseOKComposer()
            : base(ServerPacketHeader.PurchaseOKMessageComposer)
        {
			WriteInteger(0);
			WriteString("");
			WriteBoolean(false);
			WriteInteger(0);
			WriteInteger(0);
			WriteInteger(0);
			WriteBoolean(true);
			WriteInteger(1);
			WriteString("s");
			WriteInteger(0);
			WriteString("");
			WriteInteger(1);
			WriteInteger(0);
			WriteString("");
			WriteInteger(1);
        }
    }
}