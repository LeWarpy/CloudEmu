using Cloud.HabboHotel.Rooms.Games.Teams;

namespace Cloud.HabboHotel.Rooms.Chat.Commands.User.Fun
{
    class EnableCommand : IChatCommand
    {
        public string PermissionRequired => "command_enable";
        public string Parameters => "[ID]";
        public string Description => "Habilita um efeito!";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Você deve escrever um ID de efeito");
                return;
            }


            RoomUser ThisUser = Session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Username);
            if (ThisUser == null)
                return;

            if (ThisUser.RidingHorse)
            {
                Session.SendWhisper("Não pode ativar um efeito em cima do cavalo!");
                return;
            }
            else if (ThisUser.Team != TEAM.NONE)
                return;
            else if (ThisUser.isLying)
                return;

            int EffectId = 0;
            if (!int.TryParse(Params[1], out EffectId))
                return;

            if (EffectId > int.MaxValue || EffectId < int.MinValue)
                return;

            if ((EffectId == 102 || EffectId == 187 || EffectId == 593 || EffectId == 596 || EffectId == 598) && !Session.GetHabbo().GetPermissions().HasRight("mod_tool"))
            {
                Session.SendWhisper("Sentimos muito, somente os staff's podem usar esse efeito!");
                return;
            }

            if ((EffectId == 592 || EffectId == 595 || EffectId == 597 && Session.GetHabbo()._guidelevel < 1))
            {
                Session.SendWhisper("Sentimos muito, somente membros da equipe guia podem usar esse comando!");
                return;
            }

            if (EffectId == 594 && Session.GetHabbo()._croupier < 1)
            {
                Session.SendWhisper("Sentimos muito, somente membros da equipe Croupier podem usar esse comando!");
                return;
            }

            if (EffectId == 599 && Session.GetHabbo()._builder < 1)
            {
                Session.SendWhisper("Sentimos muito, somente membros da equipe arquiteto podem usar esse comando!");
                return;
            }

            if (EffectId == 44 && (Session.GetHabbo().Rank < 2))
            {
                Session.SendWhisper("Sentimos muito, somente membros VIP's podem usar esse comando!");
                return;
            }

            if (EffectId == 178 && (!Session.GetHabbo().GetPermissions().HasRight("gold_vip") && !Session.GetHabbo().GetPermissions().HasRight("events_staff")))
            {
                Session.SendWhisper("Sentimos muito, somente ajudantes podem usar esse comando!");
                return;
            }

            Session.GetHabbo().Effects().ApplyEffect(EffectId);
        }
    }
}
