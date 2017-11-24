using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;

using Cloud.HabboHotel.Rooms;
using Cloud.HabboHotel.Users;
using Cloud.Communication.Packets.Incoming;

namespace Cloud.HabboHotel.Items.Wired.Boxes.Effects
{
    class BotCommunicatesToAllBox : IWiredItem
    {
        public Room Instance { get; set; }
        public Item Item { get; set; }
        public WiredBoxType Type { get { return WiredBoxType.EffectBotCommunicatesToAllBox; } }
        public ConcurrentDictionary<int, Item> SetItems { get; set; }
        public string StringData { get; set; }
        public bool BoolData { get; set; }
        public string ItemsData { get; set; }

        public BotCommunicatesToAllBox(Room Instance, Item Item)
        {
            this.Instance = Instance;
            this.Item = Item;
            this.SetItems = new ConcurrentDictionary<int, Item>();
        }

        public void HandleSave(ClientPacket Packet)
        {
            int Unknown = Packet.PopInt();
            int ChatMode = Packet.PopInt();
            string ChatConfig = Packet.PopString();

            this.StringData = ChatConfig;
            if (ChatMode == 1)
            {
                this.BoolData = true;
            }
            else
            {
                this.BoolData = false;
            }

        }

        public bool Execute(params object[] Params)
        {
            if (Params == null || Params.Length == 0)
                return false;

            if (String.IsNullOrEmpty(this.StringData))
                return false;

            this.StringData.Split(' ');

            string BotName = this.StringData.Split('	')[0];
            string Chat = this.StringData.Split('	')[1];

            RoomUser User = this.Instance.GetRoomUserManager().GetBotByName(BotName);
            if (User == null)
                return false;

            if (this.BoolData == true)
            {
                User.Chat(Chat, true, 31);
            }
            else
            {
                User.Chat(Chat, false, 31);
            }


            return true;
        }
    }
}