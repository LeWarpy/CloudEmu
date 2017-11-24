using System;
using Cloud.HabboHotel.GameClients;
using Cloud.Communication.Packets.Outgoing.Moderation;

namespace Cloud.Communication.RCON.Commands.Hotel
{
    class HotelAlertCommand : IRCONCommand
    {
        public string Description => "Este comando se utiliza para mandar alerta a um usuário .";
        public string Parameters => "[MENSAJE]";

        public bool TryExecute(string[] parameters)
        {
            string message = Convert.ToString(parameters[0]);

            CloudServer.GetGame().GetClientManager().SendMessage(new BroadcastMessageAlertComposer(CloudServer.GetGame().GetLanguageManager().TryGetValue("server.console.alert") + "\n\n" + message));
            return true;
        }
    }
}