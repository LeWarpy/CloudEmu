using Cloud.Database.Interfaces;

namespace Cloud.HabboHotel.Rooms.Chat.Commands.User
{
    class MutePetsCommand : IChatCommand
    {
        public string PermissionRequired => "command_mute_pets";
        public string Parameters => "";
        public string Description => "Ignorar ou permitir bot do bate-papo ";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            Session.GetHabbo().AllowPetSpeech = !Session.GetHabbo().AllowPetSpeech;

            using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.runFastQuery("UPDATE `users` SET `pets_muted` = '" + ((Session.GetHabbo().AllowPetSpeech) ? 1 : 0) + "' WHERE `id` = '" + Session.GetHabbo().Id + "' LIMIT 1");
            }

            if (Session.GetHabbo().AllowPetSpeech)
                Session.SendWhisper("Mudança bem sucedida porque você não pode ver os animais de estimação de fala.");
            else
                Session.SendWhisper("Mudança bem sucedida, agora você pode ver os animais de estimação de fala.");
        }
    }
}
