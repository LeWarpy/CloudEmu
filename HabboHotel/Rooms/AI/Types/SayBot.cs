using System;
using Cloud.HabboHotel.GameClients;
using System.Data;
using Cloud.Database.Interfaces;
using System.Drawing;
using Cloud.Communication.Packets.Outgoing.Inventory.Purse;
using Cloud.Communication.Packets.Outgoing.Rooms.Engine;
using Cloud.HabboHotel.Rooms.AI.Speech;

namespace Cloud.HabboHotel.Rooms.AI.Types
{
    class SayBot : BotAI
    {
        private int VirtualId;
        private static readonly Random Random = new Random();
        private int ActionTimer = 0;
        private int SpeechTimer = 0;

        public SayBot(int VirtualId)
        {
            this.VirtualId = VirtualId;
        }

        public override void OnSelfEnterRoom()
        {
        }

        public override void OnSelfLeaveRoom(bool Kicked)
        {
        }

        public override void OnUserEnterRoom(RoomUser User)
        {
        }


        public override void OnUserLeaveRoom(GameClient Client)
        {
        }

        public override void OnUserSay(RoomUser User, string Message)
        {
            if (User == null || User.GetClient() == null || User.GetClient().GetHabbo() == null)
                return;

            if (GetBotData() == null)
                return;

            if (Gamemap.TileDistance(GetRoomUser().X, GetRoomUser().Y, User.X, User.Y) > 8)
                return;

            string Command = Message.Substring(GetBotData().Name.ToLower().Length + 1);

            if ((Message.ToLower().StartsWith(GetBotData().Name.ToLower() + " ")))
            {
                switch (Command)
                {
                    case "ven":
                    case "comehere":
                    case "come here":
                    case "ven aqui":
                    case "come":
                        GetRoomUser().Chat("¡Voy!", false, 0);
                        GetRoomUser().MoveTo(User.SquareInFront);
                        break;

                    case "sirve":
                    case "serve":
                        if (GetRoom().CheckRights(User.GetClient()))
                        {
                            foreach (var current in GetRoom().GetRoomUserManager().GetRoomUsers()) current.CarryItem(Random.Next(1, 38));
                            GetRoomUser().Chat("Vale. Ya teneis todos algo para zampar.", false, 0);
                            return;
                        }
                        break;
                    case "agua":
                    case "té":
                    case "te":
                    case "tea":
                    case "juice":
                    case "water":
                    case "zumo":
                        GetRoomUser().Chat("Aquí tienes.", false, 0);
                        User.CarryItem(Random.Next(1, 3));
                        break;

                    case "helado":
                    case "icecream":
                    case "ice cream":
                        GetRoomUser().Chat("Aquí tienes. ¡Que no se te quede pegada la lengua, je je!", false, 0);
                        User.CarryItem(4);
                        break;

                    case "rose":
                    case "rosa":
                        GetRoomUser().Chat("Aquí tienes... que te vaya bien en tu cita.", false, 0);
                        User.CarryItem(Random.Next(1000, 1002));
                        break;

                    case "girasol":
                    case "sunflower":
                        GetRoomUser().Chat("Aquí tienes algo muy bonito de la naturaleza.", false, 0);
                        User.CarryItem(1002);
                        break;

                    case "flor":
                    case "flower":
                        GetRoomUser().Chat("Aquí tienes algo muy bonito de la naturaleza.", false, 0);
                        if (Random.Next(1, 3) == 2)
                        {
                            User.CarryItem(Random.Next(1019, 1024));
                            return;
                        }
                        User.CarryItem(Random.Next(1006, 1010));
                        break;

                    case "zanahoria":
                    case "zana":
                    case "carrot":
                        GetRoomUser().Chat("Aquí tienes una buena verdura. ¡Provecho!", false, 0);
                        User.CarryItem(3);
                        break;

                    case "café":
                    case "cafe":
                    case "capuccino":
                    case "coffee":
                    case "latte":
                    case "mocha":
                    case "espresso":
                    case "expreso":
                        GetRoomUser().Chat("Aquí tienes tu café. ¡Está espumoso!", false, 0);
                        User.CarryItem(Random.Next(11, 18));
                        break;

                    case "fruta":
                    case "fruit":
                        GetRoomUser().Chat("Aquí tienes algo sano, fresco y natural. ¡Que lo disfrutes!", false, 0);
                        User.CarryItem(Random.Next(36, 40));
                        break;

                    case "naranja":
                    case "orange":
                        GetRoomUser().Chat("Aquí tienes algo sano, fresco y natural. ¡Que lo disfrutes!", false, 0);
                        User.CarryItem(38);
                        break;

                    case "manzana":
                    case "apple":
                        GetRoomUser().Chat("Aquí tienes algo sano, fresco y natural. ¡Que lo disfrutes!", false, 0);
                        User.CarryItem(37);
                        break;

                    case "cola":
                    case "habbocola":
                    case "cocacola":
                    case "coca cola":
                    case "coca-cola":
                    case "habbo cola":
                    case "habbo-cola":
                        GetRoomUser().Chat("Aquí tienes un refresco bastante famoso.", false, 0);
                        User.CarryItem(19);
                        break;

                    case "pear":
                    case "pera":
                        GetRoomUser().Chat("Aquí tienes algo sano, fresco y natural. ¡Que lo disfrutes!", false, 0);
                        User.CarryItem(36);
                        break;

                    case "ananá":
                    case "pineapple":
                    case "piña":
                    case "rodaja de piña":
                        GetRoomUser().Chat("Aquí tienes algo sano, fresco y natural. ¡Que lo disfrutes!", false, 0);
                        User.CarryItem(39);
                        break;

                    case "puta":
                    case "puto":
                    case "gilipollas":
                    case "metemela":
                    case "polla":
                    case "pene":
                    case "penis":
                    case "idiot":
                    case "fuck":
                    case "bastardo":
                    case "idiota":
                    case "chupamela":
                    case "tonta":
                    case "tonto":
                    case "mierda":
                        GetRoomUser().Chat("¡No me trates así, eh!", true, 0);
                        break;

                    case "casate conmigo":
                        GetRoomUser().Chat("Ire ahora!", true, 0);
                        break;

                    case "protocolo destruir":
                        GetRoomUser().Chat("Iniciando Auto Destrucción del Mundo", true, 0);
                        break;

                    case "lindo":
                    case "hermoso":
                    case "linda":
                    case "guapa":
                    case "beautiful":
                    case "handsome":
                    case "love":
                    case "guapo":
                    case "i love you":
                    case "hermosa":
                    case "preciosa":
                    case "teamo":
                    case "amor":
                    case "miamor":
                    case "mi amor":
                        GetRoomUser().Chat("Soy un bot, err... esto se está poniendo incómodo, ¿sabes?", false, 0);
                        break;
                    case "chupala":
                        if (User.GetClient().GetHabbo().Credits < 500)
                        {
                            GetRoomUser().Chat("Opps no tienes creditos suficientes para recibir lo tuyo... necesitas 500$", false, 0);
                            return;
                        }
                        GetRoomUser().Chat("* Chupar a " + User.GetClient().GetHabbo().Username + " *", false, 0);
                        User.Chat("* Oh seh BUENA CHUPADA! ohh seh!! *", false, 0);
                        User.GetClient().GetHabbo().Credits = User.GetClient().GetHabbo().Credits - 500;
                        User.GetClient().SendMessage(new CreditBalanceComposer(User.GetClient().GetHabbo().Credits));
                        break;
                    case "pajilla":
                        if (User.GetClient().GetHabbo().Credits < 300)
                        {
                            GetRoomUser().Chat("Opps no tienes creditos suficientes para recibir lo tuyo... necesitas 300$", false, 0);
                            return;
                        }
                        GetRoomUser().Chat("* Haciendo una pajilla a " + User.GetClient().GetHabbo().Username + " *", false, 0);
                        User.Chat("* Oh seh baby! ohh seh!! *", false, 0);
                        User.GetClient().GetHabbo().Credits = User.GetClient().GetHabbo().Credits - 300;
                        User.GetClient().SendMessage(new CreditBalanceComposer(User.GetClient().GetHabbo().Credits));
                        break;
                    case "juangomez":
                    case "JuanGomez":
                        GetRoomUser().Chat("Alabado sea el Sr. JuanGomez!", true, 34);
                        break;
                }
            }
        }

