using System;
using System.Linq;
using System.Data;
using System.Collections.Generic;
using log4net;
using Cloud.HabboHotel.Items;
using Cloud.HabboHotel.Catalog.Pets;
using Cloud.HabboHotel.Catalog.Vouchers;
using Cloud.HabboHotel.Catalog.Marketplace;
using Cloud.HabboHotel.Catalog.Clothing;
using Cloud.HabboHotel.Catalog.PredesignedRooms;
using Cloud.Database.Interfaces;

namespace Cloud.HabboHotel.Catalog
{
    public class CatalogManager
    {
        private static readonly ILog log = LogManager.GetLogger("Cloud.HabboHotel.Catalog.CatalogManager");
        private MarketplaceManager _marketplace;
        private PetRaceManager _petRaceManager;
        private VoucherManager _voucherManager;
        private ClothingManager _clothingManager;
        private PredesignedRoomsManager _predesignedManager;
        private Dictionary<int, int> _itemOffers;
        private Dictionary<int, CatalogPage> _pages;
        private Dictionary<int, CatalogBot> _botPresets;
        private Dictionary<int, Dictionary<int, CatalogItem>> _items;
        private Dictionary<int, Dictionary<int, CatalogDeal>> _deals;
        private Dictionary<int, PredesignedContent> _predesignedItems;
        private readonly List<int> _recyclerLevels;

        public CatalogManager()
        {
            _marketplace = new MarketplaceManager();
            _petRaceManager = new PetRaceManager();
            _voucherManager = new VoucherManager();
            _clothingManager = new ClothingManager();
            _predesignedManager = new PredesignedRoomsManager();
            _predesignedManager.Initialize();

            _itemOffers = new Dictionary<int, int>();
            _pages = new Dictionary<int, CatalogPage>();
            _botPresets = new Dictionary<int, CatalogBot>();
            _items = new Dictionary<int, Dictionary<int, CatalogItem>>();
            _deals = new Dictionary<int, Dictionary<int, CatalogDeal>>();
            _predesignedItems = new Dictionary<int, PredesignedContent>();
            _recyclerLevels = new List<int>();
        }

