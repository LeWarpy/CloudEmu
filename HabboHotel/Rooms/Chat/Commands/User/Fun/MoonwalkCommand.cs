namespace Cloud.HabboHotel.Rooms.Chat.Commands.User.Fun
{
    class MoonwalkCommand : IChatCommand
    {
        public string PermissionRequired => "command_moonwalk";
        public string Parameters => "";
        public string Description => "Use os sapatos de Michael Jackson.";

        public void Execute(GameClients.GameClient Session, Room Room, string[] Params)
        {

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == null)
                return;

            User.moonwalkEnabled = !User.moonwalkEnabled;

            if (User.moonwalkEnabled)
                Session.SendWhisper("Modo Michael Jackson Ativado! Auu!");
            else
                Session.SendWhisper("Modo Michael Jackson Desativado! Ah :(");
        }
    }
}
