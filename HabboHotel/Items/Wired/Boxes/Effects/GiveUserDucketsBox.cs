using System;
using System.Collections.Concurrent;
using Cloud.Communication.Packets.Outgoing.Inventory.Purse;
using Cloud.Communication.Packets.Outgoing.Rooms.Notifications;
using Cloud.Communication.Packets.Incoming;
using Cloud.HabboHotel.Rooms;
using Cloud.HabboHotel.Users;

namespace Cloud.HabboHotel.Items.Wired.Boxes.Effects
{
    class GiveUserDucketsBox : IWiredItem
    {
        public Room Instance { get; set; }
        public Item Item { get; set; }
        public WiredBoxType Type { get { return WiredBoxType.EffectGiveUserDucketsBox; } }
        public ConcurrentDictionary<int, Item> SetItems { get; set; }
        public string StringData { get; set; }
        public bool BoolData { get; set; }
        public string ItemsData { get; set; }

        public GiveUserDucketsBox(Room Instance, Item Item)
        {
            this.Instance = Instance;
            this.Item = Item;
            this.SetItems = new ConcurrentDictionary<int, Item>();
        }

        public void HandleSave(ClientPacket Packet)
        {
            int Unknown = Packet.PopInt();
            string Ducket = Packet.PopString();

            this.StringData = Ducket;
        }

        public bool Execute(params object[] Params)
        {
            if (Params == null || Params.Length == 0)
                return false;

            Habbo Owner = CloudServer.GetHabboById(Item.UserID);
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
                Player.GetClient().SendWhisper("La cantidad de Duckets pasa de los limites.");
                return false;
            }
            else
            {
            string Ducket = StringData;

            Player.Duckets = Player.Duckets += Convert.ToInt32(Ducket);
            Player.GetClient().SendMessage(new HabboActivityPointNotificationComposer(Player.Duckets, Convert.ToInt32(Ducket)));
            Player.GetClient().SendMessage(new RoomNotificationComposer("command_notification_credits", "message", "Has recibido " + Ducket + " ducket(s)!"));
            return true;
			}
        }
    }
}