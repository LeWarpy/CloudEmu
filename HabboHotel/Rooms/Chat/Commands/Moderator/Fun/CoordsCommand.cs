namespace Cloud.HabboHotel.Rooms.Chat.Commands.Moderator.Fun
{
    class CoordsCommand : IChatCommand
    {
        public string PermissionRequired => "command_coords";
        public string Parameters => "";
        public string Description => "Obtener Coordenadas de su posición.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            RoomUser ThisUser = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (ThisUser == null)
                return;

            Session.SendNotification("X: " + ThisUser.X + "\n - Y: " + ThisUser.Y + "\n - Z: " + ThisUser.Z + "\n - Rot: " + ThisUser.RotBody + ", sqState: " + Room.GetGameMap().GameMap[ThisUser.X, ThisUser.Y].ToString() + "\n\n - RoomID: " + Session.GetHabbo().CurrentRoomId);                           
        }
    }
}
