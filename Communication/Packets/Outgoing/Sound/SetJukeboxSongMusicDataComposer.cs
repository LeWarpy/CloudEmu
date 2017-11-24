
using System.Collections.Generic;
using Cloud.HabboHotel.Rooms.TraxMachine;

namespace Cloud.Communication.Packets.Outgoing.Sound
{
    class SetJukeboxSongMusicDataComposer : ServerPacket
    {
        public SetJukeboxSongMusicDataComposer(ICollection<TraxMusicData> Songs)
            : base(ServerPacketHeader.SetJukeboxSongMusicDataMessageComposer)
        {
			WriteInteger(Songs.Count);//while

            foreach (var item in Songs)
            {
				WriteInteger(item.Id);// Song id
				WriteString(item.CodeName); // Song code name
				WriteString(item.Name);
				WriteString(item.Data);
				WriteInteger((int)(item.Length * 1000.0)); // Music Length - Duration
				WriteString(item.Artist);
            }
        }
    }
}