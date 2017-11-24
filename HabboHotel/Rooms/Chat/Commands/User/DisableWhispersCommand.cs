namespace Cloud.HabboHotel.Rooms.Chat.Commands.User
{
    class DisableWhispersCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_disable_whispers"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "permite voce ativar ou desativar a capacidade de sussurros."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            
            Session.GetHabbo().ReceiveWhispers = !Session.GetHabbo().ReceiveWhispers;
            Session.SendWhisper("You're " + (Session.GetHabbo().ReceiveWhispers ? "agora" : "nao") + " Recebendo sussurro!");
        }
    }
}
