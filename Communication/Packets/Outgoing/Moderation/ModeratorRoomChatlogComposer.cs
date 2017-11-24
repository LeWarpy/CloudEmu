using Cloud.Utilities;
using Cloud.HabboHotel.Rooms;
using System.Collections.Generic;
using Cloud.HabboHotel.Rooms.Chat.Logs;

namespace Cloud.Communication.Packets.Outgoing.Moderation
{
    class ModeratorRoomChatlogComposer : ServerPacket
    {
        public ModeratorRoomChatlogComposer(Room Room, ICollection<ChatlogEntry> chats)
            : base(ServerPacketHeader.ModeratorRoomChatlogMessageComposer)
        {
			WriteByte(1);
			WriteShort(2);//Count
			WriteString("roomName");
			WriteByte(2);
			WriteString(Room.Name);
			WriteString("roomId");
			WriteByte(1);
			WriteInteger(Room.Id);

			WriteShort(chats.Count);
            foreach (ChatlogEntry Entry in chats)
            {
                string Username = "Unknown";
                if (Entry.PlayerNullable() != null)
                {
                    Username = Entry.PlayerNullable().Username;
                }

				WriteString(UnixTimestamp.FromUnixTimestamp(Entry.Timestamp).ToShortTimeString()); // time?
				WriteInteger(Entry.PlayerId); // User Id
				WriteString(Username); // Username
				WriteString(!string.IsNullOrEmpty(Entry.Message) ? Entry.Message : "** O usuário enviou uma mensagem em branco **"); // Message        
				WriteBoolean(false); //TODO, AI's?
            }
        }
    }
}
