using Cloud.HabboHotel.Helpers;

namespace Cloud.Communication.Packets.Outgoing.Help.Helpers
{
	class CallForHelperWindowComposer : ServerPacket
    {
        public CallForHelperWindowComposer(bool IsHelper, int Category, string Message, int WaitTime)
            : base(ServerPacketHeader.CallForHelperWindowMessageComposer)
        {
			WriteBoolean(IsHelper);
			WriteInteger(Category);
			WriteString(Message);
			WriteInteger(WaitTime);
        }
        public CallForHelperWindowComposer(bool IsHelper, HelperCase Case)
            : base(ServerPacketHeader.CallForHelperWindowMessageComposer)
        {
			WriteBoolean(IsHelper);
			WriteInteger((int)Case.Type);
			WriteString(Case.Message);
			WriteInteger(Case.ReamingToExpire);
        }

    }
}
