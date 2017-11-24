﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Cloud.HabboHotel.Rooms.AI;
using Cloud.HabboHotel.Items;
using Cloud.HabboHotel.GameClients;
using Cloud.HabboHotel.Users.Inventory.Pets;
using Cloud.HabboHotel.Users.Inventory.Bots;
using Cloud.Communication.Packets.Outgoing.Inventory.Furni;
using Cloud.Database.Interfaces;

namespace Cloud.HabboHotel.Users.Inventory
{
    public class InventoryComponent
    {
        private int _userId;
        private GameClient _client;

        public int InventaryUserId;

        public ConcurrentDictionary<int, Bot> _botItems;
        public ConcurrentDictionary<int, Pet> _petsItems;
        public ConcurrentDictionary<int, Item> _floorItems;
        public ConcurrentDictionary<int, Item> _wallItems;
        public ConcurrentDictionary<int, Item> _songDisks;

        public InventoryComponent(int UserId, GameClient Client)
        {
            this._client = Client;
            this._userId = UserId;

            this.InventaryUserId = 0;

            this._floorItems = new ConcurrentDictionary<int, Item>();
            this._wallItems = new ConcurrentDictionary<int, Item>();
            this._petsItems = new ConcurrentDictionary<int, Pet>();
            this._botItems = new ConcurrentDictionary<int, Bot>();
            this._songDisks = new ConcurrentDictionary<int, Item>();

            this.Init();
        }

        public void Init()
        {
            if (this._floorItems.Count > 0)
                this._floorItems.Clear();
            if (this._wallItems.Count > 0)
                this._wallItems.Clear();
            if (this._petsItems.Count > 0)
                this._petsItems.Clear();
            if (this._botItems.Count > 0)
                this._botItems.Clear();
            if (this._songDisks.Count > 0)
                this._songDisks.Clear();

            List<Item> Items = ItemLoader.GetItemsForUser(_userId);
            foreach (Item Item in Items.ToList())
            {
                if (Item.IsFloorItem)
                {
                    if (!this._floorItems.TryAdd(Item.Id, Item))
                        continue;
                }
                else if (Item.IsWallItem)
                {
                    if (!this._wallItems.TryAdd(Item.Id, Item))
                        continue;
                }
                else
                    continue;
            }

            List<Pet> Pets = PetLoader.GetPetsForUser(Convert.ToInt32(_userId));
            foreach (Pet Pet in Pets)
            {
                if (!this._petsItems.TryAdd(Pet.PetId, Pet))
                {
                    Console.WriteLine("Error whilst loading pet x1: " + Pet.PetId);
                }
            }

            List<Bot> Bots = BotLoader.GetBotsForUser(Convert.ToInt32(_userId));
            foreach (Bot Bot in Bots)
            {
                if (!this._botItems.TryAdd(Bot.Id, Bot))
                {
                    Console.WriteLine("Error whilst loading bot x1: " + Bot.Id);
                }
            }
        }

        public void ClearItems()
        {
            UpdateItems(true);

            using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.runFastQuery("DELETE items, wired_items, user_presents, room_items_moodlight, room_items_tele_links, room_items_toner, items_groups FROM items " +
                    "LEFT JOIN wired_items ON(wired_items.id = items.id) " +
                    "LEFT JOIN user_presents ON(user_presents.item_id = items.id) " +
                    "LEFT JOIN room_items_moodlight ON(room_items_moodlight.item_id = items.id) " +
                    "LEFT JOIN room_items_tele_links ON(room_items_tele_links.tele_one_id = items.id OR room_items_tele_links.tele_two_id = items.id) " +
                    "LEFT JOIN room_items_toner ON(room_items_toner.id = items.id) " +
                    "LEFT JOIN items_groups ON(items_groups.id = items.id) " +
                    "WHERE items.room_id = '0' AND items.user_id = '" + _userId + "'");
            }

            this._floorItems.Clear();
            this._wallItems.Clear();

            if (_client != null)
                _client.SendMessage(new FurniListUpdateComposer());
        }

