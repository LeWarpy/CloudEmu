using Cloud.Database.Interfaces;

namespace Cloud.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class DisableForcedFXCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_forced_effects"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Posibilidad de ignorar o permitir efectos forzadas."; }
        }

        public void Execute(GameClients.GameClient Session, Room Room, string[] Params)
        {
            
            Session.GetHabbo().DisableForcedEffects = !Session.GetHabbo().DisableForcedEffects;
            using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `users` SET `disable_forced_effects` = @DisableForcedEffects WHERE `id` = '" + Session.GetHabbo().Id + "' LIMIT 1");
                dbClient.AddParameter("DisableForcedEffects", (Session.GetHabbo().DisableForcedEffects == true ? 1 : 0).ToString());
                dbClient.RunQuery();
            }

            Session.SendWhisper("Modo FX Forzado está " + (Session.GetHabbo().DisableForcedEffects == true ? "desactivado!" : "activado!"));
        }
    }
}
