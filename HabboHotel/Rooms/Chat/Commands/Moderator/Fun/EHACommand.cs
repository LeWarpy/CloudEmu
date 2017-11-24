using Cloud.Communication.Packets.Outgoing.Rooms.Notifications;
using Cloud.HabboHotel.GameClients;

namespace Cloud.HabboHotel.Rooms.Chat.Commands.Events
{
    class EHACommand : IChatCommand
    {
        public string PermissionRequired => "command_event_alert";
        public string Parameters => "[MENSAJE]";
        public string Description => "Enviar una alerta de hotel para su evento!";

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Session != null)
            {
                if (Room != null)
                {
                    string Message = "" + "Hey, há um evento acontecendo agora, ver que é!";
                    if (Params.Length > 2)
                        Message = CommandManager.MergeParams(Params, 1);

                    CloudServer.GetGame().GetClientManager().SendMessage(new RoomNotificationComposer("Evento em andamento", Message + "\r\n- <b>" + Session.GetHabbo().Username + "</b>\r\n<i></i>", "figure/" + Session.GetHabbo().Username + "", "Go to \"" + Session.GetHabbo().CurrentRoom.Name + "\"!", "event:navigator/goto/" + Session.GetHabbo().CurrentRoomId));
                }
            }
        }
    }
}