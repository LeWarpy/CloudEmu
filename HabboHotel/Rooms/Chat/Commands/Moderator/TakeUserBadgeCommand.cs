using System;
using Cloud.Communication.Packets.Outgoing.Moderation;
using Cloud.HabboHotel.GameClients;

namespace Cloud.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class TakeUserBadgeCommand : IChatCommand
    {
        public string PermissionRequired => "command_give_badge"; 
        public string Parameters => "[USUARIO] [CODIGO]";
        public string Description => "Se utiliza para quitarle la placa a un usuario.";

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length != 3)
            {
                Session.SendWhisper("Por favor, introduzca un nombre de usuario y el código del identificador que le gustaría dar!");
                return;
            }

            GameClient client = CloudServer.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (client == null || client.GetHabbo() == null)
                return;

            // Validate the badge
            if (string.IsNullOrEmpty(Convert.ToString(Params[2])))
                return;

            string badge = Convert.ToString(Params[2]);

            if (client.GetHabbo().GetBadgeComponent().HasBadge(badge))
            {
                client.GetHabbo().GetBadgeComponent().RemoveBadge(badge);
                Session.SendMessage(new BroadcastMessageAlertComposer(CloudServer.GetGame().GetLanguageManager().TryGetValue("server.console.alert") + "\n\n" + "La placa <b>"+ badge +" le fue removida a "+ client.GetHabbo().Username+" con éxito!"));
            }
            return;
        }
    }
}