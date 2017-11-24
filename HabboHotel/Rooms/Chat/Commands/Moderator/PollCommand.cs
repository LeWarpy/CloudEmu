using Cloud.HabboHotel.GameClients;


namespace Cloud.HabboHotel.Rooms.Chat.Commands.Moderator
{
    internal class PollCommand : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length == 0)
            {
                Session.SendWhisper("Por favor, apresente a pergunta");
            }
            else
            {

                string quest = CommandManager.MergeParams(Params, 1);
                if (quest == "end")
                {
                    Room.EndQuestion();
                }
                else
                {
                    Room.StartQuestion(quest);
                }

            }
        }

        public string Description =>
            "Realize uma pesquisa imediata.";

        public string Parameters =>
            "%question%";

        public string PermissionRequired =>
            "command_give_badge";
    }
}