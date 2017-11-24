
using Cloud.HabboHotel.GameClients;
using Cloud.Communication.Packets.Outgoing.Rooms.Session;
using Cloud.Communication.Packets.Outgoing.Rooms.Notifications;
using Cloud.HabboHotel.Rooms;

namespace Cloud.Communication.RCON.Commands.User
{
    class SendUserCommand : IRCONCommand
    {
        public string Description
        {
            get { return "Este comando é usado para enviar um usuário para um quarto."; }
        }

        public string Parameters
        {
            get { return "%userId% %roomId%"; }
        }

        public bool TryExecute(string[] parameters)
        {
            
            int userId = 0;
            if (!int.TryParse(parameters[0].ToString(), out userId))
                return false;

            GameClient client = CloudServer.GetGame().GetClientManager().GetClientByUserID(userId);
            if (client == null || client.GetHabbo() == null)
                return false;

            // Validate the message
            int RoomID = 0;
            if (!int.TryParse(parameters[1], out RoomID))
                return false;

            if (!CloudServer.GetGame().GetRoomManager().RoomExist(RoomID))
                return false;

            RoomData RoomData = CloudServer.GetGame().GetRoomManager().GenerateRoomData(RoomID);
            //TargetClient.SendNotification("Has sido enviado a la sala " + RoomData.Name + "!");
            client.SendMessage(RoomNotificationComposer.SendBubble("advice", "Has sido enviado a la sala " + RoomData.Name + "!", ""));
            if (!client.GetHabbo().InRoom)
                client.SendMessage(new RoomForwardComposer(RoomID));
            else
                client.GetHabbo().PrepareRoom(RoomID, "");
            
            return true;
        }
    }
}