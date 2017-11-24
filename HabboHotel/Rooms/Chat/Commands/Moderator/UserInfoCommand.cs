using System;
using System.Text;
using System.Data;
using Cloud.HabboHotel.GameClients;
using Cloud.Database.Interfaces;

namespace Cloud.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class UserInfoCommand : IChatCommand
    {
        public string PermissionRequired => "command_user_info";
        public string Parameters => "[USUARIO]";
        public string Description => "Ver información de un usuario.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Por favor, introduzca el nombre de usuario del usuario que desea ver.");
                return;
            }

            DataRow UserData = null;
            DataRow UserInfo = null;
            string Username = Params[1];

            using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `id`,`username`,`mail`,`rank`,`motto`,`credits`,`activity_points`,`vip_points`,`gotw_points`,`online`,`rank_vip` FROM users WHERE `username` = @Username LIMIT 1");
                dbClient.AddParameter("Username", Username);
                UserData = dbClient.getRow();
            }

            if (UserData == null)
            {
                Session.SendNotification("Vaya, no hay ningún usuario en la base de datos con ese nombre de usuario (" + Username + ")!");
                return;
            }

            using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `user_info` WHERE `user_id` = '" + Convert.ToInt32(UserData["id"]) + "' LIMIT 1");
                UserInfo = dbClient.getRow();
                if (UserInfo == null)
                {
                    dbClient.runFastQuery("INSERT INTO `user_info` (`user_id`) VALUES ('" + Convert.ToInt32(UserData["id"]) + "')");

                    dbClient.SetQuery("SELECT * FROM `user_info` WHERE `user_id` = '" + Convert.ToInt32(UserData["id"]) + "' LIMIT 1");
                    UserInfo = dbClient.getRow();
                }
            }

            GameClient TargetClient = CloudServer.GetGame().GetClientManager().GetClientByUsername(Username);

            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(Convert.ToDouble(UserInfo["trading_locked"]));

            StringBuilder HabboInfo = new StringBuilder();
            HabboInfo.Append("Cuenta de " + Convert.ToString(UserData["username"]) + ":\r\r");
            HabboInfo.Append("Información Genérica:\r");
            HabboInfo.Append("ID: " + Convert.ToInt32(UserData["id"]) + "\r");
            HabboInfo.Append("Rango: " + Convert.ToInt32(UserData["rank"]) + "\r");
            HabboInfo.Append("Rango VIP: " + Convert.ToInt32(UserData["rank_vip"]) + "\r");
            HabboInfo.Append("Email: " + Convert.ToString(UserData["mail"]) + "\r");
            HabboInfo.Append("Estado en línea: " + (TargetClient != null ? "Si" : "No") + "\r\r");

            HabboInfo.Append("Información de la moneda:\r");
            HabboInfo.Append("Créditos: " + Convert.ToInt32(UserData["credits"]) + "\r");
            HabboInfo.Append("Duckets: " + Convert.ToInt32(UserData["activity_points"]) + "\r");
            HabboInfo.Append("Diamantes: " + Convert.ToInt32(UserData["vip_points"]) + "\r");
            HabboInfo.Append("GOTW Points: " + Convert.ToInt32(UserData["gotw_points"]) + "\r\r");

            HabboInfo.Append("Información de la moderación:\r");
            HabboInfo.Append("Baneos: " + Convert.ToInt32(UserInfo["bans"]) + "\r");
            HabboInfo.Append("CFHs Sent: " + Convert.ToInt32(UserInfo["cfhs"]) + "\r");
            HabboInfo.Append("Abusive CFHs: " + Convert.ToInt32(UserInfo["cfhs_abusive"]) + "\r");
            HabboInfo.Append("Bloqueo de tradeo: " + (Convert.ToInt32(UserInfo["trading_locked"]) == 0 ? "Sin bloqueo sobresaliente" : "Expira: " + (origin.ToString("dd/MM/yyyy")) + "") + "\r");
            HabboInfo.Append("Cantidad de bloqueos comerciales: " + Convert.ToInt32(UserInfo["trading_locks_count"]) + "\r\r");

            if (TargetClient != null)
            {
                HabboInfo.Append("Sesión Actual:\r");
                if (!TargetClient.GetHabbo().InRoom)
                    HabboInfo.Append("En la actualidad no en una habitación.\r");
                else
                {
                    HabboInfo.Append("Sala: " + TargetClient.GetHabbo().CurrentRoom.Name + " (" + TargetClient.GetHabbo().CurrentRoom.RoomId + ")\r");
                    HabboInfo.Append("Propietario de la sala: " + TargetClient.GetHabbo().CurrentRoom.OwnerName + "\r");
                    HabboInfo.Append("Visitantes actuales: " + TargetClient.GetHabbo().CurrentRoom.UserCount + "/" + TargetClient.GetHabbo().CurrentRoom.UsersMax);
                }
            }
            Session.SendNotification(HabboInfo.ToString());
        }
    }
}
