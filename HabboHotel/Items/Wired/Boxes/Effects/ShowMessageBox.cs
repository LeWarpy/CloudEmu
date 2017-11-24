using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Collections.Concurrent;

using Cloud.Communication.Packets.Incoming;
using Cloud.HabboHotel.Rooms;
using Cloud.HabboHotel.Users;
using Cloud.Communication.Packets.Outgoing.Rooms.Chat;

namespace Cloud.HabboHotel.Items.Wired.Boxes.Effects
{
    class ShowMessageBox : IWiredItem
    {
        public Room Instance { get; set; }

        public Item Item { get; set; }

        public WiredBoxType Type { get { return WiredBoxType.EffectShowMessage; } }

        public ConcurrentDictionary<int, Item> SetItems { get; set; }

        public string StringData { get; set; }

        public bool BoolData { get; set; }

        public string ItemsData { get; set; }

        public ShowMessageBox(Room Instance, Item Item)
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
            if (Player == null || Player.GetClient() == null || string.IsNullOrWhiteSpace(StringData))
                return false;

            RoomUser User = Player.CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Player.Username);
            if (User == null)
                return false;

            string Message = StringData;

            if (StringData.Contains("%USERNAME%"))
                Message = Message.Replace("%USERNAME%", Player.Username);

            if (StringData.Contains("%ROOMNAME%"))
                Message = Message.Replace("%ROOMNAME%", Player.CurrentRoom.Name);

            if (StringData.Contains("%USERCOUNT%"))
                Message = Message.Replace("%USERCOUNT%", Player.CurrentRoom.UserCount.ToString());

            if (StringData.Contains("%USERSONLINE%"))
                Message = Message.Replace("%USERSONLINE%", CloudServer.GetGame().GetClientManager().Count.ToString());

            if (StringData.Contains("%USERID%"))
                Message = Message.Replace("%USERID%", Convert.ToString(Player.Id));

            if (StringData.Contains("%HONOR%"))
                Message = Message.Replace("%HONOR%", Convert.ToString(Player.GOTWPoints));

            if (StringData.Contains("%DUCKETS%"))
                Message = Message.Replace("%DUCKETS%", Convert.ToString(Player.Duckets));

            if (StringData.Contains("%DIAMONDS%"))
                Message = Message.Replace("%DIAMONDS%", Convert.ToString(Player.Diamonds));

            if (StringData.Contains("%RANGO%")) // Put names not number
                Message = Message.Replace("%RANGO%", Convert.ToString(Player.Rank));

            if (StringData.Contains("%LIKESROOM%"))
                Message = Message.Replace("%LIKESROOM%", Player.CurrentRoom.Score.ToString());


            

            Player.GetClient().SendMessage(new WhisperComposer(User.VirtualId, Message, 0, 34));
            return true;
        }
    }
}