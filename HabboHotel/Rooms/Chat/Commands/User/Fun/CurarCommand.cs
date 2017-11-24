using Cloud.HabboHotel.Rooms.Games.Teams;

namespace Cloud.HabboHotel.Rooms.Chat.Commands.User.Fun
{
    class CurarCommand : IChatCommand
    {
        public string PermissionRequired => "command_stats";
        public string Parameters => "";
        public string Description => "Curar inchaço para um golpe recebido.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            RoomUser ThisUser = Session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Username);
            if (ThisUser == null)
                return;

            if (ThisUser.RidingHorse)
            {
                Session.SendWhisper("Você não pode se curar de um acidente vascular cerebral, enquanto você monta um cavalo.");
                return;
            }
            else if (ThisUser.Team != TEAM.NONE)
                return;
            else if (ThisUser.isLying)
                return;

            Session.GetHabbo().Effects().ApplyEffect(0);
            Session.SendWhisper("Você foi curado corretamente a partir de um golpe no rosto.");
        }
    }
}
