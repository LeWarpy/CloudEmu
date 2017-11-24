﻿using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;

using Cloud.Communication.Packets.Incoming;
using Cloud.HabboHotel.Rooms;
using Cloud.HabboHotel.Users;
using Cloud.Communication.Packets.Outgoing.Rooms.Chat;

namespace Cloud.HabboHotel.Items.Wired.Boxes.Effects
{
    class TeleportUserBox : IWiredItem, IWiredCycle
    {
        public Room Instance { get; set; }
        public Item Item { get; set; }
        public WiredBoxType Type { get { return WiredBoxType.EffectTeleportToFurni; } }
        public ConcurrentDictionary<int, Item> SetItems { get; set; }
        public string StringData { get; set; }
        public bool BoolData { get; set; }
        public int Delay { get { return this._delay; } set { this._delay = value; this.TickCount = value + 1; } }
        public int TickCount { get; set; }
        public string ItemsData { get; set; }

        private Queue _queue;
        private int _delay = 0;

        public TeleportUserBox(Room Instance, Item Item)
        {
            this.Instance = Instance;
            this.Item = Item;
            this.SetItems = new ConcurrentDictionary<int, Item>();

            this._queue = new Queue();
            this.TickCount = Delay;
        }

        public void HandleSave(ClientPacket Packet)
        {
            int Unknown = Packet.PopInt();
            string Unknown2 = Packet.PopString();

            if (this.SetItems.Count > 0)
                this.SetItems.Clear();

            int FurniCount = Packet.PopInt();
            for (int i = 0; i < FurniCount; i++)
            {
                Item SelectedItem = Instance.GetRoomItemHandler().GetItem(Packet.PopInt());
                if (SelectedItem != null)
                    SetItems.TryAdd(SelectedItem.Id, SelectedItem);
            }

            this.Delay = Packet.PopInt();
        }

        public bool OnCycle()
        {
            if (_queue.Count == 0 || SetItems.Count == 0)
            {
                this._queue.Clear();
                this.TickCount = Delay;
                return true;
            }

            while (_queue.Count > 0)
            {
                Habbo Player = (Habbo)_queue.Dequeue();
                if (Player == null || Player.CurrentRoom != Instance)
                    continue;

                this.TeleportUser(Player);
            }

            this.TickCount = Delay;
            return true;
        }

        public bool Execute(params object[] Params)
        {
            if (Params == null || Params.Length == 0)
                return false;

            Habbo Player = (Habbo)Params[0];

            if (Player == null)
                return false;

            if (Player.Effects() != null)
                Player.Effects().ApplyEffect(4);

            this._queue.Enqueue(Player);
            return true;
        }

        private void TeleportUser(Habbo Player)
        {
            if (Player == null)
                return;

            Room Room = Player.CurrentRoom;
            if (Room == null)
                return;

            RoomUser User = Player.CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Player.Username);
            if (User == null)
                return;

            if (Player.IsTeleporting || Player.IsHopping || Player.TeleporterId != 0)
                return;

            Random rand = new Random();
            List<Item> Items = SetItems.Values.ToList();
            Items = Items.OrderBy(x => rand.Next()).ToList();

            if (Items.Count == 0)
                return;

            Item Item = Items.First();
            if (Item == null)
                return;

            if (!Instance.GetRoomItemHandler().GetFloor.Contains(Item))
            {
                SetItems.TryRemove(Item.Id, out Item);

                if (Items.Contains(Item))
                Items.Remove(Item);

                if (SetItems.Count == 0 || Items.Count == 0)
                    return;

                Item = Items.First();
                if (Item == null)
                    return;
            }

            if (Room.GetGameMap() == null)
                return;

            Room.GetGameMap().TeleportToItem(User, Item);
            Room.GetRoomUserManager().UpdateUserStatusses();

            if (Player.Effects() != null)
                Player.Effects().ApplyEffect(0);
        }
    }
}
