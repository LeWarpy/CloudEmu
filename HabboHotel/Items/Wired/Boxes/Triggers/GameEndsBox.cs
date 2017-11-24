﻿
using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;

using Cloud.Communication.Packets.Incoming;
using Cloud.HabboHotel.Rooms;

namespace Cloud.HabboHotel.Items.Wired.Boxes.Triggers
{
    class GameEndsBox : IWiredItem
    {
        public Room Instance { get; set; }
        public Item Item { get; set; }
        public WiredBoxType Type { get { return WiredBoxType.TriggerGameEnds; } }
        public ConcurrentDictionary<int, Item> SetItems { get; set; }
        public string StringData { get; set; }
        public bool BoolData { get; set; }
        public string ItemsData { get; set; }

        public GameEndsBox(Room Instance, Item Item)
        {
            this.Item = Item;
            this.Instance = Instance;
            this.SetItems = new ConcurrentDictionary<int, Item>();
        }

        public void HandleSave(ClientPacket Packet)
        {

        }

        public bool Execute(params object[] Params)
        {
            ICollection<IWiredItem> Effects = Instance.GetWired().GetEffects(this);
            ICollection<IWiredItem> Conditions = Instance.GetWired().GetConditions(this);

            foreach (IWiredItem Condition in Conditions)
            {
                if (!Condition.Execute(Condition.Item))
                    return false;

                Instance.GetWired().OnEvent(Condition.Item);
            }

            //Check the ICollection to find the random addon effect.
            bool HasRandomEffectAddon = Effects.Where(x => x.Type == WiredBoxType.AddonRandomEffect).ToList().Count() > 0;
            if (HasRandomEffectAddon)
            {
                //Okay, so we have a random addon effect, now lets get the IWiredItem and attempt to execute it.
                IWiredItem RandomBox = Effects.FirstOrDefault(x => x.Type == WiredBoxType.AddonRandomEffect);
                if (!RandomBox.Execute())
                    return false;

                //Success! Let's get our selected box and continue.
                IWiredItem SelectedBox = Instance.GetWired().GetRandomEffect(Effects.ToList());
                if (!SelectedBox.Execute())
                    return false;

                //Woo! Almost there captain, now lets broadcast the update to the room instance.
                if (Instance != null)
                {
                    Instance.GetWired().OnEvent(RandomBox.Item);
                    Instance.GetWired().OnEvent(SelectedBox.Item);
                }
            }
            else
            {
                foreach (IWiredItem Effect in Effects)
                {
                    foreach (RoomUser User in Instance.GetRoomUserManager().GetRoomUsers().ToList())
                    {
                        if (User == null || User.GetClient() == null || User.GetClient().GetHabbo() == null)
                            continue;

                        Effect.Execute(User.GetClient().GetHabbo());
                    }

                    Instance.GetWired().OnEvent(Effect.Item);
                }
            }

            return true;
        }
    }
}