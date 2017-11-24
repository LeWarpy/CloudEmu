using Cloud.Communication.Packets.Outgoing.Sound;
using Cloud.HabboHotel.Items;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Cloud.HabboHotel.Rooms.TraxMachine
{
    public class RoomTraxManager
    {
        public Room Room { get; private set; }
        public List<Item> Playlist { get; private set; }
        public int Capacity = 10;
        public bool IsPlaying { get; private set; }
        public int StartedPlayTimestamp { get; private set; }
        public Item SelectedDiscItem { get; private set; }

        public TraxMusicData AnteriorMusic { get; private set; }
        public Item AnteriorItem { get; private set; }



        DataTable dataTable;
        public RoomTraxManager(Room room)
        {
            Room = room;
            room.OnFurnisLoad += Room_OnFurnisLoad;

            IsPlaying = false;

            StartedPlayTimestamp = 0;

            Playlist = new List<Item>();

            SelectedDiscItem = null;
            using (var adap = CloudServer.GetDatabaseManager().GetQueryReactor())
            {
                adap.RunQuery("SELECT * FROM room_jukebox_songs WHERE room_id = '" + Room.Id + "'");
                dataTable = adap.getTable();
            }
        }

        private void Room_OnFurnisLoad()
        {
            foreach (DataRow row in dataTable.Rows)
            {
                var itemid = int.Parse(row["item_id"].ToString());
                var item = Room.GetRoomItemHandler().GetItem(itemid);
                if (item == null)
                    continue;

                Playlist.Add(item);
            }
        }


        public void OnCycle()
        {
            if (IsPlaying)
            {
                if (ActualSongData != SelectedDiscItem)
                {
                    AnteriorItem = SelectedDiscItem;
                    AnteriorMusic = GetMusicByItem(SelectedDiscItem);
                    SelectedDiscItem = ActualSongData;
                    if (SelectedDiscItem == null)
                        StopPlayList();
                    Room.SendMessage(new SetJukeboxNowPlayingComposer(Room));
                }
            }
        }

        public void ClearPlayList()
        {
            if (IsPlaying)
                StopPlayList();

            Playlist.Clear();
        }

        public int TimestampSinceStarted
        {
            get
            {
                return (int)CloudServer.GetUnixTimestamp() - StartedPlayTimestamp;
            }
        }

        public int TotalPlayListLength
        {
            get
            {
                int e = 0;
                foreach (var item in Playlist)
                {
                    var music = TraxSoundManager.GetMusic(item.ExtradataInt);
                    if (music == null)
                        continue;

                    e += music.Length;
                }
                return e;
            }
        }

        public Item ActualSongData
        {
            get
            {
                var line = GetPlayLine().Reverse();
                var now = TimestampSinceStarted;
                if (now > TotalPlayListLength)
                    return null;
                foreach (var item in line)
                {
                    if (item.Key <= now)
                        return item.Value;
                }

                //IsPlaying = false;
                return null;
            }
        }

        public int ActualSongTimePassed
        {
            get
            {
                var line = GetPlayLine();
                int indextime = 0;
                foreach (var music in line)
                {
                    if (music.Value == ActualSongData)
                        indextime = music.Key;
                }
                return TimestampSinceStarted - indextime;
            }
        }

        public Dictionary<int, Item> GetPlayLine()
        {
            var i = 0;
            var e = new Dictionary<int, Item>();
            foreach (var item in Playlist)
            {
                var music = GetMusicByItem(item);
                if (music == null)
                    continue;
                e.Add(i, item);
                i += music.Length;
            }
            return e;
        }

        public TraxMusicData GetMusicByItem(Item item)
        {
            return item != null ? TraxSoundManager.GetMusic(item.ExtradataInt) : null;
        }

        public int GetMusicIndex(Item item)
        {
            for (var i = 0; i < Playlist.Count; i++)
            {
                if (Playlist[i] == item)
                    return i;
            }

            return 0;
        }

        public void PlayPlaylist()
        {
            if (Playlist.Count == 0)
                return;
            StartedPlayTimestamp = (int)CloudServer.GetUnixTimestamp();
            SelectedDiscItem = null;
            IsPlaying = true;
            //Room.SendMessage(new SetJukeboxNowPlayingComposer(Room));
            SetJukeboxesState();
        }

        public void StopPlayList()
        {
            IsPlaying = false;
            StartedPlayTimestamp = 0;
            SelectedDiscItem = null;
            Room.SendMessage(new SetJukeboxNowPlayingComposer(Room));
            SetJukeboxesState();
        }

        public void TriggerPlaylistState()
        {
            if (IsPlaying)
                StopPlayList();
            else
                PlayPlaylist();
        }

        public void SetJukeboxesState()
        {
            foreach (var item in Room.GetRoomItemHandler().GetFloor)
                if (item.GetBaseItem().InteractionType == InteractionType.JUKEBOX)
                {
                    item.ExtraData = IsPlaying ? "1" : "0";
                    item.UpdateState();
                }
        }

        public bool AddDisc(Item item)
        {
            if (item.GetBaseItem().InteractionType != InteractionType.MUSIC_DISC)
                return false;

            int musicId;
            if (!int.TryParse(item.ExtraData, out musicId))
                return false;

            var music = TraxSoundManager.GetMusic(musicId);

            if (music == null)
                return false;

            if (Playlist.Contains(item))
                return false;

            if (IsPlaying)
                return false;

            using (var adap = CloudServer.GetDatabaseManager().GetQueryReactor())
            {
                adap.SetQuery("INSERT INTO room_jukebox_songs (room_id, item_id) VALUES (@room, @item)");
                adap.AddParameter("room", Room.Id);
                adap.AddParameter("item", item.Id);
                adap.RunQuery();
            }

            Playlist.Add(item);
            Room.SendMessage(new SetJukeboxPlayListComposer(Room));
            Room.SendMessage(new LoadJukeboxUserMusicItemsComposer(Room));
            return true;
        }

        public bool RemoveDisc(int id)
        {
            var item = GetDiscItem(id);
            if (item == null)
                return false;

            if (IsPlaying)
                return false;

            using (var adap = CloudServer.GetDatabaseManager().GetQueryReactor())
            {
                adap.runFastQuery("DELETE FROM room_jukebox_songs WHERE item_id = '" + item.Id + "'");
            }
            Playlist.Remove(item);

            Room.SendMessage(new SetJukeboxPlayListComposer(Room));
            Room.SendMessage(new LoadJukeboxUserMusicItemsComposer(Room));

            return true;
        }

        public bool RemoveDisc(Item item)
        {
            return RemoveDisc(item.Id);
        }

        public List<Item> GetAvaliableSongs()
        {
            return Room.GetRoomItemHandler().GetFloor.Where(c => c.GetBaseItem().InteractionType == InteractionType.MUSIC_DISC && !Playlist.Contains(c)).ToList();
        }

        public Item GetDiscItem(int id)
        {
            foreach (var item in Playlist)
                if (item.Id == id)
                    return item;

            return null;
        }


    }
}
