namespace Cloud.HabboHotel.Rooms.Chat.Commands.Moderator.Fun
{
    class TeleportCommand : IChatCommand
    {
        public string PermissionRequired => "command_teleport";
        public string Parameters => ""; 
        public string Description => "La habilidad de teletransportarse cualquier lugar dentro de la habitación.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == null)
                return;

            User.TeleportEnabled = !User.TeleportEnabled;
            Room.GetGameMap().GenerateMaps();
        }
    }
}
