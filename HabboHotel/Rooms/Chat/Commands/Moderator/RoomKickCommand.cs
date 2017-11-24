using System.Linq;

namespace Cloud.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class RoomKickCommand : IChatCommand
    {
        public string PermissionRequired => "command_room_kick";
        public string Parameters => "[MENSAJE]";
        public string Description => "Expulsar a todos los usuarios de estas ala.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            
            if (Params.Length == 1)
            {
                Session.SendWhisper("Por favor, proporcione una razón para los usuarios de esta patada habitación.");
                return;
            }

            string Message = CommandManager.MergeParams(Params, 1);
            foreach (RoomUser RoomUser in Room.GetRoomUserManager().GetUserList().ToList())
            {
                if (RoomUser == null || RoomUser.IsBot || RoomUser.GetClient() == null || RoomUser.GetClient().GetHabbo() == null || RoomUser.GetClient().GetHabbo().GetPermissions().HasRight("mod_tool") || RoomUser.GetClient().GetHabbo().Id == Session.GetHabbo().Id)
                    continue;

                RoomUser.GetClient().SendNotification("Se le ha pateado por un moderador: " + Message);

                Room.GetRoomUserManager().RemoveUserFromRoom(RoomUser.GetClient(), true, false);
            }

            Session.SendWhisper("Pateado con éxito todos los usuarios de la sala.");
        }
    }
}
