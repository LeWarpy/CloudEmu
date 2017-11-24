using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Cloud.Communication.Packets.Outgoing.Rooms.Notifications;
using Cloud.Communication.Packets.Incoming;
using Cloud.HabboHotel.Rooms;
using Cloud.HabboHotel.Users;
using Cloud.Communication.Packets.Outgoing.Rooms.Chat;
using System.Globalization;

namespace Cloud.HabboHotel.Items.Wired.Boxes.Effects
{
    class AddScoreBox : IWiredItem, IWiredCycle
    {
        public Room Instance { get; set; }
        public Item Item { get; set; }
        public WiredBoxType Type { get { return WiredBoxType.EffectAddScore; } }
        public ConcurrentDictionary<int, Item> SetItems { get; set; }
        public string StringData { get; set; }
        public bool BoolData { get; set; }
        public int Delay { get { return this._delay; } set { this._delay = value; this.TickCount = value + 1; } }
        public int TickCount { get; set; }
        public string ItemsData { get; set; }

        private Queue _queue;
        private int _delay = 0;

        public AddScoreBox(Room Instance, Item Item)
        {
            this.Instance = Instance;
            this.Item = Item;
            this.SetItems = new ConcurrentDictionary<int, Item>();

            this._queue = new Queue();
            this.TickCount = Delay;
        }

        public void HandleSave(ClientPacket Packet)
        {
            int Unknown = Packet.PopInt();
            int score = Packet.PopInt();
            int times = Packet.PopInt();
            string Unknown2 = Packet.PopString();
            int Unknown3 = Packet.PopInt();
            int Delay = Packet.PopInt();

            this.Delay = Delay;
            this.StringData = Convert.ToString(score + ";" + times);

            // this.Delay = Packet.PopInt();
        }

        public bool OnCycle()
        {
            if (_queue.Count == 0)
            {
                this._queue.Clear();
                this.TickCount = Delay;
                return true;
            }

            while (_queue.Count > 0)
            {
                Habbo Player = (Habbo)_queue.Dequeue();
                if (Player == null || Player.CurrentRoom != Instance)
                    continue;

                this.TeleportUser(Player);
            }

            this.TickCount = Delay;
            return true;
        }

        public bool Execute(params object[] Params)
        {
            if (Params == null || Params.Length == 0)
                return false;

            Habbo Player = (Habbo)Params[0];

            if (Player == null)
                return false;

            this._queue.Enqueue(Player);
            return true;
        }

        private void TeleportUser(Habbo Player)
        {
            RoomUser User = Player.CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Player.Id);
            if (User == null)
                return;

            Room Instance = Player.CurrentRoom;

            int currentscore = 0;
            int mScore = int.Parse(StringData.Split(';')[0]) * int.Parse(StringData.Split(';')[1]);
            KeyValuePair<int, string> newkey;
            KeyValuePair<int, string> item;

            if ((Instance == null || User == null ? false : !User.IsBot))
            {
                Instance.GetRoomItemHandler().usedwiredscorebord = true;

                if (Instance.WiredScoreFirstBordInformation.Count == 3)
                {
                    Instance.GetRoomItemHandler().ScorebordChangeCheck();
                }

                if ((Instance.WiredScoreBordDay == null || Instance.WiredScoreBordMonth == null ? false : Instance.WiredScoreBordWeek != null))
                {
                    string username = User.GetClient().GetHabbo().Username;

                    lock (Instance.WiredScoreBordDay)
                    {
                        if (!Instance.WiredScoreBordDay.ContainsKey(User.UserId))
                        {
                            Instance.WiredScoreBordDay.Add(User.UserId, new KeyValuePair<int, string>(mScore, username));
                        }
                        else
                        {
                            item = Instance.WiredScoreBordDay[User.UserId];
                            currentscore = (item.Key + mScore);

                            newkey = new KeyValuePair<int, string>(currentscore, username);
                            Instance.WiredScoreBordDay[User.UserId] = newkey;
                        }
                    }

                    lock (Instance.WiredScoreBordWeek)
                    {
                        if (!Instance.WiredScoreBordWeek.ContainsKey(User.UserId))
                        {
                            Instance.WiredScoreBordWeek.Add(User.UserId, new KeyValuePair<int, string>(mScore, username));
                        }
                        else
                        {
                            item = Instance.WiredScoreBordWeek[User.UserId];
                            currentscore = (item.Key + mScore);

                            newkey = new KeyValuePair<int, string>(currentscore, username);
                            Instance.WiredScoreBordWeek[User.UserId] = newkey;
                        }
                    }

                    lock (Instance.WiredScoreBordMonth)
                    {
                        if (!Instance.WiredScoreBordMonth.ContainsKey(User.UserId))
                        {
                            Instance.WiredScoreBordMonth.Add(User.UserId, new KeyValuePair<int, string>(mScore, username));
                        }
                        else
                        {
                            item = Instance.WiredScoreBordMonth[User.UserId];
                            currentscore = (item.Key + mScore);
                            newkey = new KeyValuePair<int, string>(currentscore, username);
                            Instance.WiredScoreBordMonth[User.UserId] = newkey;
                        }
                    }
                    //Instance.GetWired().ExecuteWired(WiredItemType.TriggerScoreAchieved, User, currentscore);
                }

                Instance.GetRoomItemHandler().UpdateWiredScoreBord();
                User.GetClient().SendMessage(RoomNotificationComposer.SendBubble("award", "Has ganado " + mScore + " puntos en la clasificación. ¡Enhorabuena!", ""));

                if (Player.Effects() != null)
                    Player.Effects().ApplyEffect(0);
            }
        }
    }
}