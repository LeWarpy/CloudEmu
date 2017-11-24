using Cloud.Core;

namespace Cloud.Communication.RCON.Commands.Hotel
{
    class ReloadServerSettingsCommand : IRCONCommand
    {
        public string Description => "Se utiliza para recarregar as configurações";
        public string Parameters => "";

        public bool TryExecute(string[] parameters)
        {
            CloudServer.GetGame().GetSettingsManager().Init();
            return true;
        }
    }
}