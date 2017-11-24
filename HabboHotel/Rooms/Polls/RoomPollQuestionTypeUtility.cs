namespace Cloud.HabboHotel.Rooms.Polls
{
    public static class RoomPollQuestionTypeUtility
    {
        public static RoomPollQuestionType GetQuestionType(string type)
        {
            switch (type.ToLower())
            {
                default:
                case "radio":
                    return RoomPollQuestionType.Radio;
                case "checkbox":
                    return RoomPollQuestionType.Checkbox;
                case "textbox":
                    return RoomPollQuestionType.Textbox;
                case "opinion":
                    return RoomPollQuestionType.Opinion;
            }
        }

        public static int GetQuestionType(RoomPollQuestionType type)
        {
            switch (type)
            {
                default:
                case RoomPollQuestionType.Radio:
                    return 1;
                case RoomPollQuestionType.Checkbox:
                    return 2;
                case RoomPollQuestionType.Textbox:
                    return 3;
            }
        }
    }
}