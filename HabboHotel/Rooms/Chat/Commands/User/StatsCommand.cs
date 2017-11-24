using System;
using System.Text;

namespace Cloud.HabboHotel.Rooms.Chat.Commands.User
{
    class StatsCommand : IChatCommand
    {
        public string PermissionRequired => "command_stats";
        public string Parameters => "";
        public string Description => "Ver as estatísticas atuais.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            double Minutes = Session.GetHabbo().GetStats().OnlineTime / 60;
            double Hours = Minutes / 60;
            int OnlineTime = Convert.ToInt32(Hours);
            string s = OnlineTime == 1 ? "" : "s";

            StringBuilder HabboInfo = new StringBuilder();
            HabboInfo.Append("Estatísticas da sua conta são:\r\r");

            HabboInfo.Append("Informações de moedas:\r");
            HabboInfo.Append("Créditos: " + Session.GetHabbo().Credits + "\r");
            HabboInfo.Append("Duckets: " + Session.GetHabbo().Duckets + "\r");
            HabboInfo.Append("Diamantes: " + Session.GetHabbo().Diamonds + "\r");
            HabboInfo.Append("tempo online: " + OnlineTime + " Hour" + s + "\r");
            HabboInfo.Append("lembranças: " + Session.GetHabbo().GetStats().Respect + "\r");
            HabboInfo.Append("Pontos GOTW: " + Session.GetHabbo().GOTWPoints + "\r\r");


            Session.SendNotification(HabboInfo.ToString());
        }
    }
}
