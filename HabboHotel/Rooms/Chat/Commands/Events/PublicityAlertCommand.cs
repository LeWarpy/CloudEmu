using Cloud.Communication.Packets.Outgoing.Rooms.Notifications;
using Cloud.HabboHotel.GameClients;
using Cloud.Core;

namespace Cloud.HabboHotel.Rooms.Chat.Commands.Events
{
    class PublicityAlertCommand : IChatCommand
    {
        public string PermissionRequired => "command_publi_alert";
        public string Parameters => "[MENSAJE]";
        public string Description => "Enviar um alerta para o seu evento!";

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Session == null) return;
            if (Room == null) return;
            CloudServer.GetGame().GetClientManager().SendMessage(new RoomNotificationComposer("Abriu onda de publicidade..",
                 "Há uma nova onda de publicidade na ativa! Se você quer ganhar <b>muitas recompensas</b> para participar ir para a publicidade salão.<br><br>Quer da uma olhada? <b> <font color=\"#58ACFA\">  "
                 + Session.GetHabbo().Username + "</font></b><br>Se você quiser participar, clique no botão abaixo <b>Ir a Sala</b>, e lá você pode participar.<br><br>O que é essa onda?<br><br><font color='#084B8A'><b>Tente seguir as instruções do aumento guias para participar e ganhar o seu prêmio!</b></font><br><br>¡Te esperamos!", "zpam", "Ir ao Evento", "event:navigator/goto/" + Session.GetHabbo().CurrentRoomId));
        }
    }
}