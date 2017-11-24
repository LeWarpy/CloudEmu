using System.Linq;
using Cloud.Communication.Packets.Outgoing.Inventory.Purse;
using Cloud.Database.Interfaces;
using Cloud.HabboHotel.GameClients;
using Cloud.Communication.Packets.Outgoing.Rooms.Notifications;
using System.Text;
using Cloud.Communication.Packets.Outgoing.Notifications;

namespace Cloud.HabboHotel.Rooms.Chat.Commands.Moderator
{
    internal class GlobalGiveCommand : IChatCommand
    {
        public string PermissionRequired => "commandglobal_currency";
        public string Parameters => "[MONEDA] [MONTO]";
        public string Description => "Enviar monedas a todos.";

        public void Execute(GameClient Session, Room room, string[] Params)
        {

            if (Params.Length == 1)
            {
                StringBuilder List = new StringBuilder();
                List.Append("¿Como puedo dar creditos, diamantes, duckets o " + Core.ExtraSettings.PTOS_COINS + "?\n········································································\n");
                List.Append(":globalgive credits [MONTO] - Créditos a todos los usuarios.\n········································································\n");
                List.Append(":globalgive diamonds [MONTO] - Diamantes a todos los usuarios.\n········································································\n");
                List.Append(":globalgive duckets [MONTO] - Duckets a todos los usuarios.\n········································································\n");
                List.Append(":globalgive famepoints [MONTO] - " + Core.ExtraSettings.PTOS_COINS + " a todos los usuarios.\n········································································\n");
                Session.SendMessage(new MOTDNotificationComposer(List.ToString()));
                return;
            }

            string updateVal = Params[1];
            int amount;
            switch (updateVal.ToLower())
            {
                case "coins":
                case "credits":
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_give_coins"))
                        {
                            Session.SendWhisper("Vaya, parece que usted no tiene los permisos necesarios para utilizar este comando!");
                            break;
                        }
                        
                        if (int.TryParse(Params[2], out amount))
                        {
                            foreach (GameClient client in CloudServer.GetGame().GetClientManager().GetClients.ToList())
                            {
                                client.GetHabbo().Credits += amount;
                                client.SendMessage(new CreditBalanceComposer(client.GetHabbo().Credits));
                                client.SendMessage(new RoomNotificationComposer("command_notification_credits", "message", "Recebeu "+amount+" crédito(s) globais!"));
                            }
                            using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
                            {
                                dbClient.runFastQuery("UPDATE users SET credits = credits + " + amount);
                            }
                            break;
                        }
                        Session.SendWhisper("Vaya, que parece ser una cantidad no válida!");
                        break;

                case "pixels":
                case "duckets":
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_give_pixels"))
                        {
                            Session.SendWhisper("Vaya, parece que usted no tiene los permisos necesarios para utilizar este comando!");
                            break;
                        }
                        if (int.TryParse(Params[2], out amount))
                        {
                            foreach (GameClient client in CloudServer.GetGame().GetClientManager().GetClients.ToList())
                            {
                                client.GetHabbo().Duckets += amount;
                                client.SendMessage(new HabboActivityPointNotificationComposer(
                                    client.GetHabbo().Duckets, amount));
                                client.SendMessage(new RoomNotificationComposer("command_notification_credits", "message", "Recebeu " + amount + " ducket(s) globais!"));
                            }
                            using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
                            {
                                dbClient.runFastQuery("UPDATE users SET activity_points = activity_points + " + amount);
                            }
                            break;
                        }
                        Session.SendWhisper("Vaya, que parece ser una cantidad no válida!");
                        break;

                case "diamonds":
                case "diamantes":
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_give_diamonds"))
                        {
                            Session.SendWhisper("Vaya, parece que usted no tiene los permisos necesarios para utilizar este comando!");
                            break;
                        }
                        if (int.TryParse(Params[2], out amount))
                        {
                            foreach (GameClient client in CloudServer.GetGame().GetClientManager().GetClients.ToList())
                            {
                                client.GetHabbo().Diamonds += amount;
                                client.SendMessage(new HabboActivityPointNotificationComposer(client.GetHabbo().Diamonds,
                                    amount,
                                    5));
                            }
                            using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
                            {
                                dbClient.runFastQuery("UPDATE users SET vip_points = vip_points + " + amount);
                            }
                            break;
                        }
                        Session.SendWhisper("Vaya, que parece ser una cantidad no válida!");
                        break;
                case "gotw":
                case "gotwpoints":
                case "fame":
                case "fama":
                case "famepoints":
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_give_gotw"))
                        {
                            Session.SendWhisper("Vaya, parece que usted no tiene los permisos necesarios para utilizar este comando!");
                            break;
                        }
                        if (int.TryParse(Params[2], out amount))
                        {
                            foreach (GameClient client in CloudServer.GetGame().GetClientManager().GetClients.ToList())
                            {
                                client.GetHabbo().GOTWPoints = client.GetHabbo().GOTWPoints + amount;
                                client.SendMessage(new HabboActivityPointNotificationComposer(client.GetHabbo().GOTWPoints,
                                    amount, 103));
                                client.SendMessage(new RoomNotificationComposer("command_notification_credits", "message", "Recebeu " + amount + " "+Core.ExtraSettings.PTOS_COINS+" globais!"));
                            }
                            using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
                            {
                                dbClient.runFastQuery("UPDATE users SET gotw_points = gotw_points + " + amount);
                            }
                            break;
                        }
                        Session.SendWhisper("Vaya, que parece ser una cantidad no válida!");
                        break;
            }
        }
    }
}