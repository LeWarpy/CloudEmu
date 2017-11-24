using System.Linq;
using Cloud.HabboHotel.GameClients;

namespace Cloud.Communication.RCON.Commands.Hotel
{
    class ReloadRanksCommand : IRCONCommand
    {
        public string Description => "Se utiliza para recarregar os ranks";
        public string Parameters => "";

        public bool TryExecute(string[] parameters)
        {
            CloudServer.GetGame().GetPermissionManager().Init();

            foreach (GameClient client in CloudServer.GetGame().GetClientManager().GetClients.ToList())
            {
                if (client == null || client.GetHabbo() == null || client.GetHabbo().GetPermissions() == null)
                    continue;

                client.GetHabbo().GetPermissions().Init(client.GetHabbo());
            }
            
            return true;
        }
    }
}