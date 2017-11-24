namespace Cloud.Communication.RCON.Commands.Hotel
{
    class ReloadQuestsCommand : IRCONCommand
    {
        public string Description => "Se utiliza para atualizar as quest";
        public string Parameters => "";

        public bool TryExecute(string[] parameters)
        {
            CloudServer.GetGame().GetQuestManager().Init();

            return true;
        }
    }
}