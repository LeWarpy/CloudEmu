namespace Cloud.HabboHotel.Rooms.Chat.Commands.User.Fun
{
    class SpinCommand : IChatCommand
    {
        public string PermissionRequired => "command_spin";
        public string Parameters => "";
        public string Description => "Por sua vez, um círculo";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == null)
                return;

            User.ApplyEffect(500);
            Session.SendWhisper("Permitam-me esse gato ;)");
        }
    }
}