using Cloud.HabboHotel.GameClients;
using Cloud.Database.Interfaces;
using Cloud.Communication.Packets.Outgoing.Rooms.Notifications;
using Cloud.Utilities;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using Cloud.Communication.Packets.Outgoing.Inventory.Purse;

namespace Cloud.HabboHotel.Rooms.AI.Types
{
    class CasinoRoullete : BotAI
    {
        private int VirtualId;
        int GameLength = 100;
        bool BetsOpen = false;
        int Bets;
        private double offerMultiplier;

        private Dictionary<int, CasinoDataLocura> Data1;

        public CasinoRoullete(int VirtualId)
        {
            this.VirtualId = VirtualId;
            Data1 = new Dictionary<int, CasinoDataLocura>();
            Bets = 1;
            offerMultiplier = 1.2;
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

            if (Gamemap.TileDistance(GetRoomUser().X, GetRoomUser().Y, User.X, User.Y) > 8)
                return;

            // Notice that the bet is off to the users.
            if (BetsOpen == false)
            {
                GetRoomUser().Chat("Las apuestas están cerradas para esta partida, debes esperar todavía " + GameLength + " segundos para apostar.", false, 33);
                return;
            }

            if (User.GetClient().GetHabbo().Diamonds >= 2 && BetsOpen == true)
            {

                // Define all the betting variables.
                string Multiplier = Message.Split(' ')[0];
                string Bet = Message.Split(' ')[2];

                int IntMultiplier = 0;
                // Check if the multiplier is a number.
                if (!int.TryParse(Multiplier, out IntMultiplier))
                {
                    GetRoomUser().Chat(Multiplier + " no es un valor apto para la apuesta.", false, 33);
                    return;
                }

                int IntBet = 0;
                // Check if the bet is a number.
                if (!int.TryParse(Bet, out IntBet))
                {
                    GetRoomUser().Chat(Bet + " no es un valor apto para la apuesta.", false, 33);
                    return;
                }

                // Check if the bet doesn't exceed the limits.
                if (IntBet < 0 || IntBet > 36)
                {
                    GetRoomUser().Chat("Debes apostar entre el 0 y el 36, no puedes salir de esos límites.", false, 33);
                    return;
                }

                User.GetClient().GetHabbo().Diamonds = User.GetClient().GetHabbo().Diamonds - IntMultiplier;
                User.GetClient().SendMessage(new HabboActivityPointNotificationComposer(User.GetClient().GetHabbo().Diamonds, -IntMultiplier, 5));
                User.GetClient().SendMessage(RoomNotificationComposer.SendBubble("eventoxx", User.GetClient().GetHabbo().Username + ", acabas de apostar " + IntMultiplier + " diamantes al " + IntBet +".\n\n Buena suerte y apuesta con cabeza.", "catalog/open/habbiween"));

                GetRoomUser().Chat("Apuesta de " + IntMultiplier + " diamantes realizada al " + IntBet +" por "+ User.GetClient().GetHabbo().Username +".", false, 33);

                offerMultiplier += .2;
                
                CasinoDataLocura data = new CasinoDataLocura();
                data.bet = IntBet;
                data.quantity = IntMultiplier;
                data.userId = User.GetClient().GetHabbo().Id;


                Data1.Add(Bets, data);

                // Notify BET Id to the user.
                User.GetClient().SendShout(Bets +" es tu número de apuesta.", 33);

                Bets++;


            }

            
        }

        public class CasinoDataLocura
        {
            public int userId;
            public int quantity;
            public int bet;
            
        }

        public override void OnUserShout(RoomUser User, string Message)
        {
        }

