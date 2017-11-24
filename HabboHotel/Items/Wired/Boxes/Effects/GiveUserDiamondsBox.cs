using System;
using System.Collections.Concurrent;
using Cloud.Communication.Packets.Outgoing.Inventory.Purse;
using Cloud.Communication.Packets.Outgoing.Rooms.Notifications;
using Cloud.Communication.Packets.Incoming;
using Cloud.HabboHotel.Rooms;
using Cloud.HabboHotel.Users;

namespace Cloud.HabboHotel.Items.Wired.Boxes.Effects
{
    class GiveUserDiamondsBox : IWiredItem
    {
        public Room Instance { get; set; }
        public Item Item { get; set; }
        public WiredBoxType Type { get { return WiredBoxType.EffectGiveUserDiamondsBox; } }
        public ConcurrentDictionary<int, Item> SetItems { get; set; }
        public string StringData { get; set; }
        public bool BoolData { get; set; }
        public string ItemsData { get; set; }

        public GiveUserDiamondsBox(Room Instance, Item Item)
        {
            this.Instance = Instance;
            this.Item = Item;
            this.SetItems = new ConcurrentDictionary<int, Item>();
        }

        public void HandleSave(ClientPacket Packet)
        {
            int Unknown = Packet.PopInt();
            string Coin = Packet.PopString();

            this.StringData = Coin;
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

            int Amount;
            Amount = Convert.ToInt32(StringData);
            if (Amount > 500)
            {
                Player.GetClient().SendWhisper("La cantidad de Diamantes pasa de los limites.");
                return false;
            }
            else
            {
                Player.GetClient().GetHabbo().Diamonds += Amount;
                Player.GetClient().SendMessage(new HabboActivityPointNotificationComposer(Player.GetClient().GetHabbo().Diamonds, Amount, 5));
                Player.GetClient().SendMessage(new RoomNotificationComposer("command_notification_credits", "message", "Se te a dado " + Amount + " diamante(s)!"));
            }

            return true;
        }
    }
}