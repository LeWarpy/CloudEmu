using System;
using Cloud.Database.Interfaces;
using Cloud.HabboHotel.Users;

namespace Cloud.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class TradeBanCommand : IChatCommand
    {
        public string PermissionRequired => "command_trade_ban";
        public string Parameters => "[USUARIO] [TIEMPO]";
        public string Description => "Prohibir el tradeo de otro usuario.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            
            if (Params.Length == 1)
            {
                Session.SendWhisper("Por favor, introduzca un nombre de usuario y una longitud válida en días (minuto 1 día, máximo 365 días).");
                return;
            }

            Habbo Habbo = CloudServer.GetHabboByUsername(Params[1]);
            if (Habbo == null)
            {
                Session.SendWhisper("Se produjo un error mientras que la búsqueda de usuario en la base de datos.");
                return;
            }

            if (Convert.ToDouble(Params[2]) == 0)
            {
                using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.RunQuery("UPDATE `user_info` SET `trading_locked` = '0' WHERE `user_id` = '" + Habbo.Id + "' LIMIT 1");
                }

                if (Habbo.GetClient() != null)
                {
                    Habbo.TradingLockExpiry = 0;
                    Habbo.GetClient().SendNotification("Su prohibición comercial excepcional se ha eliminado.");
                }

                Session.SendWhisper("Usted ha eliminado con éxito el baneo de tradeo a: " + Habbo.Username + ".");
                return;
            }

            double Days;
            if (double.TryParse(Params[2], out Days))
            {
                if (Days < 1)
                    Days = 1;

                if (Days > 365)
                    Days = 365;

                double Length = (CloudServer.GetUnixTimestamp() + (Days * 86400));
                using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.runFastQuery("UPDATE `user_info` SET `trading_locked` = '" + Length + "', `trading_locks_count` = `trading_locks_count` + '1' WHERE `user_id` = '" + Habbo.Id + "' LIMIT 1");
                }

                if (Habbo.GetClient() != null)
                {
                    Habbo.TradingLockExpiry = Length;
                    Habbo.GetClient().SendNotification("Se le ha prohibido el tradei por: " + Days + " día(s)!");
                }

                Session.SendWhisper("Usted con éxito ha prohibido el tradeo de " + Habbo.Username + " por " + Days + " día(s).");
            }
            else
                Session.SendWhisper("Por favor, introduzca un número entero válido.");
        }
    }
}
