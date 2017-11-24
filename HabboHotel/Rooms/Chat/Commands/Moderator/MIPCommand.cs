using System;
using Cloud.Database.Interfaces;
using Cloud.HabboHotel.Users;
using Cloud.HabboHotel.GameClients;
using Cloud.HabboHotel.Moderation;

namespace Cloud.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class MIPCommand : IChatCommand
    {
        public string PermissionRequired => "command_mip";
        public string Parameters => "[USUARIO]";
        public string Description => "Machine ban, Banear IP y la cuenta de otro usuario.";

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            
            if (Params.Length == 1)
            {
                Session.SendWhisper("Por favor, introduzca el nombre de usuario del usuario que desea Ban IP y cuenta de la prohibición.");
                return;
            }

            Habbo Habbo = CloudServer.GetHabboByUsername(Params[1]);
            if (Habbo == null)
            {
                Session.SendWhisper("Se produjo un error mientras que la búsqueda de usuario en la base de datos.");
                return;
            }

            if (Habbo.GetPermissions().HasRight("mod_tool") && !Session.GetHabbo().GetPermissions().HasRight("mod_ban_any"))
            {
                Session.SendWhisper("Vaya, no se puede prohibir que el usuario.");
                return;
            }

            String IPAddress = String.Empty;
            Double Expire = CloudServer.GetUnixTimestamp() + 78892200;
            string Username = Habbo.Username;

            using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.runFastQuery("UPDATE `user_info` SET `bans` = `bans` + '1' WHERE `user_id` = '" + Habbo.Id + "' LIMIT 1");

                dbClient.SetQuery("SELECT `ip_last` FROM `users` WHERE `id` = '" + Habbo.Id + "' LIMIT 1");
                IPAddress = dbClient.getString();
            }

            string Reason = null;
            if (Params.Length >= 3)
                Reason = CommandManager.MergeParams(Params, 2);
            else
                Reason = "No hay razón especifica.";

            if (!string.IsNullOrEmpty(IPAddress))
                CloudServer.GetGame().GetModerationManager().BanUser(Session.GetHabbo().Username, ModerationBanType.IP, IPAddress, Reason, Expire);
            CloudServer.GetGame().GetModerationManager().BanUser(Session.GetHabbo().Username, ModerationBanType.USERNAME, Habbo.Username, Reason, Expire);

            if (!string.IsNullOrEmpty(Habbo.MachineId))
                CloudServer.GetGame().GetModerationManager().BanUser(Session.GetHabbo().Username, ModerationBanType.MACHINE, Habbo.MachineId, Reason, Expire);

            GameClient TargetClient = CloudServer.GetGame().GetClientManager().GetClientByUsername(Username);
            if (TargetClient != null)
                TargetClient.Disconnect();
            Session.SendWhisper("Éxito, la maquina de baneos baneo a: '" + Username + "' por la siguiente razón: '" + Reason + "'!");
        }
    }
}