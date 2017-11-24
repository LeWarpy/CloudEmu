using System.Linq;
using System.Collections.Generic;

namespace Cloud.HabboHotel.Rooms.Chat.Commands.Moderator.Fun
{
    class MassEnableCommand : IChatCommand
    {
        public string PermissionRequired => "command_massenable";
        public string Parameters => "[EFFECTID]";
        public string Description => "Efecto a cada usuario en la sala.";

        public void Execute(GameClients.GameClient Session, Room Room, string[] Params)
        {
            
            if (Params.Length == 1)
            {
                Session.SendWhisper("Por favor, introduzca un ID de efecto.");
                return;
            }

            int EnableId = 0;
            if (int.TryParse(Params[1], out EnableId))
            {
                if ((EnableId == 102 || EnableId == 187) && !Session.GetHabbo().GetPermissions().HasRight("mod_tool"))
                {
                    Session.SendWhisper("Lo sentimos, sólo los miembros del personal pueden utilizar estos efectos.");
                    return;
                }

                if (EnableId == 178 && (!Session.GetHabbo().GetPermissions().HasRight("gold_vip") && !Session.GetHabbo().GetPermissions().HasRight("events_staff")))
                {
                    Session.SendWhisper("Lo sentimos, sólo los miembros VIP Gold y Eventos del personal pueden utilizar este efecto.");
                    return;
                }

                if (!Session.GetHabbo().GetPermissions().HasCommand("command_override_massenable") && Room.OwnerId != Session.GetHabbo().Id)
                {
                    Session.SendWhisper("Sólo puede utilizar este comando en su propia habitación.");
                    return;
                }

                List<RoomUser> Users = Room.GetRoomUserManager().GetRoomUsers();
                if (Users.Count > 0)
                {
                    foreach (RoomUser U in Users.ToList())
                    {
                        if (U == null || U.RidingHorse)
                            continue;

                        U.ApplyEffect(EnableId);
                    }
                }
            }
            else
            {
                Session.SendWhisper("Por favor, introduzca un ID de efecto.");
                return;
            }
        }
    }
}
