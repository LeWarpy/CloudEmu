using Cloud.Database.Interfaces;
using Cloud.HabboHotel.Users;

namespace Cloud.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class MuteCommand : IChatCommand
    {
        public string PermissionRequired => "command_mute";
        public string Parameters => "[USUARIO] [TIEMPO]";
        public string Description => "Mutear al usuario por un tiempo.";

        public void Execute(GameClients.GameClient Session, Room Room, string[] Params)
        {
            
            if (Params.Length == 1)
            {
                Session.SendWhisper("Por favor, introduzca un nombre de usuario y una hora válida en segundos (máx 600, nada más se vuelve a establecer en 600).");
                return;
            }

            Habbo Habbo = CloudServer.GetHabboByUsername(Params[1]);
            if (Habbo == null)
            {
                Session.SendWhisper("Se produjo un error mientras que la búsqueda de usuario en la base de datos.");
                return;
            }

            if (Habbo.GetPermissions().HasRight("mod_tool") && !Session.GetHabbo().GetPermissions().HasRight("mod_mute_any"))
            {
                Session.SendWhisper("Vaya, no se puede mutear ese usuario.");
                return;
            }

			if (double.TryParse(Params[2], out double Time))
			{
				if (Time > 600 && !Session.GetHabbo().GetPermissions().HasRight("mod_mute_limit_override"))
					Time = 600;

				using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
				{
					dbClient.runFastQuery("UPDATE `users` SET `time_muted` = '" + Time + "' WHERE `id` = '" + Habbo.Id + "' LIMIT 1");
				}

				if (Habbo.GetClient() != null)
				{
					Habbo.TimeMuted = Time;
					Habbo.GetClient().SendNotification("Fuiste muteado por un moderador por " + Time + " segundos!");
				}

				Session.SendWhisper("Usted muteo a: " + Habbo.Username + " por " + Time + " segundos.");
			}
			else
				Session.SendWhisper("Por favor, introduzca un número entero válido.");
		}
    }
}