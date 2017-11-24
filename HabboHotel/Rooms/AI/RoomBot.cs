using System.Collections.Generic;
using Cloud.HabboHotel.Rooms.AI.Speech;
using Cloud.HabboHotel.Rooms.AI.Types;
using Cloud.HabboHotel.Items.Utilities;
using System.Drawing;

namespace Cloud.HabboHotel.Rooms.AI
{
    public class RoomBot
    {
        public int Id;
        public int BotId;
        public int VirtualId;

        public BotAIType AiType;

        public int DanceId;
        public string Gender;

        public string Look;
        public string Motto;
        public string Name;
        public int RoomId;
        public int Rot;
        internal bool WasPicked;

        public string WalkingMode;

        public int X;
        public int Y;
        public double Z;
        public int maxX;
        public int maxY;
        public int minX;
        public int minY;

        public int ownerID;

        public bool AutomaticChat;
        public int SpeakingInterval;
        public bool MixSentences;

        public RoomUser RoomUser;
        public List<RandomSpeech> RandomSpeech;

        private int _chatBubble;
        public bool ForcedMovement { get; set; }
        public int ForcedUserTargetMovement { get; set; }
        public Point TargetCoordinate { get; set; }

        public int TargetUser { get; set; }

        public RoomBot(int BotId, int RoomId, string AiType, string WalkingMode, string Name, string Motto, string Look, int X, int Y, double Z, int Rot,
            int minX, int minY, int maxX, int maxY, ref List<RandomSpeech> Speeches, string Gender, int Dance, int ownerID,
            bool AutomaticChat, int SpeakingInterval, bool MixSentences, int ChatBubble)
        {
            this.Id = BotId;
            this.BotId = BotId;
            this.RoomId = RoomId;

            this.Name = Name;
            this.Motto = Motto;
            this.Look = Look;
            this.Gender = Gender.ToUpper();

            this.AiType = BotUtility.GetAIFromString(AiType);
            this.WalkingMode = WalkingMode;

            this.X = X;
            this.Y = Y;
            this.Z = Z;
            this.Rot = Rot;
            this.minX = minX;
            this.minY = minY;
            this.maxX = maxX;
            this.maxY = maxY;

            this.VirtualId = -1;
            this.RoomUser = null;
            this.DanceId = Dance;

            this.LoadRandomSpeech(Speeches);
            //this.LoadResponses(Responses);

            this.ownerID = ownerID;

            this.AutomaticChat = AutomaticChat;
            this.SpeakingInterval = SpeakingInterval;
            this.MixSentences = MixSentences;

            this._chatBubble = ChatBubble;
            this.ForcedMovement = false;
            this.TargetCoordinate = new Point();
            this.TargetUser = 0;
            WasPicked = RoomId == 0;
        }

        public bool IsPet
        {
            get { return (AiType == BotAIType.PET); }
        }

        #region Speech Related
        public void LoadRandomSpeech(List<RandomSpeech> Speeches)
        {
            this.RandomSpeech = Speeches;
        }


        public RandomSpeech GetRandomSpeech()
        {
            if (RandomSpeech.Count < 1)
            {
                return new RandomSpeech("", 0);
            }
            return RandomSpeech[CloudServer.GetRandomNumber(0, (RandomSpeech.Count - 1))];
        }
        #endregion

        #region AI Related
        public BotAI GenerateBotAI(int VirtualId)
        {
            switch (AiType)
            {
                case BotAIType.PET:
                    return new PetBot(VirtualId);
                case BotAIType.GENERIC:
                    return new GenericBot(VirtualId);
                case BotAIType.BARTENDER:
                    return new BartenderBot(VirtualId);
                case BotAIType.VISITOR_LOGGER:
                    return new VisitorLogger(VirtualId);
                case BotAIType.SAY_BOT:
                    return new SayBot(VirtualId);
                case BotAIType.CASINO_BOT:
                    return new CasinoCounter(VirtualId);
                case BotAIType.ROULLETE_BOT:
                    return new CasinoRoullete(VirtualId);
                default:
                    return new GenericBot(VirtualId);
            }
        }
        #endregion

        public int ChatBubble
        {
            get { return this._chatBubble; }
            set { this._chatBubble = value; }
        }
    }
}