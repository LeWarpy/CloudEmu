using System.Collections.Generic;

namespace Cloud.Communication.Packets.Outgoing.Users
{
	class NameChangeUpdateComposer : ServerPacket
    {
        public NameChangeUpdateComposer(string Name, int Error, ICollection<string> Tags)
            : base(ServerPacketHeader.NameChangeUpdateMessageComposer)
        {
            WriteInteger(Error);
            WriteString(Name);

            WriteInteger(Tags.Count);
            foreach (string Tag in Tags)
            {
               WriteString(Name + Tag);
            }
        }

        public NameChangeUpdateComposer(string Name, int Error)
            : base(ServerPacketHeader.NameChangeUpdateMessageComposer)
        {
            WriteInteger(Error);
            WriteString(Name);
            WriteInteger(0);
        }
    }
}
