﻿using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Cloud.Communication.Packets.Outgoing.Moderation;
using Cloud.Communication.Packets.Incoming;
using Cloud.HabboHotel.Rooms;
using Cloud.HabboHotel.Users;
using Cloud.Communication.Packets.Outgoing.Rooms.Chat;

namespace Cloud.HabboHotel.Items.Wired.Boxes.Effects
{
    class MuteTriggererBox : IWiredItem
    {
        public Room Instance { get; set; }
        public Item Item { get; set; }
        public WiredBoxType Type { get { return WiredBoxType.EffectMuteTriggerer; } }
        public ConcurrentDictionary<int, Item> SetItems { get; set; }
        public string StringData { get; set; }
        public bool BoolData { get; set; }
        public string ItemsData { get; set; }

        public MuteTriggererBox(Room Instance, Item Item)
        {
            this.Instance = Instance;
            this.Item = Item;
            this.SetItems = new ConcurrentDictionary<int, Item>();

            if (this.SetItems.Count > 0)
                this.SetItems.Clear();
        }

        public void HandleSave(ClientPacket Packet)
        {
            if (this.SetItems.Count > 0)
                this.SetItems.Clear();

            int Unknown = Packet.PopInt();
            int Time = Packet.PopInt();
            string Message = Packet.PopString();

            this.StringData = Time + ";" + Message;
        }

        public bool Execute(params object[] Params)
        {
            if (Params.Length != 1)
                return false;

            Habbo Player = (Habbo)Params[0];
            if (Player == null)
                return false;

            RoomUser User = Instance.GetRoomUserManager().GetRoomUserByHabbo(Player.Id);
            if (User == null)
                return false;

            if (Player.GetPermissions().HasRight("mod_tool") || this.Instance.OwnerId == Player.Id)
            {
                Player.GetClient().SendMessage(new WhisperComposer(User.VirtualId, "Wired Mute Exception: Unmutable Player", 0, 0));
                return false;
            }

            int Time = (StringData != null ? int.Parse(StringData.Split(';')[0]) : 0);
            string Message = (StringData != null ? (StringData.Split(';')[1]) : "No message!");

            if (Time > 0)
            {
                Player.GetClient().SendMessage(new WhisperComposer(User.VirtualId, "Wired Mute: Muted for " + Time + "! Message: " + Message, 0, 0));
                if (!Instance.MutedUsers.ContainsKey(Player.Id))
                    Instance.MutedUsers.Add(Player.Id, (CloudServer.GetUnixTimestamp() + (Time * 60)));
                else
                {
                    Instance.MutedUsers.Remove(Player.Id);
                    Instance.MutedUsers.Add(Player.Id, (CloudServer.GetUnixTimestamp() + (Time * 60)));
                }
            }

            return true;
        }
    }
}