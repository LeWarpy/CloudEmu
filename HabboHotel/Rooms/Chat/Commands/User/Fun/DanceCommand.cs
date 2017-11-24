using Cloud.Communication.Packets.Outgoing.Rooms.Avatar;

namespace Cloud.HabboHotel.Rooms.Chat.Commands.User.Fun
{
    class DanceCommand :IChatCommand
    {
        public string PermissionRequired => "command_dance";
        public string Parameters => "[DANÇA]";
        public string Description => "Use o comando para dança com o ID 0 a 4.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            RoomUser ThisUser = Session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (ThisUser == null)
                return;

            if (Params.Length == 1)
            {
                Session.LogsNotif("Por favor, introduza um ID de uma dança.", "command_notification");
                return;
            }

            int DanceId;
            if (int.TryParse(Params[1], out DanceId))
            {
                if (DanceId > 4 || DanceId < 0)
                {
                    Session.LogsNotif("O ID da dança deve estar entre 0 e 4!", "command_notification");
                    return;
                }

                Session.GetHabbo().CurrentRoom.SendMessage(new DanceComposer(ThisUser, DanceId));
            }
            else
            {
                Session.LogsNotif("Por favor, introduza um ID válido de dança!.", "command_notification");
                return;
            }
        }
    }
}
