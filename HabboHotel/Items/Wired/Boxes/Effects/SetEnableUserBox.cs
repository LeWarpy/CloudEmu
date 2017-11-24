using System;
using System.Collections.Concurrent;
using Cloud.Communication.Packets.Incoming;
using Cloud.HabboHotel.Rooms;
using Cloud.HabboHotel.Users;
using Cloud.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Cloud.HabboHotel.Items.Wired.Boxes.Effects
{
    class SetEnableUserBox : IWiredItem
    {
        public Room Instance { get; set; }
        public Item Item { get; set; }
        public WiredBoxType Type { get { return WiredBoxType.EffectEnableUserBox; } }
        public ConcurrentDictionary<int, Item> SetItems { get; set; }
        public string StringData { get; set; }
        public bool BoolData { get; set; }
        public string ItemsData { get; set; }

        public SetEnableUserBox(Room Instance, Item Item)
        {
            this.Instance = Instance;
            this.Item = Item;
            this.SetItems = new ConcurrentDictionary<int, Item>();
        }

        public void HandleSave(ClientPacket Packet)
        {
            int Unknown = Packet.PopInt();
            string Message = Packet.PopString();

            this.StringData = Message;
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

            string Message = StringData;

            if (String.IsNullOrEmpty(StringData))
                return false;

            if (Convert.ToInt32(Message) == 102)
                return false;

            if (Convert.ToInt32(Message) == 187)
                return false;

            if (Convert.ToInt32(Message) == 189)
                return false;

            if (Convert.ToInt32(Message) == 178)
                return false;

            if (Convert.ToInt32(Message) == 188)
                return false;

            Player.GetClient().GetHabbo().Effects().ApplyEffect(Convert.ToInt32(Message));
            Player.GetClient().SendMessage(RoomNotificationComposer.SendBubble("wfeffect", "" + User.GetClient().GetHabbo().Username + ", acabas de aplicar un efecto mediante Wired en tu personaje.", ""));
            return true;
        }
    }
}