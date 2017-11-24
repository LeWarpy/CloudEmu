using System.Linq;
using Cloud.Communication.Packets.Outgoing.Inventory.Purse;
using Cloud.Communication.Packets.Outgoing.Rooms.Notifications;
using Cloud.HabboHotel.GameClients;
using System.Text;
using Cloud.Communication.Packets.Outgoing.Notifications;

namespace Cloud.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class MassGiveCommand : IChatCommand
    {
        public string PermissionRequired => "command_mass_give";
        public string Parameters => "[MONEDA] [MONTO]";
        public string Description => "Dar créditos, duckets, diamantes a todos en la sala.";

        public void Execute(GameClient Session, Room room, string[] Params)
        {


            if (Params.Length == 1)
            {
                StringBuilder List = new StringBuilder();
                List.Append("¿Como puedo dar créditos, diamantes, duckets o " + Core.ExtraSettings.PTOS_COINS + "?\n\n");
                List.Append(":massgive credits [MONTO] - Créditos a todos los usuarios.\n\n");
                List.Append(":massgive diamonds [MONTO] - Diamantes a todos los usuarios.\n\n");
                List.Append(":massgive duckets [MONTO] - Duckets a todos los usuarios.\n\n");
                List.Append(":massgive famepoints [MONTO] - " + Core.ExtraSettings.PTOS_COINS + " a todos los usuarios.\n\n");
                Session.SendMessage(new MOTDNotificationComposer(List.ToString()));
                return;
            }

            var updateVal = Params[1];
            switch (updateVal.ToLower())
            {
                case "coins":
                case "credits":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_give_coins"))
                        {
                            Session.SendWhisper("Vaya, parece que usted no tiene los permisos necesarios para utilizar este comando!");
                            break;
                        }
                        int amount;
                        if (int.TryParse(Params[2], out amount))
                        {
                            foreach (var client in CloudServer.GetGame().GetClientManager().GetClients.ToList().Where(client => client?.GetHabbo() != null && client.GetHabbo().Username != Session.GetHabbo().Username))
                            {
                                client.GetHabbo().Credits = client.GetHabbo().Credits += amount;
                                client.SendMessage(new CreditBalanceComposer(client.GetHabbo().Credits));

                                 client.SendMessage(new RoomNotificationComposer("command_notification_credits", "message", "Has recibido " + amount + " crédito(s) de " + Session.GetHabbo().Username + "!"));
                            }

                            break;
                        }
                        Session.SendWhisper("Vaya, que parece ser una cantidad no válida!");
                        break;
                    }

                case "pixels":
                case "duckets":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_give_pixels"))
                        {
                            Session.SendWhisper("Vaya, parece que usted no tiene los permisos necesarios para utilizar este comando!");
                            break;
                        }
                        int amount;
                        if (int.TryParse(Params[2], out amount))
                        {
                            foreach (var client in CloudServer.GetGame().GetClientManager().GetClients.ToList().Where(client => client?.GetHabbo() != null && client.GetHabbo().Username != Session.GetHabbo().Username))
                            {
                                client.GetHabbo().Duckets += amount;
                                client.SendMessage(new HabboActivityPointNotificationComposer(
                                    client.GetHabbo().Duckets, amount));

                                client.SendMessage(new RoomNotificationComposer("command_notification_credits", "message", "Has recibido " + amount + " ducket(s) de " + Session.GetHabbo().Username + "!"));
                            }
                            break;
                        }
                        Session.SendWhisper("Vaya, que parece ser una cantidad no válida!");
                        break;
                    }

                case "diamonds":
                case "diamantes":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_give_diamonds"))
                        {
                            Session.SendWhisper("Vaya, parece que usted no tiene los permisos necesarios para utilizar este comando!");
                            break;
                        }
                        int amount;
                        if (int.TryParse(Params[2], out amount))
                        {
                            foreach (var client in CloudServer.GetGame().GetClientManager().GetClients.ToList().Where(client => client?.GetHabbo() != null && client.GetHabbo().Username != Session.GetHabbo().Username))
                            {
                                client.GetHabbo().Diamonds += amount;
                                client.SendMessage(new HabboActivityPointNotificationComposer(client.GetHabbo().Diamonds,
                                    amount,
                                    5));

                                client.SendMessage(new RoomNotificationComposer("command_notification_credits", "message", "Has recibido " + amount + " diamante(s) de " + Session.GetHabbo().Username + "!"));
                            }

                            break;
                        }
                        Session.SendWhisper("Vaya, que parece ser una cantidad no válida!");
                        break;
                    }

                case "gotw":
                case "gotwpoints":
                case "fame":
                case "fama":
                case "ptf":
                case "famepoints":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_give_gotw"))
                        {
                            Session.SendWhisper("Vaya, parece que usted no tiene los permisos necesarios para utilizar este comando!");
                            break;
                        }
                        int Amount;
                        if (int.TryParse(Params[2], out Amount))
                        {
                            if (Amount > 50)
                            {
                                Session.SendWhisper("No pueden enviar más de 50 Puntos, esto será notificado al CEO y tomará medidas.");
                                return;
                            }

                            foreach (GameClient Target in CloudServer.GetGame().GetClientManager().GetClients.ToList())
                            {
                                if (Target == null || Target.GetHabbo() == null || Target.GetHabbo().Username == Session.GetHabbo().Username)
                                    continue;

                                Target.GetHabbo().GOTWPoints = Target.GetHabbo().GOTWPoints + Amount;
                                Target.GetHabbo().UserPoints = Target.GetHabbo().UserPoints + 1;
                                Target.SendMessage(new HabboActivityPointNotificationComposer(Target.GetHabbo().GOTWPoints, Amount, 103));

                                if (Target.GetHabbo().Id != Session.GetHabbo().Id)
                                    Target.SendMessage(RoomNotificationComposer.SendBubble("command_notification_credits", "" + Session.GetHabbo().Username + " te acaba de enviar " + Amount + " " + Core.ExtraSettings.PTOS_COINS + ".", "")); /*(RoomNotificationComposer.SendBubble("honor", "" + Session.GetHabbo().Username + " te acaba de enviar " + Amount + " puntos de honor.", ""));*/
                            }

                            break;
                        }
                        else
                        {
                            Session.SendWhisper("Oops, las cantidades solo en numeros..!");
                            break;
                        }
                    }
                case "gotwt":
                case "gotwpointst":
                case "famet":
                case "famat":
                case "ptft":
                case "famepointst":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_give_gotw"))
                        {
                            Session.SendWhisper("Vaya, parece que usted no tiene los permisos necesarios para utilizar este comando!");
                            break;
                        }
                        int amount;
                        if (int.TryParse(Params[2], out amount))
                        {
                            foreach (var client in CloudServer.GetGame().GetClientManager().GetClients.ToList().Where(client => client?.GetHabbo() != null && client.GetHabbo().Username != Session.GetHabbo().Username))
                            {
                                client.GetHabbo().GOTWPoints = client.GetHabbo().GOTWPoints + amount;
                                client.SendMessage(new HabboActivityPointNotificationComposer(client.GetHabbo().GOTWPoints,
                                    amount, 103));

                                client.SendMessage(new RoomNotificationComposer("command_notification_credits", "message", "Has recibido " + amount + " " + Core.ExtraSettings.PTOS_COINS + " de " + Session.GetHabbo().Username + "!"));
                            }
                            break;
                        }
                        Session.SendWhisper("Vaya, que parece ser una cantidad no válida!");
                        break;
                    }
                default:
                    Session.SendWhisper("'" + updateVal + "' is not a valid currency!");
                    break;
            }
        }
    }
}