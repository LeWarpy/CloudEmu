
using Cloud.HabboHotel.Rooms;

namespace Cloud.Communication.Packets.Outgoing.Sound
{
    class SetJukeboxNowPlayingComposer : ServerPacket
    {
        public SetJukeboxNowPlayingComposer(Room room)
            : base(ServerPacketHeader.SetJukeboxNowPlayingMessageComposer)
        {
            var trax = room.GetTraxManager();
            if (trax.IsPlaying && trax.ActualSongData != null)
            {

                var actualmusicitem = trax.ActualSongData;
                var actualmusic = trax.GetMusicByItem(actualmusicitem);
                var musicindex = trax.GetMusicIndex(actualmusicitem);
                var anteriorlength = trax.AnteriorMusic != null ? trax.AnteriorMusic.Length : 0;
				WriteInteger(actualmusic.Id); // songid
				WriteInteger(musicindex);
				WriteInteger(actualmusic.Id); // songid
				WriteInteger((int)((trax.TotalPlayListLength) * 1000.0));
				WriteInteger((int)(trax.ActualSongTimePassed * 1000.0));
            }
            else
            {
				WriteInteger(-1);
				WriteInteger(-1);
				WriteInteger(-1);
				WriteInteger(-1);
				WriteInteger(-1);

            }
        }
    }
}