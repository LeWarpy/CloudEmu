namespace Cloud.HabboHotel.Rooms.Chat.Commands.User
{
    class SetSpeedCommand : IChatCommand
    {
        public string PermissionRequired => "command_setspeed";
        public string Parameters => "[VELOCIDAD]";
        public string Description => "Velocidade dos rollers.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {

            if (!Room.CheckRights(Session, true))
                return;

            if (Params.Length == 1)
            {
                Session.SendWhisper("Por favor, introduza uma velocidade para o roller.");
                return;
            }

            int Speed;
            if (int.TryParse(Params[1], out Speed))
            {
                Session.GetHabbo().CurrentRoom.GetRoomItemHandler().SetSpeed(Speed);
            }
            else
                Session.SendWhisper("número inválido, por favor digite um número válido.");
        }
    }
}