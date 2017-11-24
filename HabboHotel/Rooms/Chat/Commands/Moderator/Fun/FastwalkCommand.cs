namespace Cloud.HabboHotel.Rooms.Chat.Commands.Moderator.Fun
{
    class FastwalkCommand : IChatCommand
    {
        public string PermissionRequired => "command_fastwalk";
        public string Parameters => "";
        public string Description => "Capacidad de Caminar Rápido.";

        public void Execute(GameClients.GameClient Session, Room Room, string[] Params)
        {
            

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == null)
                return;

            User.FastWalking = !User.FastWalking;

            if (User.SuperFastWalking)
                User.SuperFastWalking = false;

            if (User.FastWalking)
                Session.SendWhisper("Caminar Rapido Activado!");
            else
                Session.SendWhisper("Caminar Rapido Desactivado!");
        }
    }
}
