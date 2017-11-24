using System;
using System.Linq;
using Cloud.Core;
using Cloud.HabboHotel.Items;
using Cloud.HabboHotel.Catalog;
using Cloud.HabboHotel.Catalog.Utilities;

namespace Cloud.Communication.Packets.Outgoing.Catalog
{
	public class CatalogPageComposer : ServerPacket
    {
        public CatalogPageComposer(CatalogPage Page, string CataMode)
            : base(ServerPacketHeader.CatalogPageMessageComposer)
        {
			WriteInteger(Page.Id);
			WriteString(CataMode);
			WriteString(Page.Template);

			WriteInteger(Page.PageStrings1.Count);
            foreach (string s in Page.PageStrings1)
            {
				WriteString(s);
            }

			WriteInteger(Page.PageStrings2.Count);
            foreach (string s in Page.PageStrings2)
            {
				WriteString(s);
            }

            if (!Page.Template.Equals("frontpage") && !Page.Template.Equals("loyalty_vip_buy"))
            {
				WriteInteger(Page.Items.Count);
                foreach (CatalogItem Item in Page.Items.Values)
                {
					WriteInteger(Item.Id);
					WriteString(Item.Name);
					WriteBoolean(false);//IsRentable
					WriteInteger(Item.CostCredits);

                    if (Item.CostDiamonds > 0)
                    {
						WriteInteger(Item.CostDiamonds);
						WriteInteger(5); // Diamonds
                    }
                    else if (Item.CostPixels > 0)
                    {
						WriteInteger(Item.CostPixels);
						WriteInteger(0); // Type of PixelCost

                    }
                    else
                    {
						WriteInteger(Item.CostGotw);
						WriteInteger(103); // Gotw
                    }

					WriteBoolean(Item.PredesignedId > 0 ? false : ItemUtility.CanGiftItem(Item));
                    if (Item.PredesignedId > 0)
                    {
						WriteInteger(Page.PredesignedItems.Items.Count);
                        foreach (var predesigned in Page.PredesignedItems.Items.ToList())
                        {
                            ItemData Data = null;
                            if (CloudServer.GetGame().GetItemManager().GetItem(predesigned.Key, out Data)) { }
							WriteString(Data.Type.ToString());
							WriteInteger(Data.SpriteId);
							WriteString(string.Empty);
							WriteInteger(predesigned.Value);
							WriteBoolean(false);
                        }

						WriteInteger(0);
						WriteBoolean(false);
						WriteBoolean(true); // Niu Rilí
						WriteString(""); // Niu Rilí
                    }
                    else if (Page.Deals.Count > 0)
                    {
                        foreach (var Deal in Page.Deals.Values)
                        {
							WriteInteger(Deal.ItemDataList.Count);
                            foreach (var DealItem in Deal.ItemDataList.ToList())
                            {
								WriteString(DealItem.Data.Type.ToString());
								WriteInteger(DealItem.Data.SpriteId);
								WriteString(string.Empty);
								WriteInteger(DealItem.Amount);
								WriteBoolean(false);
                            }

							WriteInteger(0);
							WriteBoolean(false);
                        }
                    }
                    else
                    {
						WriteInteger(string.IsNullOrEmpty(Item.Badge) ? 1 : 2);//Count 1 item if there is no badge, otherwise count as 2.
                        {
                            if (!string.IsNullOrEmpty(Item.Badge))
                            {
								WriteString("b");
								WriteString(Item.Badge);
                            }

							WriteString(Item.Data.Type.ToString());
                            if (Item.Data.Type.ToString().ToLower() == "b")
                            {
								//This is just a badge, append the name.
								WriteString(Item.Data.ItemName);
                            }
                            else
                            {
								WriteInteger(Item.Data.SpriteId);
                                if (Item.Data.InteractionType == InteractionType.WALLPAPER || Item.Data.InteractionType == InteractionType.FLOOR || Item.Data.InteractionType == InteractionType.LANDSCAPE)
                                {
									WriteString(Item.Name.Split('_')[2]);
                                }
                                else if (Item.Data.InteractionType == InteractionType.BOT)//Bots
                                {
                                    CatalogBot CatalogBot = null;
                                    if (!CloudServer.GetGame().GetCatalog().TryGetBot(Item.ItemId, out CatalogBot))
										WriteString("hd-180-7.ea-1406-62.ch-210-1321.hr-831-49.ca-1813-62.sh-295-1321.lg-285-92");
                                    else
										WriteString(CatalogBot.Figure);
                                }
                                else if (Item.ExtraData != null)
                                {
									WriteString(Item.ExtraData ?? string.Empty);
                                }
								WriteInteger(Item.Amount);
								WriteBoolean(Item.IsLimited); // IsLimited
                                if (Item.IsLimited)
                                {
									WriteInteger(Item.LimitedEditionStack);
									WriteInteger(Item.LimitedEditionStack - Item.LimitedEditionSells);
                                }
                            }
							WriteInteger(0); //club_level
							WriteBoolean(ItemUtility.CanSelectAmount(Item));

							WriteBoolean(true); // Niu Rilí
							WriteString(""); // Niu Rilí
                        }
                    }
                }
                /*}*/
            }
            else
				WriteInteger(0);
			WriteInteger(-1);
			WriteBoolean(false);
            if (Page.Template.Equals("frontpage4"))
            {
				WriteInteger(4); // count

				//New Rare Jellyfish Lamp!
				WriteInteger(1); // id
				WriteString(CatalogSettings.CATALOG_NOTICE_1); // name
				WriteString(CatalogSettings.CATALOG_IMG_NOTICE_1); // image
				WriteInteger(0);
				WriteString(CatalogSettings.CATALOG_URL_NOTICE_1); // page link?
				WriteInteger(-1); // page id?


				WriteInteger(2);
				WriteString(CatalogSettings.CATALOG_NOTICE_2); // name
				WriteString(CatalogSettings.CATALOG_IMG_NOTICE_2); // image
				WriteInteger(0);
				WriteString(CatalogSettings.CATALOG_URL_NOTICE_2); // page link?
				WriteInteger(-1);


				WriteInteger(3);
				WriteString(CatalogSettings.CATALOG_NOTICE_3); // name
				WriteString(CatalogSettings.CATALOG_IMG_NOTICE_3); // image
				WriteInteger(0);
				WriteString(CatalogSettings.CATALOG_URL_NOTICE_3); // page link?
				WriteInteger(-1);


				WriteInteger(4);
				WriteString(CatalogSettings.CATALOG_NOTICE_4); // name
				WriteString(CatalogSettings.CATALOG_IMG_NOTICE_4); // image
				WriteInteger(0);
				WriteString(CatalogSettings.CATALOG_URL_NOTICE_4); // page link?
				WriteInteger(-1);

                if (Page.Template.Equals("loyalty_vip_buy"))
                {
					WriteInteger(0); //Page ID
					WriteString("NORMAL");
					WriteString("loyalty_vip_buy");
					WriteInteger(2);
					WriteString("hc2_clubtitle");
					WriteString("clubcat_pic");
					WriteInteger(0); // Nueva Release
					WriteInteger(0);
					WriteInteger(-1);
					WriteBoolean(false);

                    if (Page.Template.Equals("club_gifts"))
                    {
						WriteString("club_gifts");
						WriteInteger(1);
						WriteString(Convert.ToString(Page.PageStrings2));
						WriteInteger(1);
						WriteString(Convert.ToString(Page.PageStrings2));
                    }
                }
            }
        }
    }
}