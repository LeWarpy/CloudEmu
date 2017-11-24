using Cloud.HabboHotel.GameClients;
using Cloud.Communication.Packets.Outgoing.Rooms.Session;

namespace Cloud.HabboHotel.Rooms.Chat.Commands.Moderator.Fun
{
    class SummonCommand : IChatCommand
    {
        public string PermissionRequired => "command_summon";
        public string Parameters => "[USUARIO]";
        public string Description => "Traer a un usuario a la sala actual.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
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

            TargetClient.SendNotification("Se le ha citado a " + Session.GetHabbo().Username + "!");
            if (!TargetClient.GetHabbo().InRoom)
                TargetClient.SendMessage(new RoomForwardComposer(Session.GetHabbo().CurrentRoomId));
            else
                TargetClient.GetHabbo().PrepareRoom(Session.GetHabbo().CurrentRoomId, "");
        }
    }
}