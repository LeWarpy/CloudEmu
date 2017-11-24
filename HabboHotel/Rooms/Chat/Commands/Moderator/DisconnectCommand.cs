using Cloud.HabboHotel.GameClients;

namespace Cloud.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class DisconnectCommand : IChatCommand
    {
        public string PermissionRequired => "command_disconnect";
        public string Parameters => "[USUARIO]";
        public string Description => "Desconectar a un usuario.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            

            if (Params.Length == 1)
            {
                Session.SendWhisper("Escribe el nombre del usuario que deseas desconectar.");
                return;
            }

            GameClient TargetClient = CloudServer.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (TargetClient == null)
            {
                Session.SendWhisper("¡Oops! Probablemente el usuario no se encuentre en linea.");
                return;
            }

            if (TargetClient.GetHabbo().Rank >= Session.GetHabbo().Rank)
            {
                Session.SendWhisper("No puedes desconectar a este usuario.");
                return;
            }

            if (TargetClient.GetHabbo().GetPermissions().HasRight("mod_tool") && !Session.GetHabbo().GetPermissions().HasRight("mod_disconnect_any"))
            {
                Session.SendWhisper("No puedes desconectar a este usuario.");
                return;
            }

            TargetClient.GetConnection().Dispose();
        }
    }
}