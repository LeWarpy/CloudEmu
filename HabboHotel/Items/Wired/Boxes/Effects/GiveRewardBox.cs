using System;
using System.Collections.Concurrent;

using Cloud.Communication.Packets.Incoming;
using Cloud.HabboHotel.Rooms;
using Cloud.HabboHotel.Users;
using Cloud.Communication.Packets.Outgoing.Rooms.Chat;
using Cloud.Communication.Packets.Outgoing.Inventory.Furni;
using Cloud.Communication.Packets.Outgoing.Catalog;
using Cloud.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Cloud.HabboHotel.Items.Wired.Boxes.Effects
{
    class GiveRewardBox : IWiredItem
    {
        public Room Instance { get; set; }
        public Item Item { get; set; }
        public WiredBoxType Type { get { return WiredBoxType.EffectGiveReward; } }
        public ConcurrentDictionary<int, Item> SetItems { get; set; }
        public string StringData { get; set; }
        public bool BoolData { get; set; }
        public string ItemsData { get; set; }

        public GiveRewardBox(Room Instance, Item Item)
        {
            this.Instance = Instance;
            this.Item = Item;
            SetItems = new ConcurrentDictionary<int, Item>();
        }

        public void HandleSave(ClientPacket Packet)
        {
            int Unknown = Packet.PopInt();
            int Often = Packet.PopInt();
            bool Unique = (Packet.PopInt() == 1);
            int Limit = Packet.PopInt();
            int Often_No = Packet.PopInt();
            string Reward = Packet.PopString();

            BoolData = Unique;
            StringData = Reward + "-" + Often + "-" + Limit;
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

            int amountLeft = int.Parse(StringData.Split('-')[2]);
            int often = int.Parse(StringData.Split('-')[1]);
            bool unique = BoolData;

            bool premied = false;

            if (amountLeft == 1)
            {
                Player.GetClient().SendNotification("Ya no hay mas premios, vuelve mas tarde.");
                return true;
            }

            foreach (var dataStr in (StringData.Split('-')[0]).Split(';'))
            {
                var dataArray = dataStr.Split(',');

                var isbadge = dataArray[0] == "0";
                var code = dataArray[1];
                var percentage = int.Parse(dataArray[2]);

                var random = CloudServer.GetRandomNumber(0, 100);

                if (!unique && percentage < random)
                    continue;
                premied = true;

                if (isbadge)
                {
                    if (Player.GetBadgeComponent().HasBadge(code))
                        Player.GetClient().SendMessage(new WhisperComposer(User.VirtualId, "Oops,Parece que já recebeu este emblema !", 0, User.LastBubble));
                    else
                    {
                        Player.GetBadgeComponent().GiveBadge(code, true, Player.GetClient());
                        Player.GetClient().SendMessage(RoomNotificationComposer.SendBubble("badge/" + Params[2], "Acaba de receber um emblema!", "/inventory/open/badge"));
                    }
                }
                else
                {
                    ItemData ItemData = null;

                    if (!CloudServer.GetGame().GetItemManager().GetItem(int.Parse(code), out ItemData))
                    {
                        Player.GetClient().SendMessage(new WhisperComposer(User.VirtualId, "No se pudo obtener Item ID: " + code, 0, User.LastBubble));
                        return false;
                    }

                    Item Item = ItemFactory.CreateSingleItemNullable(ItemData, Player.GetClient().GetHabbo(), "", "", 0, 0, 0);


                    if (Item != null)
                    {
                        Player.GetClient().GetHabbo().GetInventoryComponent().TryAddItem(Item);
                        Player.GetClient().SendMessage(new FurniListNotificationComposer(Item.Id, 1));
                        Player.GetClient().SendMessage(new PurchaseOKComposer());
                        Player.GetClient().SendMessage(new FurniListAddComposer(Item));
                        Player.GetClient().SendMessage(new FurniListUpdateComposer());
                        Player.GetClient().SendNotification("¡Has recibido un regalo! Revisa tu inventario.");
                    }
                }
            }

            if (!premied)
            {
                Player.GetClient().SendNotification("Suerte la proxima vez :(");
            }
            else if (amountLeft > 1)
            {
                amountLeft--;
                this.StringData.Split('-')[2] = amountLeft.ToString();
            }

            return true;
        }
    }
}