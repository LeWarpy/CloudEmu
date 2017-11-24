using Cloud.HabboHotel.GameClients;
using Cloud.Communication.Packets.Outgoing.Rooms.Session;
using Cloud.Communication.Packets.Outgoing.Rooms.Notifications;
using Cloud.Communication.Packets.Outgoing;

namespace Cloud.HabboHotel.Rooms.Chat.Commands.Moderator.Fun
{
    class SendUserCommand : IChatCommand
    {
        public string PermissionRequired => "command_summon";
        public string Parameters => "[USUARIO] [SALAID]";
        public string Description => "Manda a un Usuario a una sala por id";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            
            if (Params.Length == 2)
            {
                Session.SendWhisper("Por favor, introduzca el nombre de usuario del usuario que desea enviar y la id de la sala.");
                return;
            }

            GameClient TargetClient = CloudServer.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (TargetClient == null)
            {
                Session.SendWhisper("Se produjo un error mientras que la búsqueda de usuario, tal vez no está en línea.");
                return;
            }
            
            
            if (TargetClient.GetHabbo() == null)
            {
                Session.SendWhisper("Se produjo un error mientras que la búsqueda de usuario, tal vez no está en línea.");
                return;
            }

            if (TargetClient.GetHabbo().Username == Session.GetHabbo().Username)
            {
                Session.SendWhisper("Consigue una vida.");
                return;
            }

            int RoomID;
            if (!int.TryParse(Params[2], out RoomID))
            {
                Session.SendWhisper("Se produjo un error mientras que la búsqueda de usuario, tal vez no exista... Recuerda utilizar solo numeros para la sala.");
                return;
            }

            if (!CloudServer.GetGame().GetRoomManager().RoomExist(RoomID))
            {
                Session.SendWhisper("Se produjo un error mientras que la búsqueda de la sala, tal vez esta no exista.");
                return;

            }
            RoomData RoomData = CloudServer.GetGame().GetRoomManager().GenerateRoomData(RoomID);
            //TargetClient.SendNotification("Has sido enviado a la sala " + RoomData.Name + "!");
            TargetClient.SendMessage(RoomNotificationComposer.SendBubble("advice", "Has sido enviado a la sala " + RoomData.Name + "!", ""));
            if (!TargetClient.GetHabbo().InRoom)
                TargetClient.SendMessage(new RoomForwardComposer(RoomID));
            else
                TargetClient.GetHabbo().PrepareRoom(RoomID, "");
        }
    }
}