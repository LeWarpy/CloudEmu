using System.Collections.Generic;

namespace Cloud.HabboHotel.Rooms.Chat.Moderation
{
    class ModerationRoomChatLog
    {
        public int UserId { get; set; }
        public List<string> Chat { get; set; }

        public ModerationRoomChatLog(int UserId, List<string> Chat)
        {
            this.UserId = UserId;
            this.Chat = Chat;
        }
    }
}
