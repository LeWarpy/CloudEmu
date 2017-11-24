namespace Cloud.Communication.Packets.Outgoing.LandingView
{
	class ConcurrentUsersGoalProgressComposer : ServerPacket
    {
        public ConcurrentUsersGoalProgressComposer(int UsersNow, int type, int goal)
            : base(ServerPacketHeader.ConcurrentUsersGoalProgressMessageComposer)
        {
			WriteInteger(type);
			WriteInteger(UsersNow);
			WriteInteger(goal);
        }
    }
}
