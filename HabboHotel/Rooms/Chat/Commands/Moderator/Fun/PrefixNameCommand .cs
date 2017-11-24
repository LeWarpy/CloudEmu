using Cloud.Communication.Packets.Outgoing.Rooms.Notifications;
using Cloud.Database.Interfaces;

namespace Cloud.HabboHotel.Rooms.Chat.Commands.Moderator.Fun
{
    class PrefixNameCommand : IChatCommand
    {

        public string PermissionRequired => "command_event_alert"; 
        public string Parameters => "[PREFIX]"; 
        public string Description => "off/red/green/blue/cyan/purple"; 

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Por favor, introduce por ejemplo: [ADM]");
                return;
            }

            if (Params[1].ToString().ToLower() == "off")
            {
                Session.GetHabbo()._NamePrefix = "";
                Session.SendWhisper("Desactivaste tu prefijo con éxito!");
                using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.runFastQuery("UPDATE `users` SET `prefix_name` = '' WHERE `id` = '" + Session.GetHabbo().Id + "' LIMIT 1");
                }
            }
            else
            {
                string PrefixName = CommandManager.MergeParams(Params, 1);
                Session.GetHabbo()._NamePrefix = PrefixName;
                Session.SendWhisper("Tu prefijo para el nombre se añadio correctamente");
                using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("UPDATE `users` SET `prefix_name` = @prefix WHERE `id` = '" + Session.GetHabbo().Id + "' LIMIT 1");
                    dbClient.AddParameter("prefix", PrefixName);
                    dbClient.RunQuery();
                }
            }
            return;
        }
    }
}