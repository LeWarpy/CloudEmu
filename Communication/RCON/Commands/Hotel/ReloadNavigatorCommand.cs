namespace Cloud.Communication.RCON.Commands.Hotel
{
    class ReloadNavigatorCommand : IRCONCommand
    {
        public string Description => "Se utiliza para atualizar navegador";
        public string Parameters => "";

        public bool TryExecute(string[] parameters)
        {
            CloudServer.GetGame().GetNavigator().Init();

            return true;
        }
    }
}