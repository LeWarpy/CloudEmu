using System;
using Cloud.HabboHotel.GameClients;
using Cloud.Database.Interfaces;
using Cloud.Communication.Packets.Outgoing.Inventory.Purse;

namespace Cloud.Communication.RCON.Commands.User
{
    class ReloadUserCurrencyCommand : IRCONCommand
    {
        public string Description
        {
            get { return "Este comando é usado para atualizar a moeda dos usuários do banco de dados."; }
        }

        public string Parameters
        {
            get { return "%userId% %currency%"; }
        }

        public bool TryExecute(string[] parameters)
        {
            int userId = 0;
            if (!int.TryParse(parameters[0].ToString(), out userId))
                return false;

            GameClient client = CloudServer.GetGame().GetClientManager().GetClientByUserID(userId);
            if (client == null || client.GetHabbo() == null)
                return false;

            // Validate the currency type
            if (string.IsNullOrEmpty(Convert.ToString(parameters[1])))
                return false;

            string currency = Convert.ToString(parameters[1]);

            switch (currency)
            {
                default:
                    return false;

                case "coins":
                case "credits":
                    {
                        int credits = 0;
                        using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("SELECT `credits` FROM `users` WHERE `id` = @id LIMIT 1");
                            dbClient.AddParameter("id", client.GetHabbo().Id);
                            credits = dbClient.getInteger();
                        }

                        client.GetHabbo().Credits = credits;
                        client.SendMessage(new CreditBalanceComposer(client.GetHabbo().Credits));
                        break;
                    }

                case "pixels":
                case "duckets":
                    {
                        int duckets = 0;
                        using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("SELECT `activity_points` FROM `users` WHERE `id` = @id LIMIT 1");
                            dbClient.AddParameter("id", client.GetHabbo().Id);
                            duckets = dbClient.getInteger();
                        }

                        client.GetHabbo().Duckets = duckets;
                        client.SendMessage(new HabboActivityPointNotificationComposer(client.GetHabbo().Duckets, duckets));
                        break;
                    }

                case "diamonds":
                    {
                        int diamonds = 0;
                        using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("SELECT `vip_points` FROM `users` WHERE `id` = @id LIMIT 1");
                            dbClient.AddParameter("id", client.GetHabbo().Id);
                            diamonds = dbClient.getInteger();
                        }

                        client.GetHabbo().Diamonds = diamonds;
                        client.SendMessage(new HabboActivityPointNotificationComposer(diamonds, 0, 5));
                        break;
                    }

                case "gotw":
                    {
                        int gotw = 0;
                        using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("SELECT `gotw_points` FROM `users` WHERE `id` = @id LIMIT 1");
                            dbClient.AddParameter("id", client.GetHabbo().Id);
                            gotw = dbClient.getInteger();
                        }

                        client.GetHabbo().GOTWPoints = gotw;
                        client.SendMessage(new HabboActivityPointNotificationComposer(gotw, 0, 103));
                        break;
                    }
            }
            return true;
        }
    }
}