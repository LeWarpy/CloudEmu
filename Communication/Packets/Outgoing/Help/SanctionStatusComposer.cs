using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloud.Communication.Packets.Outgoing.Help
{
	class SanctionStatusComposer : ServerPacket
	{
		public SanctionStatusComposer()
			: base(ServerPacketHeader.SanctionStatusMessageComposer)
		{
			WriteBoolean(true);
			WriteBoolean(false);
			WriteString("ALERT");
			WriteInteger(0);
			WriteInteger(30);
			WriteString("cfh.reason.EMPTY");
			WriteString("2017-11-23 19:41 (GMT +0000)");
			WriteInteger(720);
			WriteString("ALERT");
			WriteInteger(0);
			WriteInteger(30);
			WriteString("");
			WriteBoolean(false);
		}
	}
}