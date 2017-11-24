using Cloud.HabboHotel.GameClients;
using Cloud.HabboHotel.Groups.Forums;
using System;

namespace Cloud.Communication.Packets.Outgoing.Groups
{
	class ThreadsListDataComposer : ServerPacket
    {
        public ThreadsListDataComposer(GroupForum Forum, GameClient Session, int StartIndex = 0, int MaxLength = 20)
            : base(ServerPacketHeader.ThreadsListDataMessageComposer)
        {
			WriteInteger(Forum.GroupId);//Group Forum ID
			WriteInteger(StartIndex); //Page Index

            var Threads = Forum.Threads;
            if (Threads.Count - 1 >= StartIndex)
                Threads = Threads.GetRange(StartIndex, Math.Min(MaxLength, Threads.Count - StartIndex));

			WriteInteger(Threads.Count); //Thread Count

            foreach (var Thread in Threads)
            {
                Thread.SerializeData(Session, this);
            }
        }
    }
}

