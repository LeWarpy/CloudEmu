using System;

namespace Cloud.Communication.Packets.Outgoing.Rooms.Poll
{
	class QuickPollMessageComposer : ServerPacket
    {
        public QuickPollMessageComposer(String question)
            : base(ServerPacketHeader.QuickPollMessageComposer)
        {
			WriteString("");
			WriteInteger(0);
			WriteInteger(0);
			WriteInteger(1);   //duration
			WriteInteger(-1);  //id
			WriteInteger(120); //number
			WriteInteger(3);
			WriteString(question);
        }
    }
}