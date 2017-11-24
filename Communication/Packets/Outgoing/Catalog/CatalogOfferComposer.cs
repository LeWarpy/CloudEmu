using Cloud.HabboHotel.Items;
using Cloud.HabboHotel.Catalog;
using Cloud.HabboHotel.Catalog.Utilities;

namespace Cloud.Communication.Packets.Outgoing.Catalog
{
	class CatalogOfferComposer : ServerPacket
    {
        public CatalogOfferComposer(CatalogItem Item)
            : base(ServerPacketHeader.CatalogOfferMessageComposer)
        {
			WriteInteger(Item.OfferId);
			WriteString(Item.Data.ItemName);
			WriteBoolean(false);//IsRentable
			WriteInteger(Item.CostCredits);

            if (Item.CostDiamonds > 0)
            {
				WriteInteger(Item.CostDiamonds);
				WriteInteger(5); // Diamonds
            }
            else if (Item.CostGotw > 0)
            {
				WriteInteger(Item.CostGotw);
				WriteInteger(103); // Puntos de fama
            }
            else
            {
				WriteInteger(Item.CostPixels);
				WriteInteger(0); // Type of PixelCost

            }

			WriteBoolean(ItemUtility.CanGiftItem(Item));
			WriteInteger(string.IsNullOrEmpty(Item.Badge) ? 1 : 2);//Count 1 item if there is no badge, otherwise count as 2.

            if (!string.IsNullOrEmpty(Item.Badge))
            {
				WriteString("b");
				WriteString(Item.Badge);
            }

			WriteString(Item.Data.Type.ToString());
            if (Item.Data.Type.ToString().ToLower() == "b")
				WriteString(Item.Data.ItemName);//Badge name.
            else
            {
				WriteInteger(Item.Data.SpriteId);
                if (Item.Data.InteractionType == InteractionType.WALLPAPER || Item.Data.InteractionType == InteractionType.FLOOR || Item.Data.InteractionType == InteractionType.LANDSCAPE)
					WriteString(Item.Name.Split('_')[2]);
                else if (Item.PageID == 9)//Bots
                {
                    CatalogBot CataBot = null;
                    if (!CloudServer.GetGame().GetCatalog().TryGetBot(Item.ItemId, out CataBot))
						WriteString("hd-180-7.ea-1406-62.ch-210-1321.hr-831-49.ca-1813-62.sh-295-1321.lg-285-92");
                    else
						WriteString(CataBot.Figure);
                }
                else if (Item.ExtraData != null)
				WriteString(Item.ExtraData ?? string.Empty);
				WriteInteger(Item.Amount);
				WriteBoolean(Item.IsLimited); // IsLimited
                if (Item.IsLimited)
                {
					WriteInteger(Item.LimitedEditionStack);
					WriteInteger(Item.LimitedEditionStack - Item.LimitedEditionSells);
                }
            }
			WriteInteger(0); // club_level
			WriteBoolean(ItemUtility.CanSelectAmount(Item));
			WriteInteger(0);
        }
    }
}
