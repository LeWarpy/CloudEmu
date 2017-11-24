using Cloud.HabboHotel.Cache.Type;

namespace Cloud.Communication.Packets.Outgoing.Messenger
{
	class NewBuddyRequestComposer : ServerPacket
    {
        public NewBuddyRequestComposer(UserCache Habbo)
            : base(ServerPacketHeader.NewBuddyRequestMessageComposer)
        {
			WriteInteger(Habbo.Id);
			WriteString(Habbo.Username);
			WriteString(Habbo.Look);
        }
    }
}
