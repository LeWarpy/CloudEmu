namespace Cloud.Communication.Packets.Outgoing.Messenger
{
	class NewConsoleMessageComposer : ServerPacket
    {
        public NewConsoleMessageComposer(int Sender, string Message, int Time = 0)
            : base(ServerPacketHeader.NewConsoleMessageMessageComposer)
        {
			WriteInteger(Sender);
			WriteString(Message);
			WriteInteger(Time);
        }
    }

    class FuckingConsoleMessageComposer : ServerPacket
    {
        public FuckingConsoleMessageComposer(int Sender, string Message, string Data)
            : base(ServerPacketHeader.NewConsoleMessageMessageComposer)
        {
			WriteInteger(Sender);
			WriteString(Message);
			WriteInteger(0);
			WriteString(Data);
        }
    }
}
