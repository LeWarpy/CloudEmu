namespace Cloud.HabboHotel.Rooms.Chat.Commands.User
{
    class DNDCommand : IChatCommand
    {
        public string PermissionRequired => "command_dnd";
        public string Parameters => "";
        public string Description => "Ativar/Desativar Mensagens do Consele.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            
            Session.GetHabbo().AllowConsoleMessages = !Session.GetHabbo().AllowConsoleMessages;
            Session.SendWhisper("Tu " + (Session.GetHabbo().AllowConsoleMessages == true ? "agora" : "nao") + " pode receber mensagens no console.");
        }
    }
}