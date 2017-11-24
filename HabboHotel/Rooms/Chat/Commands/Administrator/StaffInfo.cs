using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Cloud.HabboHotel.Rooms;
using Cloud.HabboHotel.Items;
using Cloud.Communication.Packets.Outgoing.Inventory.Furni;
using Cloud.Database.Interfaces;
using Cloud.HabboHotel.Users;
using Cloud.HabboHotel.GameClients;
using Cloud.Communication.Packets.Outgoing.Notifications;

namespace Cloud.HabboHotel.Rooms.Chat.Commands.User
{
    class StaffInfo : IChatCommand
    {
        public string PermissionRequired => "command_staffinfo";
        public string Parameters => "";
        public string Description => "Ver lista de staffs online";

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            Dictionary<Habbo, UInt32> clients = new Dictionary<Habbo, UInt32>();

            StringBuilder content = new StringBuilder();
            content.Append("Estado de Staff conectados em " + CloudServer.HotelName + ":\r\n");

            foreach (var client in CloudServer.GetGame().GetClientManager()._clients.Values)
            {
                if (client != null && client.GetHabbo() != null && client.GetHabbo().Rank > 3)
                    clients.Add(client.GetHabbo(), (Convert.ToUInt16(client.GetHabbo().Rank)));
            }

            foreach (KeyValuePair<Habbo, UInt32> client in clients.OrderBy(key => key.Value))
            {
                if (client.Key == null)
                    continue;

                content.Append("¥ " + client.Key.Username + " [Rango: " + client.Key.Rank + "] » Se na sala: " + ((client.Key.CurrentRoom == null) ? "em nenhuma sala." : client.Key.CurrentRoom.RoomData.Name) + "\r\n");
            }

            Session.SendMessage(new MOTDNotificationComposer(content.ToString()));
            return;
        }
    }
}