        public void SetIdleState()
        {
            if (_botItems != null)
                _botItems.Clear();

            if (_petsItems != null)
                _petsItems.Clear();

            if (_floorItems != null)
                _floorItems.Clear();

            if (_wallItems != null)
                _wallItems.Clear();

            _client = null;
        }


        public void UpdateItems(bool FromDatabase)
        {
            if (FromDatabase)
                Init();

            if (_client != null)
            {
                _client.SendMessage(new FurniListUpdateComposer());
            }
        }

        public Item GetItem(int Id)
        {

            if (_floorItems.ContainsKey(Id))
                return (Item)_floorItems[Id];
            else if (_wallItems.ContainsKey(Id))
                return (Item)_wallItems[Id];
            else if (_songDisks.ContainsKey(Id))
                return (Item)_songDisks[Id];

            return null;
        }

        public IEnumerable<Item> GetItems
        {
            get
            {
                return this._floorItems.Values.Concat(this._wallItems.Values);
            }
        }

        public Item AddNewItem(int Id, int BaseItem, string ExtraData, int Group, bool ToInsert, bool FromRoom, int LimitedNumber, int LimitedStack)
        {
            UpdateItems(true);

            if (ToInsert)
            {
                if (FromRoom)
                {
                    using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.runFastQuery("UPDATE `items` SET `room_id` = '0', `user_id` = '" + _userId + "' WHERE `id` = '" + Id + "' LIMIT 1");
                    }
                }
                else
                {
                    using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
                    {
                        if (Id > 0)
                            dbClient.runFastQuery("INSERT INTO `items` (`id`,`base_item`, `user_id`, `limited_number`, `limited_stack`) VALUES ('" + Id + "', '" + BaseItem + "', '" + _userId + "', '" + LimitedNumber + "', '" + LimitedStack + "')");
                        else
                        {
                            dbClient.SetQuery("INSERT INTO `items` (`base_item`, `user_id`, `limited_number`, `limited_stack`) VALUES ('" + BaseItem + "', '" + _userId + "', '" + LimitedNumber + "', '" + LimitedStack + "')");
                            Id = Convert.ToInt32(dbClient.InsertQuery());
                        }

                        SendNewItems(Convert.ToInt32(Id));

                        if (Group > 0)
                            dbClient.runFastQuery("INSERT INTO `items_groups` VALUES (" + Id + ", " + Group + ")");

                        if (!string.IsNullOrEmpty(ExtraData))
                        {
                            dbClient.SetQuery("UPDATE `items` SET `extra_data` = @extradata WHERE `id` = '" + Id + "' LIMIT 1");
                            dbClient.AddParameter("extradata", ExtraData);
                            dbClient.RunQuery();
                        }
                    }
                }
            }

            Item ItemToAdd = new Item(Id, 0, BaseItem, ExtraData, 0, 0, 0, 0, _userId, Group, LimitedNumber, LimitedStack, string.Empty);

            if (UserHoldsItem(Id))
                RemoveItem(Id);

