using System;
using Cloud.HabboHotel.GameClients;
using Cloud.Database.Interfaces;
using Cloud.Communication.Packets.Outgoing.Inventory.Purse;

namespace Cloud.Communication.RCON.Commands.User
{
    class GiveUserCurrencyCommand : IRCONCommand
    {
        public string Description
        {
            get { return "Este comando é usado para dar ao usuário uma quantidade especificada de uma moeda específica."; }
        }

        public string Parameters
        {
            get { return "%userId% %currency% %amount%"; }
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

            int amount = 0;
            if (!int.TryParse(parameters[2].ToString(), out amount))
                return false;

            switch (currency)
            {
                default:
                    return false;

                case "coins":
                case "credits":
                case "creditos":
                    {
                        client.GetHabbo().Credits += amount;

                        using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("UPDATE `users` SET `credits` = @credits WHERE `id` = @id LIMIT 1");
                            dbClient.AddParameter("credits", client.GetHabbo().Credits);
                            dbClient.AddParameter("id", client.GetHabbo().Id);
                            dbClient.RunQuery();
                        }

                        client.SendMessage(new CreditBalanceComposer(client.GetHabbo().Credits));
                        break;
                    }

                case "pixels":
                case "duckets":
                    {
                        client.GetHabbo().Duckets += amount;

                        using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("UPDATE `users` SET `activity_points` = @duckets WHERE `id` = @id LIMIT 1");
                            dbClient.AddParameter("duckets", client.GetHabbo().Duckets);
                            dbClient.AddParameter("id", client.GetHabbo().Id);
                            dbClient.RunQuery();
                        }

                        client.SendMessage(new HabboActivityPointNotificationComposer(client.GetHabbo().Duckets, amount));
                        break;
                    }

                case "diamonds":
                case "diamantes":
                    {
                        client.GetHabbo().Diamonds += amount;

                        using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("UPDATE `users` SET `vip_points` = @diamonds WHERE `id` = @id LIMIT 1");
                            dbClient.AddParameter("diamonds", client.GetHabbo().Diamonds);
                            dbClient.AddParameter("id", client.GetHabbo().Id);
                            dbClient.RunQuery();
                        }

                        client.SendMessage(new HabboActivityPointNotificationComposer(client.GetHabbo().Diamonds, 0, 5));
                        break;
                    }

                case "gotw":
                case "gotw_points":
                case "gotwpoints":
                case "fame":
                case "fama":
                case "famepoints":
                    {
                        client.GetHabbo().GOTWPoints += amount;

                        using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("UPDATE `users` SET `gotw_points` = @gotw WHERE `id` = @id LIMIT 1");
                            dbClient.AddParameter("gotw", client.GetHabbo().GOTWPoints);
                            dbClient.AddParameter("id", client.GetHabbo().Id);
                            dbClient.RunQuery();
                        }

                        client.SendMessage(new HabboActivityPointNotificationComposer(client.GetHabbo().GOTWPoints, 0, 103));
                        break;
                    }
            }
            return true;
        }
    }
}