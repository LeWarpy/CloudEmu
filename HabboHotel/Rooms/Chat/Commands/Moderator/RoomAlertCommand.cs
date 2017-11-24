namespace Cloud.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class RoomAlertCommand : IChatCommand
    {
        public string PermissionRequired => "command_room_alert";
        public string Parameters => "[MENSAJE]";
        public string Description => "Enviar mensaje a todos en la sala.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Por favor, introduzca un mensaje que le gustaría enviar a la habitación.");
                return;
            }

            if(!Session.GetHabbo().GetPermissions().HasRight("mod_alert") && Room.OwnerId != Session.GetHabbo().Id)
            {
                Session.SendWhisper("Sólo se puede Habitación de alerta en su propia habitación!");
                return;
            }

            string Message = CommandManager.MergeParams(Params, 1);
            foreach (RoomUser RoomUser in Room.GetRoomUserManager().GetRoomUsers())
            {
                if (RoomUser == null || RoomUser.GetClient() == null || Session.GetHabbo().Id == RoomUser.UserId)
                    continue;

                RoomUser.GetClient().SendNotification(Session.GetHabbo().Username + " alerto a la sala con el siguiente mensaje:\n\n" + Message);
            }
            Session.SendWhisper("Mensaje enviado con éxito a la habitación.");
        }
    }
}