        public override void OnUserShout(RoomUser User, string Message)
        {

        }


        public override void OnTimerTick()
        {
            if (GetBotData() == null)
                return;

            if (SpeechTimer <= 0)
            {
                if (GetBotData().RandomSpeech.Count > 0)
                {
                    if (GetBotData().AutomaticChat == false)
                        return;

                    RandomSpeech Speech = GetBotData().GetRandomSpeech();

					string String = CloudServer.GetGame().GetChatManager().GetFilter().IsUnnaceptableWord(Speech.Message, out string word) ? "Spam" : Speech.Message;
					if (String.Contains("<img src") || String.Contains("<font ") || String.Contains("</font>") || String.Contains("</a>") || String.Contains("<i>"))
                        String = "I really shouldn't be using HTML within bot speeches.";
                    GetRoomUser().Chat(String, false, GetBotData().ChatBubble);
                }
                SpeechTimer = GetBotData().SpeakingInterval;
            }
            else
                SpeechTimer--;

            if (ActionTimer <= 0)
            {
                Point nextCoord;
                switch (GetBotData().WalkingMode.ToLower())
                {
                    default:
                    case "stand":
                        // (8) Why is my life so boring?
                        break;

                    case "freeroam":
                        if (GetBotData().ForcedMovement)
                        {
                            if (GetRoomUser().Coordinate == GetBotData().TargetCoordinate)
                            {
                                GetBotData().ForcedMovement = false;
                                GetBotData().TargetCoordinate = new Point();

                                GetRoomUser().MoveTo(GetBotData().TargetCoordinate.X, GetBotData().TargetCoordinate.Y);
                            }
                        }
                        else if (GetBotData().ForcedUserTargetMovement > 0)
                        {
                            RoomUser Target = GetRoom().GetRoomUserManager().GetRoomUserByHabbo(GetBotData().ForcedUserTargetMovement);
                            if (Target == null)
                            {
                                GetBotData().ForcedUserTargetMovement = 0;
                                GetRoomUser().ClearMovement(true);
                            }
                            else
                            {
                                var Sq = new Point(Target.X, Target.Y);

                                if (Target.RotBody == 0)
                                {
                                    Sq.Y--;
                                }
                                else if (Target.RotBody == 2)
                                {
                                    Sq.X++;
                                }
                                else if (Target.RotBody == 4)
                                {
                                    Sq.Y++;
                                }
                                else if (Target.RotBody == 6)
                                {
                                    Sq.X--;
                                }


                                GetRoomUser().MoveTo(Sq);
                            }
                        }
                        else if (GetBotData().TargetUser == 0)
                        {
                            nextCoord = GetRoom().GetGameMap().GetRandomWalkableSquare();
                            GetRoomUser().MoveTo(nextCoord.X, nextCoord.Y);
                        }
                        break;

                    case "specified_range":

                        break;
                }

                ActionTimer = new Random(DateTime.Now.Millisecond + this.VirtualId ^ 2).Next(5, 15);
            }
            else
                ActionTimer--;
        }
    }
}