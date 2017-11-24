using System.Collections.Generic;

namespace Cloud.Communication.Packets.Outgoing.Navigator
{
	class PopularRoomTagsResultComposer : ServerPacket
    {
        public PopularRoomTagsResultComposer(ICollection<KeyValuePair<string, int>> Tags)
            : base(ServerPacketHeader.PopularRoomTagsResultMessageComposer)
        {
			WriteInteger(Tags.Count);
            foreach (KeyValuePair<string, int> tag in Tags)
            {
				WriteString(tag.Key);
				WriteInteger(tag.Value);
            }
        }
    }
}
