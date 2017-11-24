using System;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections.Generic;

using Cloud.HabboHotel.Users;
using Cloud.HabboHotel.GameClients;

using Cloud.Database.Interfaces;
using Cloud.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Cloud.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class LastConsoleMessagesCommand : IChatCommand
    {
        public string PermissionRequired => "command_user_info";
        public string Parameters => "[USUARIO]";
        public string Description => "Consulta los últimos mensajes del usuario en la consola.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Introduce el nombre del usuario que deseas ver revisar su información.");
                return;
            }

            DataRow UserData = null;
            string Username = Params[1];

            using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `username` FROM users WHERE `username` = @Username LIMIT 1");
                dbClient.AddParameter("Username", Username);
                UserData = dbClient.getRow();
            }

            if (UserData == null)
            {
                Session.SendMessage(new RoomAlertComposer("No existe ningún usuario con el nombre " + Username + "."));
                return;
            }

            GameClient TargetClient = CloudServer.GetGame().GetClientManager().GetClientByUsername(Username);

            DataTable GetLogs = null;
            StringBuilder HabboInfo = new StringBuilder();

            HabboInfo.Append("Estos son los últimos mensajes del usuario sospechoso, recuerda revisar siempre estos casos antes de proceder a banear a menos que sea un  caso evidente de spam.\n\n");

            using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `message` FROM `chatlogs_console` WHERE `user_id` = '" + TargetClient.GetHabbo().Id + "' ORDER BY `id` DESC LIMIT 10");
                GetLogs = dbClient.getTable();

                if (GetLogs == null)
                {
                    Session.SendMessage(new RoomAlertComposer("Lamentablemente el usuario que has solicitado no tiene mensajes en el registro."));
                }

                else if (GetLogs != null)
                {
                    int Number = 11;
                    foreach (DataRow Log in GetLogs.Rows)
                    {
                        Number -= 1;
                        HabboInfo.Append("<font size ='8' color='#B40404'><b>[" + Number + "]</b></font>" + " " + Convert.ToString(Log["message"]) + "\r");
                    }
                }

                Session.SendMessage(new RoomNotificationComposer("Últimos mensajes de " + Username + ":", (HabboInfo.ToString()), "fig/" + TargetClient.GetHabbo().Look + "", "", ""));

            }
        }
    }
}
