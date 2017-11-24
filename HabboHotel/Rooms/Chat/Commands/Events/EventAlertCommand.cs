using Cloud.Communication.Packets.Outgoing.Rooms.Notifications;
using Cloud.HabboHotel.GameClients;
using Cloud.Core;

namespace Cloud.HabboHotel.Rooms.Chat.Commands.Events
{
    class EventAlertCommand : IChatCommand
    {
        public string PermissionRequired => "command_event_alert";
        public string Parameters => "[MENSAJE]";
        public string Description => "Enviar Alerta de Evento ao Hotel!";

        public void Execute(GameClient Session, Room Room, string[] Params)
        {

            if (Session != null)
            {
                if (Room != null)
                {
                    if (Params.Length == 1)
                    {
                        Session.SendWhisper("Por favor, digite uma mensagem para enviar.");
                        return;
                    }
                    else
                    {
                        string Message = CommandManager.MergeParams(Params, 1);

                        CloudServer.GetGame().GetClientManager().SendMessage(new RoomNotificationComposer("Novo Evento no Hotel ",
                             "<font color=\"#00adff\"> <b>" + Session.GetHabbo().Username + "</b></font> Esta organizando um novo Evento!<br><br>" +
                             "Quer particpar desse jogo ? Clique no botão inferior  <b>Ir ao Evento</b>, e você pode participar, siga as instruções!<br><br>" +
                             "Qual Evento é?<br><br>" +
                             "<font color=\"#f11648\"><b>" + Message + "</b></font><br><br>" +
                             "Te esperamos de braços abertos, " + CloudServer.HotelName + "!",
                             NotificationSettings.NOTIFICATION_EVENT_IMG, "Ir ao Evento", "event:navigator/goto/" + Session.GetHabbo().CurrentRoomId));
                    }
                }
            }
        }

    }
}