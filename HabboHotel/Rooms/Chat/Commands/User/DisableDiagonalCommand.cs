namespace Cloud.HabboHotel.Rooms.Chat.Commands.User
{
    class DisableDiagonalCommand : IChatCommand
    {
        public string PermissionRequired => "command_disable_diagonal";
        public string Parameters => "";
        public string Description => " Desativar diagonal no seu quarto!";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {

            if (!Room.CheckRights(Session, true))
            {
                Session.SendWhisper("Oops, Somente o proprietário da sala pode executar esse comando!");
                return;
            }

            Room.GetGameMap().DiagonalEnabled = !Room.GetGameMap().DiagonalEnabled;
            Session.SendWhisper("Comando executado com Sucesso!.");
        }
    }
}
