using System.Linq;
using Cloud.HabboHotel.GameClients;
using Cloud.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Cloud.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class MassBadgeCommand : IChatCommand
    {
        public string PermissionRequired => "command_mass_badge";
        public string Parameters => "[CODIGO]";
        public string Description => "Dar placas a todo el hotel.";

        public void Execute(GameClient Session, Room Room, string[] Params)
        {

            if (Params.Length == 1)
            {
                Session.SendWhisper("Por favor, introduzca el código de la tarjeta de identificación que le gustaría dar a todo el hotel.");
                return;
            }

            foreach (GameClient Client in CloudServer.GetGame().GetClientManager().GetClients.ToList())
            {
                if (Client == null || Client.GetHabbo() == null || Client.GetHabbo().Username == Session.GetHabbo().Username)
                    return;

                if (!Client.GetHabbo().GetBadgeComponent().HasBadge(Params[1]))
                {
                    Client.GetHabbo().GetBadgeComponent().GiveBadge(Params[1], true, Client);
                    Client.SendMessage(RoomNotificationComposer.SendBubble("badge/" + Params[1], "Acabas de recibir una placa!", "/inventory/open/badge"));
                }
                else
                    Client.SendWhisper(Session.GetHabbo().Username + " intento darle una placa, pero ya la tienes!");
            }

            Session.SendWhisper("Usted ha dado con éxito cada usuario en este hotel la placa: " + Params[1] + "!");
        }
    }
}
