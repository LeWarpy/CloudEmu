using Cloud.HabboHotel.Helpers;

namespace Cloud.Communication.Packets.Outgoing.Help.Helpers
{
	class HandleHelperToolComposer : ServerPacket
    {
        public HandleHelperToolComposer(bool onDuty, int helperAmount, int guideAmount, int guardianAmount)
            : base(ServerPacketHeader.HandleHelperToolMessageComposer)
        {
			WriteBoolean(onDuty);
			WriteInteger(guideAmount);
			WriteInteger(helperAmount);
			WriteInteger(guardianAmount);
        }

        public HandleHelperToolComposer(bool onDuty)
            : base(ServerPacketHeader.HandleHelperToolMessageComposer)
        {
			WriteBoolean(onDuty);
			WriteInteger(HelperToolsManager.GuideCount);
			WriteInteger(HelperToolsManager.HelperCount);
			WriteInteger(HelperToolsManager.GuardianCount);

        }

    }
}
