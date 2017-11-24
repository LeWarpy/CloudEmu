﻿using System;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;

using Cloud.HabboHotel.Rooms;
using Cloud.HabboHotel.Users;
using Cloud.Communication.Packets.Incoming;

namespace Cloud.HabboHotel.Items.Wired.Boxes.Effects
{
    class BotMovesToFurniBox : IWiredItem
    {
        public Room Instance { get; set; }
        public Item Item { get; set; }
        public WiredBoxType Type { get { return WiredBoxType.EffectBotMovesToFurniBox; } }
        public ConcurrentDictionary<int, Item> SetItems { get; set; }
        public string StringData { get; set; }
        public bool BoolData { get; set; }
        public string ItemsData { get; set; }

        public BotMovesToFurniBox(Room Instance, Item Item)
        {
            this.Instance = Instance;
            this.Item = Item;
            this.SetItems = new ConcurrentDictionary<int, Item>();
        }

        public void HandleSave(ClientPacket Packet)
        {
            int Unknown = Packet.PopInt();
            string BotName = Packet.PopString();

            if (this.SetItems.Count > 0)
                this.SetItems.Clear();

            int FurniCount = Packet.PopInt();
            for (int i = 0; i < FurniCount; i++)
            {
                Item SelectedItem = Instance.GetRoomItemHandler().GetItem(Packet.PopInt());
                if (SelectedItem != null)
                    SetItems.TryAdd(SelectedItem.Id, SelectedItem);
            }

            this.StringData = BotName;
        }

        public bool Execute(params object[] Params)
        {
            if (Params == null || Params.Length == 0 || String.IsNullOrEmpty(this.StringData))
                return false;

            RoomUser User = this.Instance.GetRoomUserManager().GetBotByName(this.StringData);
            if (User == null)
                return false;

            Random rand = new Random();
            List<Item> Items = SetItems.Values.ToList();
            Items = Items.OrderBy(x => rand.Next()).ToList();

            if (Items.Count == 0)
                return false;

            Item Item = Items.First();
            if (Item == null)
                return false;

            if (!Instance.GetRoomItemHandler().GetFloor.Contains(Item))
            {
                SetItems.TryRemove(Item.Id, out Item);

                if (Items.Contains(Item))
                    Items.Remove(Item);

                if (SetItems.Count == 0 || Items.Count == 0)
                    return false;

                Item = Items.First();
                if (Item == null)
                    return false;
            }

            if (this.Instance.GetGameMap() == null)
                return false;

            if (User.IsWalking)
            {
                User.ClearMovement(true);
            }

            User.BotData.ForcedMovement = true;
            User.BotData.TargetCoordinate = new Point(Item.GetX, Item.GetY);
            User.MoveTo(Item.GetX, Item.GetY);

            return true;
        }
    }
}