
using Cloud.HabboHotel.Rooms;

namespace Cloud.Communication.Packets.Outgoing.Sound
{
    class SetJukeboxPlayListComposer : ServerPacket
    {
        public SetJukeboxPlayListComposer(Room room)
            : base(ServerPacketHeader.SetJukeboxPlayListMessageComposer)
        {
            var items = room.GetTraxManager().Playlist;
			WriteInteger(items.Count); //Capacity
			WriteInteger(items.Count); //While items Songs Count

            foreach (var item in items)
            {
                int musicid;
                int.TryParse(item.ExtraData, out musicid);
				WriteInteger(item.Id);
				WriteInteger(musicid);//EndWhile
            }
        }
    }
}