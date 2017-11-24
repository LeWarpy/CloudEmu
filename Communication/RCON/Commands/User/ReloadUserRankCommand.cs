using Cloud.Communication.Packets.Outgoing.Moderation;
using Cloud.Database.Interfaces;
using Cloud.HabboHotel.GameClients;

namespace Cloud.Communication.RCON.Commands.User
{
    class ReloadUserRankCommand : IRCONCommand
    {
        public string Description
        {
            get { return "Este comando é usado para recarregar uma classificação e permissões de usuários."; }
        }

        public string Parameters
        {
            get { return "%userId%"; }
        }

        public bool TryExecute(string[] parameters)
        {
            int userId = 0;
            if (!int.TryParse(parameters[0].ToString(), out userId))
                return false;

            GameClient client = CloudServer.GetGame().GetClientManager().GetClientByUserID(userId);
            if (client == null || client.GetHabbo() == null)
                return false;

            using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `rank` FROM `users` WHERE `id` = @userId LIMIT 1");
                dbClient.AddParameter("userId", userId);
                client.GetHabbo().Rank = dbClient.getInteger();
            }

            client.GetHabbo().GetPermissions().Init(client.GetHabbo());

            if (client.GetHabbo().GetPermissions().HasRight("mod_tickets"))
            {
                client.SendMessage(new ModeratorInitComposer(
                  CloudServer.GetGame().GetModerationManager().UserMessagePresets,
                  CloudServer.GetGame().GetModerationManager().RoomMessagePresets,
                  CloudServer.GetGame().GetModerationManager().GetTickets));
            }
            return true;
        }
    }
}