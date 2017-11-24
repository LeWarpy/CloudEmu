using System.Linq;
using System.Collections.Generic;

namespace Cloud.HabboHotel.Rooms.Chat.Commands.Moderator.Fun
{
    class AllAroundMeCommand : IChatCommand
    {
        public string PermissionRequired => "command_allaroundme";
        public string Parameters => "";
        public string Description => "Necesitas atención? Pon todos los ojos en ti.";

        public void Execute(GameClients.GameClient Session, Room Room, string[] Params)
        {
            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == null)
                return;

            List<RoomUser> Users = Room.GetRoomUserManager().GetRoomUsers();
            foreach (RoomUser U in Users.ToList())
            {
                if (U == null || Session.GetHabbo().Id == U.UserId)
                    continue;

                U.MoveTo(User.X, User.Y, true);
            }
        }
    }
}
