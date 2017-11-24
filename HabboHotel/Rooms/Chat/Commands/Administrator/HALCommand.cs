using Cloud.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Cloud.HabboHotel.Rooms.Chat.Commands.Administrator
{
    class HALCommand : IChatCommand
    {
        public string PermissionRequired => "command_hal";
        public string Parameters => "[MENSAJE] [URL]";
        public string Description => "Enviar mensagem com link";

        public void Execute(GameClients.GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length == 2)
            {
                Session.SendWhisper("Por favor, escreva uma mensagem e uma URL para enviar.");
                return;
            }

            string URL = Params[2];
            string Message = CommandManager.MergeParams(Params, 2);
            CloudServer.GetGame().GetClientManager().SendMessage(new RoomNotificationComposer("Alerta del Hotel!", Params[1] + "\r\n" + "- " + Session.GetHabbo().Username, "", URL, URL));
            return;
        }
    }
}
