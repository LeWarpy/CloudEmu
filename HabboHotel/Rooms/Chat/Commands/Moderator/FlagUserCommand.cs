using Cloud.HabboHotel.GameClients;
using Cloud.Communication.Packets.Outgoing.Handshake;

namespace Cloud.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class FlagUserCommand : IChatCommand
    {
        public string PermissionRequired => "command_flaguser";
        public string Parameters => "[USUARIO]";
        public string Description => "Cambiar el nombre a un usuario.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            
            if (Params.Length == 1)
            {
                Session.SendWhisper("Por favor, introduzca el nombre de usuario que desea cambiar.");
                return;
            }

            GameClient TargetClient = CloudServer.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (TargetClient == null)
            {
                Session.SendWhisper("Se produjo un error mientras que la búsqueda de usuario, tal vez no están en línea.");
                return;
            }

            if (TargetClient.GetHabbo().GetPermissions().HasRight("mod_tool"))
            {
                Session.SendWhisper("No se le permite al usuario de que la bandera.");
                return;
            }
            else
            {
                TargetClient.GetHabbo().LastNameChange = 0;
                TargetClient.GetHabbo().ChangingName = true;
                TargetClient.SendNotification("Tenga en cuenta que si su nombre de usuario se considerará como no apropiado, se le prohibió sin lugar a dudas.\r\rTambién tenga en cuenta que el personal no le permitirá cambiar su nombre de usuario de nuevo en caso de tener un problema con lo que haya elegido.\r\rCierre esta ventana y haga clic en sí mismo para comenzar a elegir un nuevo nombre de usuario!");
                TargetClient.SendMessage(new UserObjectComposer(TargetClient.GetHabbo()));
            }

        }
    }
}
