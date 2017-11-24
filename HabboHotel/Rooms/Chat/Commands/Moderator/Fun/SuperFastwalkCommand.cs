namespace Cloud.HabboHotel.Rooms.Chat.Commands.Moderator.Fun
{
    class SuperFastwalkCommand : IChatCommand
    {
        public string PermissionRequired => "command_super_fastwalk";
        public string Parameters => "";
        public string Description => "Le da la capacidad de caminar muy muy rápido.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == null)
                return;

            User.SuperFastWalking = !User.SuperFastWalking;

            if (User.FastWalking)
                User.FastWalking = false;

            if (User.SuperFastWalking)
                Session.SendWhisper("Caminar Super Rapido Activado!");
            else
                Session.SendWhisper("Caminar Super Rapido Desactivado!");
        }
    }
}
