namespace Cloud.Communication.RCON.Commands.Hotel
{
    class ReloadItemsCommand : IRCONCommand
    {
        public string Description => "Se utiliza para atualizar furnis";
        public string Parameters => "";

        public bool TryExecute(string[] parameters)
        {
            CloudServer.GetGame().GetItemManager().Init();

            return true;
        }
    }
}