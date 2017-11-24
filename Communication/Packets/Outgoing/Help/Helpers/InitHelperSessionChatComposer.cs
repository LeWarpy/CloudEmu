using Cloud.HabboHotel.Users;

namespace Cloud.Communication.Packets.Outgoing.Help.Helpers
{
	class InitHelperSessionChatComposer : ServerPacket
    {

        public InitHelperSessionChatComposer(Habbo Habbo1, Habbo Habbo2)
            : base(ServerPacketHeader.InitHelperSessionChatMessageComposer)
        {
			WriteInteger(Habbo1.Id);
			WriteString(Habbo1.Username);
			WriteString(Habbo1.Look);

			WriteInteger(Habbo2.Id);
			WriteString(Habbo2.Username);
			WriteString(Habbo2.Look);




        }
    }
}
