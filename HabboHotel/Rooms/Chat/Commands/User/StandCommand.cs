namespace Cloud.HabboHotel.Rooms.Chat.Commands.User
{
    class StandCommand : IChatCommand
    {
        public string PermissionRequired => "command_stand";
        public string Parameters => "";
        public string Description => "levantar.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Username);
            if (User == null)
                return;

            if (User.isSitting)
            {
                User.Statusses.Remove("sit");
                User.Z += 0.35;
                User.isSitting = false;
                User.UpdateNeeded = true;
            }
            else if (User.isLying)
            {
                User.Statusses.Remove("lay");
                User.Z += 0.35;
                User.isLying = false;
                User.UpdateNeeded = true;
            }
        }
    }
}
