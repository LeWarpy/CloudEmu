using Cloud.HabboHotel.GameClients;

namespace Cloud.HabboHotel.Rooms.Chat.Commands.Moderator.Fun
{
    class UnFreezeCommand : IChatCommand
    {
        public string PermissionRequired => "command_unfreeze";
        public string Parameters => "[USUARIO]";
        public string Description => "Permitir que otro usuario a caminar de nuevo.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            

            if (Params.Length == 1)
            {
                Session.SendWhisper("Por favor, introduzca el nombre de usuario del usuario que desea des-congelación.");
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
                TargetUser.Frozen = false;

            Session.SendWhisper("Descongeló exitosamente a " + TargetClient.GetHabbo().Username + "!");
        }
    }
}
