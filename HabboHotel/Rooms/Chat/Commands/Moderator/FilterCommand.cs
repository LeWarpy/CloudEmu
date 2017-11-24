using Cloud.Database.Interfaces;

namespace Cloud.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class FilterCommand : IChatCommand
    {

        public string PermissionRequired => "command_filter";
        public string Parameters => "[PALABRA]";
        public string Description => "Añadir palabras al filtro.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            

            if (Params.Length == 1)
            {
                Session.SendWhisper("Por favor, introduzca una palabra.");
                return;
            }

            using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.runFastQuery("INSERT INTO `wordfilter` (id, word, replacement, strict, addedby, bannable) VALUES (NULL, '" + Params[1] + "', '" + CloudServer.HotelName + "', '1', '" + Session.GetHabbo().Username + "', '0')");
            }

            CloudServer.GetGame().GetChatManager().GetFilter().InitWords();
            CloudServer.GetGame().GetChatManager().GetFilter().InitCharacters();
            Session.SendWhisper("Éxito, Sigamos combatiendo  los spammers");
        }
    }
}