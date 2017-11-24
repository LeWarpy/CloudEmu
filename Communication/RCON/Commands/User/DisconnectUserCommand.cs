using Cloud.HabboHotel.GameClients;

namespace Cloud.Communication.RCON.Commands.User
{
    class DisconnectUserCommand : IRCONCommand
    {
        public string Description
        {
            get { return "Este comando é usado para desconectar um usuário."; }
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

            client.Disconnect();
            return true;
        }
    }
}
