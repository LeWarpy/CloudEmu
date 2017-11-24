using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;



using Cloud.HabboHotel.Users;

using Cloud.Communication.Packets.Outgoing.Notifications;

namespace Cloud.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class ViewOnlineCommand : IChatCommand
    {
        public string PermissionRequired => "command_view_online";
        public string Parameters => "";
        public string Description => "Ver los usuarios online.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            Dictionary<Habbo, UInt32> clients = new Dictionary<Habbo, UInt32>();

            StringBuilder content = new StringBuilder();
            content.Append("- LISTA DE LOS USUARIOS ONLINE -\r\n");

            foreach (var client in CloudServer.GetGame().GetClientManager().GetClients.ToList())
            {
                if (client == null)
                    continue;

                content.Append("¥ " + client.GetHabbo().Username + " » Se encuentra en la sala: " + ((client.GetHabbo().CurrentRoom == null) ? "En ninguna sala." : client.GetHabbo().CurrentRoom.RoomData.Name) + "\r\n");
            }

            Session.SendMessage(new MOTDNotificationComposer(content.ToString()));
            return;
        }
    }
}
