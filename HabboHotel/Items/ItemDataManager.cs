using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using log4net;
using Cloud.Database.Interfaces;

namespace Cloud.HabboHotel.Items
{
    public class ItemDataManager
    {
        private static readonly ILog log = LogManager.GetLogger("Cloud.HabboHotel.Items.ItemDataManager");
        public Dictionary<int, ItemData> _items;
        public Dictionary<int, ItemData> _gifts;//<SpriteId, Item>

		public ItemDataManager()
        {
            _items = new Dictionary<int, ItemData>();
            _gifts = new Dictionary<int, ItemData>();
        }

        public void Init()
        {
            if (_items.Count > 0)
                _items.Clear();

            using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `furniture`");
                DataTable ItemData = dbClient.getTable();

                if (ItemData != null)
                {
                    foreach (DataRow Row in ItemData.Rows)
                    {
                        try
                        {
                            int id = Convert.ToInt32(Row["id"]);
                            int spriteID = Convert.ToInt32(Row["sprite_id"]);
                            string itemName = Convert.ToString(Row["item_name"]);
                            string PublicName = Convert.ToString(Row["public_name"]);
                            string type = Row["type"].ToString();
                            int width = Convert.ToInt32(Row["width"]);
                            int length = Convert.ToInt32(Row["length"]);
                            double height = Convert.ToDouble(Row["stack_height"]);
                            bool allowStack = CloudServer.EnumToBool(Row["can_stack"].ToString());
                            bool allowWalk = CloudServer.EnumToBool(Row["is_walkable"].ToString());
                            bool allowSit = CloudServer.EnumToBool(Row["can_sit"].ToString());
                            bool allowRecycle = CloudServer.EnumToBool(Row["allow_recycle"].ToString());
                            bool allowTrade = CloudServer.EnumToBool(Row["allow_trade"].ToString());
                            bool allowMarketplace = Convert.ToInt32(Row["allow_marketplace_sell"]) == 1;
                            bool allowGift = Convert.ToInt32(Row["allow_gift"]) == 1;
                            bool allowInventoryStack = CloudServer.EnumToBool(Row["allow_inventory_stack"].ToString());
                            InteractionType interactionType = InteractionTypes.GetTypeFromString(Convert.ToString(Row["interaction_type"]));
                            int behaviourData = Convert.ToInt32(Row["behaviour_data"]);
                            int cycleCount = Convert.ToInt32(Row["interaction_modes_count"]);
                            string vendingIDS = Convert.ToString(Row["vending_ids"]);
                            string heightAdjustable = Convert.ToString(Row["height_adjustable"]);
                            int EffectId = Convert.ToInt32(Row["effect_id"]);
                            int WiredId = Convert.ToInt32(Row["wired_id"]);
                            bool IsRare = CloudServer.EnumToBool(Row["is_rare"].ToString());
                            int ClothingId = Convert.ToInt32(Row["clothing_id"]);
                            bool ExtraRot = CloudServer.EnumToBool(Row["extra_rot"].ToString());

                            if (!_gifts.ContainsKey(spriteID))
                                _gifts.Add(spriteID, new ItemData(id, spriteID, itemName, PublicName, type, width, length, height, allowStack, allowWalk, allowSit, allowRecycle, allowTrade, allowMarketplace, allowGift, allowInventoryStack, interactionType, behaviourData, cycleCount, vendingIDS, heightAdjustable, EffectId, WiredId, IsRare, ClothingId, ExtraRot));

                            if (!_items.ContainsKey(id))
                                _items.Add(id, new ItemData(id, spriteID, itemName, PublicName, type, width, length, height, allowStack, allowWalk, allowSit, allowRecycle, allowTrade, allowMarketplace, allowGift, allowInventoryStack, interactionType, behaviourData, cycleCount, vendingIDS, heightAdjustable, EffectId, WiredId, IsRare, ClothingId, ExtraRot));
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString());
                            Console.ReadKey();
                            log.Warn("Could not load item #" + Convert.ToInt32(Row[0]) + ", please verify the data is okay.");
                        }
                    }
                }
            }

            log.Info("» Administrador de Items -> CARGADO");
        }

        public bool GetItem(int Id, out ItemData Item)
        {
            Item = _items.Values.Where(x => x.Id == Id).FirstOrDefault();
            return Item != null;
        }

        public bool GetItemTwo(string itemName, out ItemData item)
        {
            item = _items.Values.Where(x => x.ItemName == itemName).FirstOrDefault();
            return item != null;
        }

        internal bool GetItem(string itemName, out ItemData item)
        {
            item = _items.Values.FirstOrDefault(x => x.ItemName == itemName);
            return item != null;
        }

        public bool GetGift(int SpriteId, out ItemData Item)
        {
            if (_gifts.TryGetValue(SpriteId, out Item))
                return true;
            return false;
        }

        public ItemData GetItemByName(string name)
        {
            foreach (var entry in _items)
            {
                ItemData item = entry.Value;
                if (item.ItemName == name)
                    return item;
            }
            return null;
        }
    }
}