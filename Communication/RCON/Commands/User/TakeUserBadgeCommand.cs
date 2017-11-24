using System;

using Cloud.HabboHotel.GameClients;

namespace Cloud.Communication.RCON.Commands.User
{
    class TakeUserBadgeCommand : IRCONCommand
    {
        public string Description
        {
            get { return "Este comando é usado para tirar um Emblema de um usuário."; }
        }

        public string Parameters
        {
            get { return "%userId% %badgeId%"; }
        }

        public bool TryExecute(string[] parameters)
        {
            int userId = 0;
            if (!int.TryParse(parameters[0].ToString(), out userId))
                return false;

            GameClient client = CloudServer.GetGame().GetClientManager().GetClientByUserID(userId);
            if (client == null || client.GetHabbo() == null)
                return false;

            // Validate the badge
            if (string.IsNullOrEmpty(Convert.ToString(parameters[1])))
                return false;

            string badge = Convert.ToString(parameters[1]);

            if (client.GetHabbo().GetBadgeComponent().HasBadge(badge))
            {
                client.GetHabbo().GetBadgeComponent().RemoveBadge(badge);
            }
            return true;
        }
    }
}