using Cloud.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Cloud.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class HotelAlertCommand : IChatCommand
    {
        public string PermissionRequired => "command_hotel_alert";
        public string Parameters => "[MENSAJE]";
        public string Description => "Enviar alerta al hotel.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Por favor, introduzca un mensaje para enviar.");
                return;
            }

            string Message = CommandManager.MergeParams(Params, 1);
            CloudServer.GetGame().GetClientManager().SendMessage(new RoomNotificationComposer("${mod.alert.ha.title}", Message + "\n\n" + "- " + Session.GetHabbo().Username + "\n", "", ""));
            return;
        }
    }
}
