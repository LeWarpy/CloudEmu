using System.Linq;

namespace Cloud.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class RoomBadgeCommand : IChatCommand
    {
        public string PermissionRequired => "command_room_badge";
        public string Parameters => "[CODIGO]";
        public string Description => "Dar placa a toda la sala.";

        public void Execute(GameClients.GameClient Session, Room Room, string[] Params)
        {
            
            if (Params.Length == 1)
            {
                Session.SendWhisper("Por favor, introduzca el nombre del identificador que le gustaría dar a la habitación.");
                return;
            }

            foreach (RoomUser User in Room.GetRoomUserManager().GetUserList().ToList())
            {
                if (User == null || User.GetClient() == null || User.GetClient().GetHabbo() == null)
                    return;

                if (!User.GetClient().GetHabbo().GetBadgeComponent().HasBadge(Params[1]))
                {
                    User.GetClient().GetHabbo().GetBadgeComponent().GiveBadge(Params[1], true, User.GetClient());
                    User.GetClient().SendNotification("Usted acaba de ser dada una insignia!");
                }
                else
                {
                    User.GetClient().SendWhisper(Session.GetHabbo().Username + " intento darle una placa, pero ya la tienes!");
                    return;
                }
                    
            }

            Session.SendWhisper("Usted ha dado con éxito todos los usuarios en esta sala la placa: " + Params[2] + "!");
        }
    }
}
