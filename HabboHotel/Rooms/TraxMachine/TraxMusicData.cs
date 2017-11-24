
using System.Data;


namespace Cloud.HabboHotel.Rooms.TraxMachine
{
    public class TraxMusicData
    {
        public int Id;
        public string CodeName;
        public string Name;
        public string Artist;
        public string Data;
        public int Length;

        public TraxMusicData(int id, string code, string name, string art, string data, int len)
        {
            Id = id;
            CodeName = code;
            Name = name;
            Artist = art;
            Data = data;
            Length = len;
        }

        public static TraxMusicData Parse(DataRow row)
        {
            return new TraxMusicData(int.Parse(row["id"].ToString()), row["codename"].ToString(), row["name"].ToString(), row["artist"].ToString(), row["song_data"].ToString(), int.Parse(row["length"].ToString()));
        }
    }
}