namespace Cloud.HabboHotel.Rooms.Polls
{
    public static class RoomPollTypeUtility
    {
        public static RoomPollType GetRoomPollType(string type)
        {
            switch (type)
            {
                default:
                case "poll":
                    return RoomPollType.Poll;
                case "question":
                    return RoomPollType.Question;
            }
        }

        public static string GetRoomPollType(RoomPollType type)
        {
            switch (type)
            {
                default:
                case RoomPollType.Poll:
                    return "poll";
                case RoomPollType.Question:
                    return "question";
            }
        }
    }
}