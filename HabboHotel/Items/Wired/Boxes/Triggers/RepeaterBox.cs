﻿using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Collections.Concurrent;

using Cloud.Communication.Packets.Incoming;
using Cloud.HabboHotel.Rooms;
using Cloud.HabboHotel.Users;

namespace Cloud.HabboHotel.Items.Wired.Boxes.Triggers
{
    class RepeaterBox : IWiredItem, IWiredCycle
    {
        public Room Instance { get; set; }
        public Item Item { get; set; }
        public WiredBoxType Type { get { return WiredBoxType.TriggerRepeat; } }
        public ConcurrentDictionary<int, Item> SetItems { get; set; }
        public string StringData { get; set; }
        public bool BoolData { get; set; }
        public int Delay { get { return this._delay; } set { this._delay = value; this.TickCount = value; } }
        public int TickCount { get; set; }
        public string ItemsData { get; set; }

        private int _delay = 0;

        public RepeaterBox(Room Instance, Item Item)
        {
            this.Instance = Instance;
            this.Item = Item;
            this.SetItems = new ConcurrentDictionary<int, Item>();
        }

        public void HandleSave(ClientPacket Packet)
        {
            int Unknown = Packet.PopInt();
            int Delay = Packet.PopInt();

            this.Delay = Delay;
            this.TickCount = Delay;
        }

        public bool Execute(params object[] Params)
        {
            return true;
        }

        public bool OnCycle()
        {
            bool Success = false;
            ICollection<RoomUser> Avatars = Instance.GetRoomUserManager().GetRoomUsers().ToList();
            ICollection<IWiredItem> Effects = Instance.GetWired().GetEffects(this);
            ICollection<IWiredItem> Conditions = Instance.GetWired().GetConditions(this);

            foreach (IWiredItem Condition in Conditions.ToList())
            {
                foreach (RoomUser Avatar in Avatars.ToList())
                {
                    if (Avatar == null || Avatar.GetClient() == null || Avatar.GetClient().GetHabbo() == null)
                        continue;

                    if (!Condition.Execute(Avatar.GetClient().GetHabbo()))
                        continue;

                    Success = true;
                }

                if (!Success)
                    return false;

                Success = false;
                Instance.GetWired().OnEvent(Condition.Item);
            }

            Success = false;

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
                foreach (IWiredItem Effect in Effects.ToList())
                {
                    if (!Effect.Execute())
                        continue;

                    Success = true;

                    if (!Success)
                        return false;

                    if (Instance != null)
                        Instance.GetWired().OnEvent(Effect.Item);
                }
            }

            this.TickCount = Delay;

            return true;
        }
    }
}