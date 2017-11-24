using Cloud.Communication.Packets.Outgoing.Rooms.Notifications;
using Cloud.HabboHotel.GameClients;
using Cloud.Communication.Packets.Outgoing.Inventory.Purse;
using System.Text;
using Cloud.Communication.Packets.Outgoing.Notifications;

namespace Cloud.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class GiveCommand : IChatCommand
    {
        public string PermissionRequired => "command_give";
        public string Parameters => "[USUARIO] [MONEDA] [MONTO]";
        public string Description => "Dar créditos, duckets, diamantes a un usuario.";

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            

            if (Params.Length == 1)
            {
                StringBuilder List = new StringBuilder();
                List.Append("¿Como puedo dar creditos, diamantes, duckets o "+ Core.ExtraSettings.PTOS_COINS+ "?\n········································································\n");
                List.Append(":give [USUARIO] credits [MONTO] - Créditos a un usuario.\n········································································\n");
                List.Append(":give [USUARIO] diamonds [MONTO] - Diamantes a un usuario.\n········································································\n");
                List.Append(":give [USUARIO] duckets [MONTO] - Duckets a un usuario.\n········································································\n");
                List.Append(":give [USUARIO] famepoints [MONTO] - " + Core.ExtraSettings.PTOS_COINS + " a un usuario.\n········································································\n");
                Session.SendMessage(new MOTDNotificationComposer(List.ToString()));
                return;
            }

            GameClient Target = CloudServer.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (Target == null)
            {
                Session.SendWhisper("Vaya, no pudo encontrar ese usuario!");
                return;
            }

            string UpdateVal = Params[2];
            switch (UpdateVal.ToLower())
            {
                case "coins":
                case "credits":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_give_coins"))
                        {
                            Session.SendWhisper("Vaya, parece que usted no tiene los permisos necesarios para utilizar este comando!");
                            break;
                        }
                        else
                        {
                            int Amount;
                            if (int.TryParse(Params[3], out Amount))
                            {
                                Target.GetHabbo().Credits = Target.GetHabbo().Credits += Amount;
                                Target.SendMessage(new CreditBalanceComposer(Target.GetHabbo().Credits));

                                Session.SendMessage(new RoomNotificationComposer("command_notification_credits", "message", "Le has dado con exito, " + Amount + " crédito(s) a " + Target.GetHabbo().Username + "!"));
                                Target.SendMessage(new RoomNotificationComposer("command_notification_credits", "message", "Has recibido " + Amount + " crédito(s) de " + Session.GetHabbo().Username + "!"));
                                break;
                            }
                            else
                            {
                                Session.SendWhisper("Vaya, que parece ser una cantidad no válida!");
                                break;
                            }
                        }
                    }

                case "pixels":
                case "duckets":
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_give_pixels"))
                        {
                            Session.SendWhisper("Vaya, parece que usted no tiene los permisos necesarios para utilizar este comando!");
                            break;
                        }
                        else
                        {
                            int Amount;
                            if (int.TryParse(Params[3], out Amount))
                            {
                                Target.GetHabbo().Duckets += Amount;
                                Target.SendMessage(new HabboActivityPointNotificationComposer(Target.GetHabbo().Duckets, Amount));

                                Session.SendMessage(new RoomNotificationComposer("command_notification_credits", "message", "Le has dado con exito, " + Amount + " ducket(s) a " + Target.GetHabbo().Username + "!"));
                                Target.SendMessage(new RoomNotificationComposer("command_notification_credits", "message", "Has recibido " + Amount + " ducket(s) de " + Session.GetHabbo().Username + "!"));
                                break;
                            }
                            else
                            {
                                Session.SendWhisper("Vaya, que parece ser una cantidad no válida!");
                                break;
                            }
                        }

                case "diamonds":
                case "diamantes":
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_give_diamonds"))
                        {
                            Session.SendWhisper("Vaya, parece que usted no tiene los permisos necesarios para utilizar este comando!");
                            break;
                        }
                        else
                        {
                            int Amount;
                            if (int.TryParse(Params[3], out Amount))
                            {
                                Target.GetHabbo().Diamonds += Amount;
                                Target.SendMessage(new HabboActivityPointNotificationComposer(Target.GetHabbo().Diamonds, Amount, 5));
                                Session.SendMessage(new RoomNotificationComposer("command_notification_credits", "message", "Voce recebeu, " + Amount + " diamante(s) a " + Target.GetHabbo().Username + "!"));
                                Target.SendMessage(new RoomNotificationComposer("command_notification_credits", "message", "Voce recebeu " + Amount + " diamante(s) de " + Session.GetHabbo().Username + "!"));
                                break;
                            }
                            else
                            {
                                Session.SendWhisper("Vaya, que parece ser una cantidad no válida!");
                                break;
                            }
                        }

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
                        else
                        {
                            int Amount;
                        if (int.TryParse(Params[3], out Amount))
                        {
                            if (Amount > 500)
                            {
                                Session.SendWhisper("No pueden enviar más de 500 puntos, esto será notificado a los CEO y se tomarán las medidas oportunas.");
                                return;
                            }

                            Target.GetHabbo().GOTWPoints = Target.GetHabbo().GOTWPoints + Amount;
                            Target.GetHabbo().UserPoints = Target.GetHabbo().UserPoints + 1;
                            Target.SendMessage(new HabboActivityPointNotificationComposer(Target.GetHabbo().GOTWPoints, Amount, 103));

                            if (Target.GetHabbo().Id != Session.GetHabbo().Id)
                                Target.SendMessage(RoomNotificationComposer.SendBubble("eventoxx", "" + Session.GetHabbo().Username + " te acaba de enviar " + Amount + " " + Core.ExtraSettings.PTOS_COINS + ".\nHaz click para ver los premios disponibles.", "catalog/open/habbiween"));
                            Session.SendMessage(RoomNotificationComposer.SendBubble("eventoxx", "Acabas de enviar " + Amount + " " + Core.ExtraSettings.PTOS_COINS + " " + Target.GetHabbo().Username + "\nRecuerda que hemos depositado tu confianza en tí y que estos comandos los vemos en directo.", "catalog/open/habbiween"));
                            CloudServer.GetGame().GetAchievementManager().ProgressAchievement(Target, "ACH_EventsWon", 1);
                            break;
                        }
                        else
                        {
                            Session.SendWhisper("Sólo puedes introducir parámetros numerales, de 1 a 50.");
                            break;
                        }
                    }
                case "gotwt":
                case "gotwpointst":
                case "famet":
                case "famat":
                case "famepointst":
                    if (!Session.GetHabbo().GetPermissions().HasCommand("command_give_gotw"))
                    {
                        Session.SendWhisper("Vaya, parece que usted no tiene los permisos necesarios para utilizar este comando!");
                        break;
                    }
                    else
                    {
                        int Amount;
                        if (int.TryParse(Params[3], out Amount))
                        {
                            Target.GetHabbo().GOTWPoints = Target.GetHabbo().GOTWPoints + Amount;
                            Target.SendMessage(new HabboActivityPointNotificationComposer(Target.GetHabbo().GOTWPoints, Amount, 103));

                            Session.SendMessage(new RoomNotificationComposer("command_notification_credits", "message", "Le has dado con exito, " + Amount + " " + Core.ExtraSettings.PTOS_COINS + " a " + Target.GetHabbo().Username + "!"));
                            Target.SendMessage(new RoomNotificationComposer("command_notification_credits", "message", "Has recibido " + Amount + " " + Core.ExtraSettings.PTOS_COINS + " de " + Session.GetHabbo().Username + "!"));
                            break;
                        }
                        else
                        {
                            Session.SendWhisper("Vaya, que parece ser una cantidad no válida!");
                            break;
                        }
                    }
                default:
                    Session.SendWhisper("'" + UpdateVal + "' no es una moneda válida!");
                    break;
            }
        }
    }
}