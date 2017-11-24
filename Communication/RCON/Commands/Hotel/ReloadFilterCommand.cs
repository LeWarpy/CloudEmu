namespace Cloud.Communication.RCON.Commands.Hotel
{
    class ReloadFilterCommand : IRCONCommand
    {
        public string Description => "Se utiliza para actualizar os filtros.";
        public string Parameters => "";

        public bool TryExecute(string[] parameters)
        {
            CloudServer.GetGame().GetChatManager().GetFilter().InitWords();
            CloudServer.GetGame().GetChatManager().GetFilter().InitCharacters();
            return true;
        }
    }
}