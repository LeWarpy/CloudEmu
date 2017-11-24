using System;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections.Generic;

using System.Text.RegularExpressions;

using Cloud.Database.Interfaces;


namespace Cloud.HabboHotel.Rooms.Chat.Filter
{
    public sealed class WordFilterManager
    {
        // New filter system by Komok
        // All rights

        private List<string> _filteredWords;
        private List<WordFilterReplacements> _filterReplacements;

        public WordFilterManager()
        {
            this._filteredWords = new List<string>();
            this._filterReplacements = new List<WordFilterReplacements>();
        }

        public void InitWords()
        {
            if (this._filteredWords.Count > 0) this._filteredWords.Clear();
            DataTable Data = null;
            using (var dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT word FROM `wordfilter`");
                Data = dbClient.getTable();
                if (Data != null)
                    foreach (DataRow Row in Data.Rows) this._filteredWords.Add(Convert.ToString(Row["word"]));
            }
        }

        public void InitCharacters()
        {
            if (this._filterReplacements.Count > 0) this._filterReplacements.Clear();
            DataTable Data = null;
            using (var dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `wordfilter_characters`");
                Data = dbClient.getTable();
                if (Data != null)
                    foreach (DataRow Row in Data.Rows) this._filterReplacements.Add(new WordFilterReplacements(Convert.ToString(Row["character"]),
                        Convert.ToString(Row["replacement"])));
            }
        }

        public bool IsUnnaceptableWord(string str, out string output)
        {
            str = str.ToLower();
            foreach (var replacement in this._filterReplacements.Select(word => word).Where(word => str.Contains(word.Character)))
                str = str.Replace(replacement.Character, replacement.Replacement);
            output = _filteredWords.FirstOrDefault(hotelWords => str.Contains(hotelWords.ToLower()));
            return !string.IsNullOrEmpty(output);
        }
    }

    public class WordFilterReplacements
    {
        public string Character;
        public string Replacement;

        public WordFilterReplacements(string character, string replacement)
        {
            Character = character;
            Replacement = replacement;
        }
    }
}