        public override void OnTimerTick()
        {

            if (GameLength == 95)
            {
                // Notice to users that betting is enabled during this timelapse.
                GetRoomUser().Chat("Apuestas activas, abran juego.", false, 33);
                BetsOpen = true;
            }

            if (GameLength == 15)
            {
                // Notice to users that betting is enabled during this timelapse.
                
                GetRoomUser().Chat("Apuestas cerradas, buena suerte.", false, 33);
                BetsOpen = false;
            }

            if (GameLength <= 0)
            {
                // Generate the number from the roullette.
                #region Serialize Numbers

                int num = RandomNumber.GenerateRandom(0,36);

                switch (num)
                {
                    case 0:
                        GetRoomUser().Chat("<font color=\"#04B404\"><b>0</b>.</font>", false, 33);
                        break;
                    case 1:
                        GetRoomUser().Chat("<font color=\"#B40404\"><b>1</b>, rojo impar y falta.</font>", false, 33);
                        break;
                    case 2:
                        GetRoomUser().Chat("<font color=\"#1C1C1C\"><b>2</b>, negro par y falta.</font>", false, 33);
                        break;
                    case 3:
                        GetRoomUser().Chat("<font color=\"#B40404\"><b>3</b>, rojo impar y falta.</font>", false, 33);
                        break;
                    case 4:
                        GetRoomUser().Chat("<font color=\"#1C1C1C\"><b>4</b>, negro par y falta.</font>", false, 33);
                        break;
                    case 5:
                        GetRoomUser().Chat("<font color=\"#B40404\"><b>5</b>, rojo impar y falta.</font>", false, 33);
                        break;
                    case 6:
                        GetRoomUser().Chat("<font color=\"#1C1C1C\"><b>6</b>, negro par y falta.</font>", false, 33);
                        break;
                    case 7:
                        GetRoomUser().Chat("<font color=\"#B40404\"><b>7</b>, rojo impar y falta.</font>", false, 33);
                        break;
                    case 8:
                        GetRoomUser().Chat("<font color=\"#1C1C1C\"><b>8</b>, negro par y falta.</font>", false, 33);
                        break;
                    case 9:
                        GetRoomUser().Chat("<font color=\"#B40404\"><b>9</b>, rojo impar y falta.</font>", false, 33);
                        break;
                    case 10:
                        GetRoomUser().Chat("<font color=\"#1C1C1C\"><b>10</b>, negro par y falta.</font>", false, 33);
                        break;
                    case 11:
                        GetRoomUser().Chat("<font color=\"#1C1C1C\"><b>11</b>, negro impar y falta.</font>", false, 33);
                        break;
                    case 12:
                        GetRoomUser().Chat("<font color=\"#B40404\"><b>12</b>, rojo par y falta.</font>", false, 33);
                        break;
                    case 13:
                        GetRoomUser().Chat("<font color=\"#1C1C1C\"><b>13</b>, negro impar y falta.</font>", false, 33);
                        break;
                    case 14:
                        GetRoomUser().Chat("<font color=\"#B40404\"><b>14</b>, rojo par y falta.</font>", false, 33);
                        break;
                    case 15:
                        GetRoomUser().Chat("<font color=\"#1C1C1C\"><b>15</b>, negro impar y falta.</font>", false, 33);
                        break;
                    case 16:
                        GetRoomUser().Chat("<font color=\"#B40404\"><b>16</b>, rojo par y falta.</font>", false, 33);
                        break;
                    case 17:
                        GetRoomUser().Chat("<font color=\"#1C1C1C\"><b>17</b>, negro impar y falta.</font>", false, 33);
                        break;
                    case 18:
                        GetRoomUser().Chat("<font color=\"#B40404\"><b>18</b>, rojo par y falta.</font>", false, 33);
                        break;
                    case 19:
                        GetRoomUser().Chat("<font color=\"#B40404\"><b>19</b>, rojo impar y pasa.</font>", false, 33);
                        break;
                    case 20:
                        GetRoomUser().Chat("<font color=\"#1C1C1C\"><b>20</b>, negro par y pasa.</font>", false, 33);
                        break;
                    case 21:
                        GetRoomUser().Chat("<font color=\"#B40404\"><b>21</b>, rojo impar y pasa.</font>", false, 33);
                        break;
                    case 22:
                        GetRoomUser().Chat("<font color=\"#1C1C1C\"><b>22</b>, negro par y pasa.</font>", false, 33);
                        break;
                    case 23:
                        GetRoomUser().Chat("<font color=\"#B40404\"><b>23</b>, rojo impar y pasa.</font>", false, 33);
                        break;
                    case 24:
                        GetRoomUser().Chat("<font color=\"#1C1C1C\"><b>24</b>, negro par y pasa.</font>", false, 33);
                        break;
                    case 25:
                        GetRoomUser().Chat("<font color=\"#B40404\"><b>25</b>, rojo impar y pasa.</font>", false, 33);
                        break;
                    case 26:
                        GetRoomUser().Chat("<font color=\"#1C1C1C\"><b>26</b>, negro par y pasa.</font>", false, 33);
                        break;
                    case 27:
                        GetRoomUser().Chat("<font color=\"#B40404\"><b>27</b>, rojo impar y pasa.</font>", false, 33);
                        break;
                    case 28:
                        GetRoomUser().Chat("<font color=\"#1C1C1C\"><b>28</b>, negro par y pasa.</font>", false, 33);
                        break;
                    case 29:
                        GetRoomUser().Chat("<font color=\"#1C1C1C\"><b>29</b>, negro impar y pasa.</font>", false, 33);
                        break;
                    case 30:
                        GetRoomUser().Chat("<font color=\"#B40404\"><b>30</b>, rojo par y pasa.</font>", false, 33);
                        break;
                    case 31:
                        GetRoomUser().Chat("<font color=\"#1C1C1C\"><b>31</b>, negro impar y pasa.</font>", false, 33);
                        break;
                    case 32:
                        GetRoomUser().Chat("<font color=\"#B40404\"><b>32</b>, rojo par y pasa.</font>", false, 33);
                        break;
                    case 33:
                        GetRoomUser().Chat("<font color=\"#1C1C1C\"><b>33</b>, negro impar y pasa.</font>", false, 33);
                        break;
                    case 34:
                        GetRoomUser().Chat("<font color=\"#B40404\"><b>34</b>, rojo par y pasa.</font>", false, 33);
                        break;
                    case 35:
                        GetRoomUser().Chat("<font color=\"#1C1C1C\"><b>35</b>, negro impar y pasa.</font>", false, 33);
                        break;
                    case 36:
                        GetRoomUser().Chat("<font color=\"#B40404\"><b>36</b>, rojo par y pasa.</font>", false, 33);
                        break;
                }
                #endregion

                foreach (KeyValuePair<int, CasinoDataLocura> pair in Data1)
                {
                    int userId = pair.Key;
                    CasinoDataLocura Data2 = pair.Value;

                    if (Data2.bet == num)
                    {
                        GameClient client = CloudServer.GetGame().GetClientManager().GetClientByUserID(Data2.userId);
                        if (client is GameClient)
                        {
                            int d = (int)Math.Ceiling(Data2.quantity * offerMultiplier);
                            client.GetHabbo().Diamonds += d;
                            client.SendMessage(new HabboActivityPointNotificationComposer(client.GetHabbo().Diamonds, d, 5));
                            
                        }
                    }
                }

                Data1.Clear();
                offerMultiplier = 1.2;
                Bets = 1;



                // Reset the timer of the round.
                GameLength = 100;
               
            }
                else  GameLength--;
                GetRoomUser().Chat(GameLength + "", false, 33);

        }
    }
}
