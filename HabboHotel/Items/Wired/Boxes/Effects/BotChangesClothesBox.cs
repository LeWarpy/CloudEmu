﻿using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;

using Cloud.HabboHotel.Rooms;
using Cloud.HabboHotel.Users;
using Cloud.Communication.Packets.Incoming;
using Cloud.Communication.Packets.Outgoing;
using Cloud.Database.Interfaces;

namespace Cloud.HabboHotel.Items.Wired.Boxes.Effects
{
    class BotChangesClothesBox : IWiredItem
    {
        public Room Instance { get; set; }
        public Item Item { get; set; }
        public WiredBoxType Type { get { return WiredBoxType.EffectBotChangesClothesBox; } }
        public ConcurrentDictionary<int, Item> SetItems { get; set; }
        public string StringData { get; set; }
        public bool BoolData { get; set; }
        public string ItemsData { get; set; }

        public BotChangesClothesBox(Room Instance, Item Item)
        {
            this.Instance = Instance;
            this.Item = Item;
            this.SetItems = new ConcurrentDictionary<int, Item>();
        }

        public void HandleSave(ClientPacket Packet)
        {
            int Unknown = Packet.PopInt();
            string BotConfiguration = Packet.PopString();

            if (this.SetItems.Count > 0)
                this.SetItems.Clear();

            this.StringData = BotConfiguration;
        }

        public bool Execute(params object[] Params)
        {
            if (Params == null || Params.Length == 0)
                return false;

            if (String.IsNullOrEmpty(this.StringData))
                return false;


            string[] Stuff = this.StringData.Split('\t');
            if (Stuff.Length != 2)
                return false;//This is important, incase a cunt scripts.

            string Username = Stuff[0];

            RoomUser User = this.Instance.GetRoomUserManager().GetBotByName(Username);
            if (User == null)
                return false;      
            
            string Figure = Stuff[1];

            ServerPacket UserChangeComposer = new ServerPacket(ServerPacketHeader.UserChangeMessageComposer);
            UserChangeComposer.WriteInteger(User.VirtualId);
            UserChangeComposer.WriteString(Figure);
            UserChangeComposer.WriteString("M");
            UserChangeComposer.WriteString(User.BotData.Motto);
            UserChangeComposer.WriteInteger(0);
            this.Instance.SendMessage(UserChangeComposer);

            User.BotData.Look = Figure;
            User.BotData.Gender = "M";

            using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `bots` SET `look` = @look, `gender` = '" + User.BotData.Gender + "' WHERE `id` = '" + User.BotData.Id + "' LIMIT 1");
                dbClient.AddParameter("look", User.BotData.Look);
                dbClient.RunQuery();
            }

            return true;
        }
    }
}