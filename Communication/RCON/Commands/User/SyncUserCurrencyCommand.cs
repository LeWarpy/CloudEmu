using System;
using Cloud.HabboHotel.GameClients;
using Cloud.Database.Interfaces;
using Cloud.Communication.Packets.Outgoing.Inventory.Purse;

namespace Cloud.Communication.RCON.Commands.User
{
    class SyncUserCurrencyCommand : IRCONCommand
    {
        public string Description
        {
            get { return "Este comando é usado para sincronizar uma moeda especificada pelos usuários no banco de dados."; }
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
                case "creditos":
                    {
                        using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("UPDATE `users` SET `credits` = @credits WHERE `id` = @id LIMIT 1");
                            dbClient.AddParameter("credits", client.GetHabbo().Credits);
                            dbClient.AddParameter("id", client.GetHabbo().Id);
                            dbClient.RunQuery();
                        }
                        break;
                    }

                case "pixels":
                case "duckets":
                    {
                        using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("UPDATE `users` SET `activity_points` = @duckets WHERE `id` = @id LIMIT 1");
                            dbClient.AddParameter("duckets", client.GetHabbo().Duckets);
                            dbClient.AddParameter("id", client.GetHabbo().Id);
                            dbClient.RunQuery();
                        }
                        break;
                    }

                case "diamonds":
                case "diamantes":
                    {
                        using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("UPDATE `users` SET `vip_points` = @diamonds WHERE `id` = @id LIMIT 1");
                            dbClient.AddParameter("diamonds", client.GetHabbo().Diamonds);
                            dbClient.AddParameter("id", client.GetHabbo().Id);
                            dbClient.RunQuery();
                        }
                        break;
                    }

                case "gotw":
                case "gotw_points":
                case "gotwpoints":
                case "fame":
                case "fama":
                case "famepoints":
                    {
                        using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("UPDATE `users` SET `gotw_points` = @gotw WHERE `id` = @id LIMIT 1");
                            dbClient.AddParameter("gotw", client.GetHabbo().GOTWPoints);
                            dbClient.AddParameter("id", client.GetHabbo().Id);
                            dbClient.RunQuery();
                        }
                        break;
                    }
            }
            return true;
        }
    }
}