            if (ItemToAdd.IsWallItem)
                this._wallItems.TryAdd(ItemToAdd.Id, ItemToAdd);
            else
                this._floorItems.TryAdd(ItemToAdd.Id, ItemToAdd);
            return ItemToAdd;
        }

        private bool UserHoldsItem(int itemID)
        {

            if (_floorItems.ContainsKey(itemID))
                return true;
            if (_wallItems.ContainsKey(itemID))
                return true;
            return false;
        }

        public Dictionary<int, Item> songDisks
        {
            get
            {
                Dictionary<int, Item> discs = new Dictionary<int, Item>();

                foreach (Item item in _floorItems.Values)
                {
                    if (item.GetBaseItem().InteractionType == InteractionType.MUSIC_DISC)
                    {
                        discs.Add(item.Id, item);
                    }
                }

                return discs;
            }
        }

        public void RemoveItem(int Id)
        {
            UpdateItems(true);

            if (GetClient() == null)
                return;

            if (GetClient().GetHabbo() == null || GetClient().GetHabbo().GetInventoryComponent() == null)
                GetClient().Disconnect();

            if (this._floorItems.ContainsKey(Id))
            {
                Item ToRemove = null;
                this._floorItems.TryRemove(Id, out ToRemove);
            }

            if (this._wallItems.ContainsKey(Id))
            {
                Item ToRemove = null;
                this._wallItems.TryRemove(Id, out ToRemove);
            }

            GetClient().SendMessage(new FurniListRemoveComposer(Id));
        }

        private GameClient GetClient()
        {
            return CloudServer.GetGame().GetClientManager().GetClientByUserID(_userId);
        }

        public void SendNewItems(int Id)
        {
            _client.SendMessage(new FurniListNotificationComposer(Id, 1));
        }

        #region Pet Handling
        public ICollection<Pet> GetPets()
        {
            return this._petsItems.Values;
        }

        public bool TryAddPet(Pet Pet)
        {
            //TODO: Sort this mess.
            Pet.RoomId = 0;
            Pet.PlacedInRoom = false;

            return this._petsItems.TryAdd(Pet.PetId, Pet);
        }

        public bool TryRemovePet(int PetId, out Pet PetItem)
        {
            if (this._petsItems.ContainsKey(PetId))
                return this._petsItems.TryRemove(PetId, out PetItem);
            else
            {
                PetItem = null;
                return false;
            }
        }

        public bool TryGetPet(int PetId, out Pet Pet)
        {
            if (this._petsItems.ContainsKey(PetId))
                return this._petsItems.TryGetValue(PetId, out Pet);
            else
            {
                Pet = null;
                return false;
            }
        }
        #endregion

        #region Bot Handling
        public ICollection<Bot> GetBots()
        {
            return this._botItems.Values;
        }

        public bool TryAddBot(Bot Bot)
        {
            return this._botItems.TryAdd(Bot.Id, Bot);
        }

        public bool TryRemoveBot(int BotId, out Bot Bot)
        {
            if (this._botItems.ContainsKey(BotId))
                return this._botItems.TryRemove(BotId, out Bot);
            else
            {
                Bot = null;
                return false;
            }
        }

        public bool TryGetBot(int BotId, out Bot Bot)
        {
            if (this._botItems.ContainsKey(BotId))
                return this._botItems.TryGetValue(BotId, out Bot);
            else
            {
                Bot = null;
                return false;
            }
        }
        #endregion

        public void LoadUserInventory(int InventaryId)
        {
            // dont need to update Inventary?

            InventaryUserId = InventaryId;
            UpdateItems(true);
        }

        private int GetUserInventaryId()
        {
            if (InventaryUserId > 0)
                return InventaryUserId;
            else
                return _userId;
        }

        internal Item GetFirstItemByBaseId(int id)
        {
            return _floorItems.Values.Where(item => item != null && item.GetBaseItem() != null && item.GetBaseItem().Id == id).FirstOrDefault();
        }

        public bool TryAddItem(Item item)
        {
            if (item.Data.Type.ToString().ToLower() == "s")// ItemType.FLOOR)
            {
                return this._floorItems.TryAdd(item.Id, item);
            }
            else if (item.Data.Type.ToString().ToLower() == "i")//ItemType.WALL)
            {
                return this._wallItems.TryAdd(item.Id, item);
            }
            else
            {
                throw new InvalidOperationException("Item did not match neither floor or wall item");
            }
        }

        public bool TryAddFloorItem(int itemId, Item item)
        {
            return this._floorItems.TryAdd(itemId, item);
        }
        public bool TryAddWallItem(int itemId, Item item)
        {
            return this._floorItems.TryAdd(itemId, item);
        }

        public ICollection<Item> GetFloorItems()
        {
            return this._floorItems.Values;
        }

        public ICollection<Item> GetWallItems()
        {
            return this._wallItems.Values;
        }

        public IEnumerable<Item> GetWallAndFloor
        {
            get
            {
                return this._floorItems.Values.Concat(this._wallItems.Values);
            }
        }

        public ICollection<Item> GetSongDisks()
        {
            return this._songDisks.Values;
        }
    }
}