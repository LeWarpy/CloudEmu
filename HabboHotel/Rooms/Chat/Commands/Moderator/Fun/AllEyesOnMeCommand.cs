using System.Linq;
using System.Collections.Generic;
using Cloud.HabboHotel.Rooms.Pathfinding;

namespace Cloud.HabboHotel.Rooms.Chat.Commands.Moderator.Fun
{
    class AllEyesOnMeCommand : IChatCommand
    {
        public string PermissionRequired => "command_alleyesonme";
        public string Parameters => ""; 
        public string Description => "¿Quieres un poco de atención? Hacer todos los que se enfrentan!";

        public void Execute(GameClients.GameClient Session, Room Room, string[] Params)
        {
            RoomUser ThisUser = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (ThisUser == null)
                return;

            List<RoomUser> Users = Room.GetRoomUserManager().GetRoomUsers();
            foreach (RoomUser U in Users.ToList())
            {
                if (U == null || Session.GetHabbo().Id == U.UserId)
                    continue;

                U.SetRot(Rotation.Calculate(U.X, U.Y, ThisUser.X, ThisUser.Y), false);
            }
        }
    }
}
