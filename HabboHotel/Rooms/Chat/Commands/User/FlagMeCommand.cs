using Cloud.HabboHotel.Users;
using Cloud.Communication.Packets.Outgoing.Handshake;

namespace Cloud.HabboHotel.Rooms.Chat.Commands.User
{
    class FlagMeCommand : IChatCommand
    {
        public string PermissionRequired => "command_flagme";
        public string Parameters => "";
        public string Description => "Mudar nome de usuario.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            
            if (!this.CanChangeName(Session.GetHabbo()))
            {
                Session.SendWhisper("Parece que atualmente nao tem a opçao para mudar seu nome de usuario!");
                return;
            }

            Session.GetHabbo().ChangingName = true;
            Session.SendNotification("Observe que, se o seu nome de usuário é considerado como impróprio, foi proibido sem dúvida.\r\r Observe também que o pessoal não vai mudar seu nome de usuário novamente se você tiver um problema com o que você escolheu.\r\rFechar esta janela e clique em si mesmo para começar a escolher um novo nome de usuário!");
            Session.SendMessage(new UserObjectComposer(Session.GetHabbo()));
        }

        private bool CanChangeName(Habbo Habbo)
        {
            if (Habbo.Rank == 1 && Habbo.VIPRank == 0 && Habbo.LastNameChange == 0)
                return true;
            else if (Habbo.Rank == 1 && Habbo.VIPRank == 1 && (Habbo.LastNameChange == 0 || (CloudServer.GetUnixTimestamp() + 604800) > Habbo.LastNameChange))
                return true;
            else if (Habbo.Rank == 1 && Habbo.VIPRank == 2 && (Habbo.LastNameChange == 0 || (CloudServer.GetUnixTimestamp() + 86400) > Habbo.LastNameChange))
                return true;
            else if (Habbo.Rank == 1 && Habbo.VIPRank == 3)
                return true;
            else if (Habbo.GetPermissions().HasRight("mod_tool"))
                return true;

            return false;
        }
    }
}
