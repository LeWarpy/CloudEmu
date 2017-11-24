using Cloud.HabboHotel.GameClients;

namespace Cloud.HabboHotel.Rooms.Chat.Commands.Moderator.Fun
{
    class FreezeCommand : IChatCommand
    {
        public string PermissionRequired => "command_freeze";
        public string Parameters => "[USUARIO]";
        public string Description => "Congelar a un usuario.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            
            if (Params.Length == 1)
            {
                Session.SendWhisper("Por favor, introduzca el nombre de usuario del usuario que desea congelar.");
                return;
            }

            GameClient TargetClient = CloudServer.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (TargetClient == null)
            {
                Session.SendWhisper("Se produjo un error mientras que la búsqueda de usuario, tal vez no están en línea.");
                return;
            }

            RoomUser TargetUser = Session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Params[1]);
            if (TargetUser != null)
                TargetUser.Frozen = true;

			Session.SendWhisper("Se congelo exitosamente a " + TargetClient.GetHabbo().Username + "!");
        }
    }
}
