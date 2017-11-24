using Cloud.HabboHotel.GameClients;

namespace Cloud.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class KickCommand : IChatCommand
    {
        public string PermissionRequired => "command_kick";
        public string Parameters => "[USUARIO] [MENSAJE]";
        public string Description => "Expulsar o usuário e enviar a razão pela qual.";

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            

            if (Params.Length == 1)
            {
                Session.SendWhisper("Por favor, introduzca el nombre de usuario del usuario que desea llamar.");
                return;
            }

            GameClient TargetClient = CloudServer.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (TargetClient == null)
            {
                Session.SendWhisper("Se produjo un error mientras que la búsqueda de usuario, tal vez no están en línea.");
                return;
            }

            if (TargetClient.GetHabbo() == null)
            {
                Session.SendWhisper("Se produjo un error mientras que la búsqueda de usuario, tal vez no están en línea.");
                return;
            }

            if (TargetClient.GetHabbo().Username == Session.GetHabbo().Username)
            {
                Session.SendWhisper("Consigue una vida.");
                return;
            }

            if (!TargetClient.GetHabbo().InRoom)
            {
                Session.SendWhisper("Ese usuario actualmente no está en una habitación.");
                return;
            }

            Room TargetRoom;
            if (!CloudServer.GetGame().GetRoomManager().TryGetRoom(TargetClient.GetHabbo().CurrentRoomId, out TargetRoom))
                return;

            if (Params.Length > 2)
                TargetClient.SendNotification("Un moderador le ha expulsado de la sala por la siguiente razón: " + CommandManager.MergeParams(Params, 2));
            else
                TargetClient.SendNotification("Un moderador le ha expulsado de la sala.");

            TargetRoom.GetRoomUserManager().RemoveUserFromRoom(TargetClient, true, false);
        }
    }
}
