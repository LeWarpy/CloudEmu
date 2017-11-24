using System.Collections.Generic;

namespace Cloud.Communication.Packets.Outgoing.Users
{
    public class IgnoredUsersComposer : ServerPacket
    {
        public IgnoredUsersComposer(List<string> ignoredUsers)
            : base(ServerPacketHeader.IgnoredUsersMessageComposer)
        {
            WriteInteger(ignoredUsers.Count);
            foreach (string Username in ignoredUsers)
            {
                WriteString(Username);
            }
        }
    }
}
