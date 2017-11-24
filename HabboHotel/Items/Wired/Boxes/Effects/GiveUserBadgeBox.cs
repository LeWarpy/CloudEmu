﻿using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Collections.Concurrent;

using Cloud.Communication.Packets.Incoming;
using Cloud.HabboHotel.Rooms;
using Cloud.HabboHotel.Users;
using Cloud.Communication.Packets.Outgoing.Rooms.Chat;
using Cloud.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Cloud.HabboHotel.Items.Wired.Boxes.Effects
{
    class GiveUserBadgeBox : IWiredItem
    {
        public Room Instance { get; set; }

        public Item Item { get; set; }

        public WiredBoxType Type { get { return WiredBoxType.EffectGiveUserBadge; } }

        public ConcurrentDictionary<int, Item> SetItems { get; set; }

        public string StringData { get; set; }

        public bool BoolData { get; set; }

        public string ItemsData { get; set; }

        public GiveUserBadgeBox(Room Instance, Item Item)
        {
            this.Instance = Instance;
            this.Item = Item;
            this.SetItems = new ConcurrentDictionary<int, Item>();
        }

        public void HandleSave(ClientPacket Packet)
        {
            int Unknown = Packet.PopInt();
            string Badge = Packet.PopString();

            this.StringData = Badge;
        }

        public bool Execute(params object[] Params)
        {
            if (Params == null || Params.Length == 0)
                return false;

            Habbo Owner = CloudServer.GetHabboById(Item.UserID);
            if (Owner == null || !Owner.GetPermissions().HasRight("room_item_wired_rewards"))
                return false;

            Habbo Player = (Habbo)Params[0];
            if (Player == null || Player.GetClient() == null)
                return false;

            RoomUser User = Player.CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Player.Username);
            if (User == null)
                return false;

            if (String.IsNullOrEmpty(StringData))
                return false;

            if (Player.GetBadgeComponent().HasBadge(StringData))
                Player.GetClient().SendNotification("Parece que você já tem esse emblema no inventário .");
            else
            {
                Player.GetBadgeComponent().GiveBadge(StringData, true, Player.GetClient());
                Player.GetClient().SendMessage(RoomNotificationComposer.SendBubble("badge/" + Params[2], "Acaba de receber um emblema!", "/inventory/open/badge"));
            }

            return true;
        }
    }
}