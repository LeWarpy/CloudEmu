using System;
using System.Collections.Concurrent;
using Cloud.Communication.Packets.Outgoing.Rooms.Avatar;
using Cloud.Communication.Packets.Incoming;
using Cloud.HabboHotel.Rooms;
using Cloud.HabboHotel.Users;
using Cloud.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Cloud.HabboHotel.Items.Wired.Boxes.Effects
{
    class SetDanceUserBox : IWiredItem
    {
        public Room Instance { get; set; }
        public Item Item { get; set; }
        public WiredBoxType Type { get { return WiredBoxType.EffectDanceUserBox; } }
        public ConcurrentDictionary<int, Item> SetItems { get; set; }
        public string StringData { get; set; }
        public bool BoolData { get; set; }
        public string ItemsData { get; set; }

        public SetDanceUserBox(Room Instance, Item Item)
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

            RoomUser ThisUser = Player.CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Player.Id);
            if (ThisUser == null)
                return false;

            string Dance = StringData;

            if (String.IsNullOrEmpty(StringData))
                return false;

            Player.GetClient().SendMessage(new DanceComposer(ThisUser, Convert.ToInt32(Dance)));
            Player.GetClient().SendMessage(RoomNotificationComposer.SendBubble("wfdance", "" + Player.GetClient().GetHabbo().Username + ", acabas de activar el baile " + StringData + " mediante Wired.", ""));
            return true;
        }
    }
}