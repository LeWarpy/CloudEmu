using System.Collections.Concurrent;
using Cloud.Communication.Packets.Incoming;
using Cloud.HabboHotel.Rooms;
using Cloud.HabboHotel.Users;
using Cloud.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Cloud.HabboHotel.Items.Wired.Boxes.Effects
{
    class SetFastWalkUserBox : IWiredItem
    {
        public Room Instance { get; set; }
        public Item Item { get; set; }
        public WiredBoxType Type { get { return WiredBoxType.EffectFastWalkUserBox; } }
        public ConcurrentDictionary<int, Item> SetItems { get; set; }
        public string StringData { get; set; }
        public bool BoolData { get; set; }
        public string ItemsData { get; set; }

        public SetFastWalkUserBox(Room Instance, Item Item)
        {
            this.Instance = Instance;
            this.Item = Item;

            this.SetItems = new ConcurrentDictionary<int, Item>();
        }

        public void HandleSave(ClientPacket Packet)
        {
            int Unknown = Packet.PopInt();
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

            User.FastWalking = !User.FastWalking;

            if (User.SuperFastWalking)
                User.SuperFastWalking = false;

            if (!User.FastWalking)
            {
                Player.GetClient().SendMessage(RoomNotificationComposer.SendBubble("wffwalk", "" + User.GetClient().GetHabbo().Username + ", acabas de desactivar la hiperactividad mediante Wired.", ""));
            }
            else
            {
                Player.GetClient().SendMessage(RoomNotificationComposer.SendBubble("wffwalk", "" + User.GetClient().GetHabbo().Username + ", acabas de activar la hiperactividad mediante Wired, ve con cuidado, ahora vas más rápido que la luz.", ""));
            }
            return true;
        }
    }
}