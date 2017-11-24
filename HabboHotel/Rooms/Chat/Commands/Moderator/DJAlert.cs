using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Cloud.Communication.Packets.Outgoing.Moderation;
using Cloud.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Cloud.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class DJAlert : IChatCommand
    {
        public string PermissionRequired => "command_djalert";
        public string Parameters => "[MENSAJE]";
        public string Description => "Envia uma alerta para todo o hotel de DJ AO VIVO.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Por favor escribe el mensaje a enviar");
                return;
            }

            string Message = CommandManager.MergeParams(Params, 1);
            CloudServer.GetGame().GetClientManager().SendMessage(RoomNotificationComposer.SendBubble("DJAlertNEW", "¡DJ " + Message + " Ele é transmitido ao vivo! sintonia " + CloudServer.HotelName+ "FM agora e aproveitar ao máximo.", ""));
            return;
        }
    }
}
