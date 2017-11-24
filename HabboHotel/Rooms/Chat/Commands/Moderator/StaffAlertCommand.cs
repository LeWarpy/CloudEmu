using Cloud.Communication.Packets.Outgoing.Notifications;

namespace Cloud.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class StaffAlertCommand : IChatCommand
    {
        public string PermissionRequired => "command_staff_alert";
        public string Parameters => "[MENSAJE]";
        public string Description => "Enviar mensaje a los staff.";

        public void Execute(GameClients.GameClient Session, Room Room, string[] Params)
        {

            if (Params.Length == 1)
            {
                Session.SendWhisper("Por favor, introduzca un mensaje para enviar.");
                return;
            }

            string Message = CommandManager.MergeParams(Params, 1);
            CloudServer.GetGame().GetClientManager().StaffAlert(new MOTDNotificationComposer("Mensaje Administrativo:\r\r" + Message + "\r\n" + "- " + Session.GetHabbo().Username));
            return;
        }
    }
}