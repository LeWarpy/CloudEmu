using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Cloud.Communication.Packets.Outgoing.Rooms.Notifications;
using Cloud.Communication.Packets.Incoming;
using Cloud.HabboHotel.Rooms;
using Cloud.HabboHotel.Users;
using Cloud.Communication.Packets.Outgoing.Rooms.Chat;

namespace Cloud.HabboHotel.Items.Wired.Boxes.Effects
{
    class HandUserItemBox : IWiredItem
    {
        public Room Instance { get; set; }
        public Item Item { get; set; }
        public WiredBoxType Type { get { return WiredBoxType.EffectHandUserItemBox; } }
        public ConcurrentDictionary<int, Item> SetItems { get; set; }
        public string StringData { get; set; }
        public bool BoolData { get; set; }
        public string ItemsData { get; set; }

        public HandUserItemBox(Room Instance, Item Item)
        {
            this.Instance = Instance;
            this.Item = Item;
            this.SetItems = new ConcurrentDictionary<int, Item>();
        }

        public void HandleSave(ClientPacket Packet)
        {
            int Unknown = Packet.PopInt();
            string HandI = Packet.PopString();

            this.StringData = HandI;
        }

        public bool Execute(params object[] Params)
        {
            if (Params == null || Params.Length == 0)
                return false;

           
            Habbo Player = (Habbo)Params[0];
            if (Player == null || Player.GetClient() == null)
                return false;

            RoomUser User = Player.CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Player.Username);
            if (User == null)
                return false;

            if (String.IsNullOrEmpty(StringData))
                return false;

            string HandI = StringData;

            User.CarryItem(Convert.ToInt32(HandI));
            Player.GetClient().SendMessage(RoomNotificationComposer.SendBubble("wfhanditem", "" + User.GetClient().GetHabbo().Username + ", acabas de recibir una bebida u otro objeto similar por un efecto de Wired.", ""));

            return true;
        }
    }
}