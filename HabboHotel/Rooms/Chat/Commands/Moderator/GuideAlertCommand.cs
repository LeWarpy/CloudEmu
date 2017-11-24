using Cloud.Communication.Packets.Outgoing.Notifications;

namespace Cloud.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class GuideAlertCommand : IChatCommand
    {
        public string PermissionRequired => "command_guide_alert";
        public string Parameters => "[MENSAJE]";
        public string Description => "Enviale un mensaje de alerta a todos los staff online.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Session.GetHabbo()._guidelevel < 1)
            {
                Session.SendWhisper("No puedes enviar alertas para guías si no lo eres.");
                return;
              
            }
            if (Params.Length == 1)
            {
                Session.SendWhisper("Escribe el mensaje que deseas enviar.");
                return;
            }

            string Message = CommandManager.MergeParams(Params, 1);
            CloudServer.GetGame().GetClientManager().GuideAlert(new MOTDNotificationComposer("[GUÍAS][" + Session.GetHabbo().Username + "]\r\r" + Message));
            return;
        }
    }
}