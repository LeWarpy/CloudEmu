using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Collections.Concurrent;

using Cloud.Communication.Packets.Incoming;
using Cloud.HabboHotel.Rooms;
using Cloud.HabboHotel.Rooms.Pathfinding;

namespace Cloud.HabboHotel.Items.Wired.Boxes.Conditions
{
    class FurniHasNoFurniBox : IWiredItem
    {
        public Room Instance { get; set; }

        public Item Item { get; set; }

        public WiredBoxType Type { get { return WiredBoxType.ConditionFurniHasNoFurni; } }

        public ConcurrentDictionary<int, Item> SetItems { get; set; }

        public string StringData { get; set; }

        public bool BoolData { get; set; }

        public string ItemsData { get; set; }

        public FurniHasNoFurniBox(Room Instance, Item Item)
        {
            this.Instance = Instance;
            this.Item = Item;
            this.SetItems = new ConcurrentDictionary<int, Item>();
        }

        public void HandleSave(ClientPacket Packet)
        {
            int Unknown = Packet.PopInt();
            int Option = Packet.PopInt();
            string Unknown2 = Packet.PopString();

            this.BoolData = Option == 1;

            if (this.SetItems.Count > 0)
                this.SetItems.Clear();

            int FurniCount = Packet.PopInt();
            for (int i = 0; i < FurniCount; i++)
            {
                Item SelectedItem = Instance.GetRoomItemHandler().GetItem(Packet.PopInt());
                if (SelectedItem != null)
                    SetItems.TryAdd(SelectedItem.Id, SelectedItem);
            }
        }

        public bool Execute(params object[] Params)
        {
            return this.BoolData ? AllFurniHaveNotFurniOn() : SomeFurniHaveNotFurniOn();
        }

        public bool AllFurniHaveNotFurniOn()
        {
            foreach (Item Item in this.SetItems.Values.ToList())
            {
                if (Item == null || !Instance.GetRoomItemHandler().GetFloor.Contains(Item))
                    continue;

                bool NoFurni = false;
                List<Item> Items = Instance.GetGameMap().GetAllRoomItemForSquare(Item.GetX, Item.GetY);
                if (!(Items.Where(x => x.GetZ >= Item.GetZ).Count() > 1))
                    NoFurni = true;

                if (!NoFurni)
                    return false;
            }

            return true;
        }

        public bool SomeFurniHaveNotFurniOn()
        {
            foreach (Item Item in this.SetItems.Values.ToList())
            {
                if (Item == null || !Instance.GetRoomItemHandler().GetFloor.Contains(Item)) //Si el Furni esta en la sala
                    continue;

                bool NoFurni = false;
                foreach (String I in this.ItemsData.Split(';'))
                {
                    if (String.IsNullOrEmpty(I))
                        continue;

                    Item II = Instance.GetRoomItemHandler().GetItem(Convert.ToInt32(I));

                    if (II == null)
                        continue;

                    List<Item> Items = Instance.GetGameMap().GetAllRoomItemForSquare(II.GetX, II.GetY);

                    if (!(Items.Where(x => x.GetZ >= Item.GetZ).Count() > 1))
                    {
                        NoFurni = true;
                        break;
                    }

                }
                if (!NoFurni)
                    return false;
            }
            return true;
        }
    }
}