        public void Init(ItemDataManager ItemDataManager)
        {
            if (_pages.Count > 0)
                _pages.Clear();
            if (_botPresets.Count > 0)
                _botPresets.Clear();
            if (_items.Count > 0)
                _items.Clear();
            if (_deals.Count > 0)
                _deals.Clear();
            if (_predesignedItems.Count > 0)
                _predesignedItems.Clear();
            if (_recyclerLevels.Count > 0)
                _recyclerLevels.Clear();

            using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `id`,`item_id`,`catalog_name`,`cost_credits`,`cost_pixels`,`cost_diamonds`,`cost_gotw`,`amount`,`page_id`,`limited_sells`,`limited_stack`,`offer_active`,`extradata`,`badge`,`offer_id`, `predesigned_id` FROM `catalog_items`");
                DataTable CatalogueItems = dbClient.getTable();

                if (CatalogueItems != null)
                {
                    foreach (DataRow Row in CatalogueItems.Rows)
                    {
                        if (Convert.ToInt32(Row["amount"]) <= 0)
                            continue;

                        int ItemId = Convert.ToInt32(Row["id"]);
                        int PageId = Convert.ToInt32(Row["page_id"]);
                        int BaseId = Convert.ToInt32(Row["item_id"]);
                        int OfferId = Convert.ToInt32(Row["offer_id"]);
                        uint PredesignedId = Convert.ToUInt32(Row["predesigned_id"]);
                        if (BaseId == 0 && PredesignedId > 0)
                        {
                            var roomPack = _predesignedManager.predesignedRoom[PredesignedId];
                            if (roomPack == null) continue;
                            if (roomPack.CatalogItems.Contains(";"))
                            {
                                var cataItems = new Dictionary<int, int>();
                                var itemArray = roomPack.CatalogItems.Split(new char[] { ';' });
                                foreach (var item in itemArray)
                                {
                                    var items = item.Split(',');
                                    ItemData PredesignedData = null;
                                    if (!ItemDataManager.GetItem(Convert.ToInt32(items[0]), out PredesignedData))
                                    {
                                        log.Error("Couldn't load Catalog Item " + ItemId + ", no furniture record found.");
                                        continue;
                                    }

                                    cataItems.Add(Convert.ToInt32(items[0]), Convert.ToInt32(items[1]));
                                }

                                _predesignedItems[PageId] = new PredesignedContent(ItemId, cataItems);
                            }
                        }

                        ItemData Data = null;
                        if (PredesignedId <= 0)
                            if (!ItemDataManager.GetItem(BaseId, out Data))
                            {
                                log.Error("Couldn't load Catalog Item " + ItemId + ", no furniture record found.");
                                continue;
                            }

                        if (!_items.ContainsKey(PageId))
                            _items[PageId] = new Dictionary<int, CatalogItem>();

                        if (OfferId != -1 && !_itemOffers.ContainsKey(OfferId))
                            _itemOffers.Add(OfferId, PageId);

                            _items[PageId].Add(Convert.ToInt32(Row["id"]), new CatalogItem(Convert.ToInt32(Row["id"]), Convert.ToInt32(Row["item_id"]),
                            Data, Convert.ToString(Row["catalog_name"]), Convert.ToInt32(Row["page_id"]), Convert.ToInt32(Row["cost_credits"]), Convert.ToInt32(Row["cost_pixels"]), Convert.ToInt32(Row["cost_diamonds"]),
                            Convert.ToInt32(Row["cost_gotw"]), Convert.ToInt32(Row["amount"]), Convert.ToInt32(Row["limited_sells"]), Convert.ToInt32(Row["limited_stack"]), CloudServer.EnumToBool(Row["offer_active"].ToString()),
                            Convert.ToString(Row["extradata"]), Convert.ToString(Row["badge"]), Convert.ToInt32(Row["offer_id"]), Convert.ToInt32(Row["predesigned_id"])));
                    }
                }

                dbClient.SetQuery("SELECT * FROM `catalog_deals`");
                DataTable GetDeals = dbClient.getTable();

                if (GetDeals != null)
                {
                    foreach (DataRow Row in GetDeals.Rows)
                    {
                        int Id = Convert.ToInt32(Row["id"]);
                        int PageId = Convert.ToInt32(Row["page_id"]);
                        string Items = Convert.ToString(Row["items"]);
                        string Name = Convert.ToString(Row["name"]);
                        int Credits = Convert.ToInt32(Row["cost_credits"]);
                        int Pixels = Convert.ToInt32(Row["cost_pixels"]);

                        if (!_deals.ContainsKey(PageId))
                            _deals[PageId] = new Dictionary<int, CatalogDeal>();

                        CatalogDeal Deal = new CatalogDeal(Id, PageId, Items, Name, Credits, Pixels, ItemDataManager);
                        _deals[PageId].Add(Deal.Id, Deal);
                    }
                }


                dbClient.SetQuery("SELECT `id`,`parent_id`,`caption`,`page_link`,`visible`,`enabled`,`min_rank`,`min_vip`,`icon_image`,`page_layout`,`page_strings_1`,`page_strings_2` FROM `catalog_pages` ORDER BY `order_num`");
                DataTable CatalogPages = dbClient.getTable();

                if (CatalogPages != null)
                {
                    foreach (DataRow Row in CatalogPages.Rows)
                    {
                        _pages.Add(Convert.ToInt32(Row["id"]), new CatalogPage(Convert.ToInt32(Row["id"]), Convert.ToInt32(Row["parent_id"]), Row["enabled"].ToString(), Convert.ToString(Row["caption"]),
                            Convert.ToString(Row["page_link"]), Convert.ToInt32(Row["icon_image"]), Convert.ToInt32(Row["min_rank"]), Convert.ToInt32(Row["min_vip"]), Row["visible"].ToString(), Convert.ToString(Row["page_layout"]),
                            Convert.ToString(Row["page_strings_1"]), Convert.ToString(Row["page_strings_2"]),
                            _items.ContainsKey(Convert.ToInt32(Row["id"])) ? _items[Convert.ToInt32(Row["id"])] : new Dictionary<int, CatalogItem>(),
                            _deals.ContainsKey(Convert.ToInt32(Row["id"])) ? _deals[Convert.ToInt32(Row["id"])] : new Dictionary<int, CatalogDeal>(),
                            _predesignedItems.ContainsKey(Convert.ToInt32(Row["id"])) ? _predesignedItems[Convert.ToInt32(Row["id"])] : null,
                            ref _itemOffers));
                    }
                }

                dbClient.SetQuery("SELECT `id`,`name`,`figure`,`motto`,`gender`,`ai_type` FROM `catalog_bot_presets`");
                DataTable bots = dbClient.getTable();

                if (bots != null)
                {
                    foreach (DataRow Row in bots.Rows)
                    {
                        _botPresets.Add(Convert.ToInt32(Row[0]), new CatalogBot(Convert.ToInt32(Row[0]), Convert.ToString(Row[1]), Convert.ToString(Row[2]), Convert.ToString(Row[3]), Convert.ToString(Row[4]), Convert.ToString(Row[5])));
                    }
                }

                _petRaceManager.Init();
                _clothingManager.Init();
            }

            log.Info("» Catalogo -> CARGADO");
        }

        public bool TryGetBot(int ItemId, out CatalogBot Bot)
        {
            return _botPresets.TryGetValue(ItemId, out Bot);
        }

        public Dictionary<int, int> ItemOffers
        {
            get { return _itemOffers; }
        }

        public bool TryGetPage(int pageId, out CatalogPage page)
        {
            return _pages.TryGetValue(pageId, out page);
        }

        public CatalogPage TryGetPageByTemplate(string template)
        {
            return _pages.Values.Where(current => current.Template == template).First();
        }

        public ICollection<CatalogPage> GetPages()
        {
            return _pages.Values;
        }

        public MarketplaceManager GetMarketplace()
        {
            return _marketplace;
        }

        public PetRaceManager GetPetRaceManager()
        {
            return _petRaceManager;
        }

        public VoucherManager GetVoucherManager()
        {
            return _voucherManager;
        }

        public ClothingManager GetClothingManager()
        {
            return _clothingManager;
        }

        internal List<int> GetEcotronRewardsLevels()
        {
            return _recyclerLevels;
        }
        internal PredesignedRoomsManager GetPredesignedRooms()
        {
            return _predesignedManager;
        }
    }
}