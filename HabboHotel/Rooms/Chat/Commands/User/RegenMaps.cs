namespace Cloud.HabboHotel.Rooms.Chat.Commands.User
{
    class RegenMaps : IChatCommand
    {
        public string PermissionRequired => "command_regen_maps";
        public string Parameters => "";
        public string Description => "Regenerar o mapa da sala!";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {


            if (!Room.CheckRights(Session, true))
            {
                Session.SendWhisper("Bem, somente o proprietário da sala pode executar este comando!");
                return;
            }

            Room.GetGameMap().GenerateMaps();
            Session.SendWhisper("mapa do jogo desta sala regenerado com sucesso.");
        }
    }
}
