using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using Cloud.Database.Interfaces;

namespace Cloud.HabboHotel.Catalog.Pets
{
    public class PetRaceManager
    {
        private List<PetRace> _races = new List<PetRace>();

        public void Init()
        {
            if (_races.Count > 0)
                _races.Clear();

            using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `catalog_pet_races`");
                DataTable Table = dbClient.getTable();

                if (Table != null)
                {
                    foreach (DataRow Row in Table.Rows)
                    {
                        PetRace Race = new PetRace(Convert.ToInt32(Row["raceid"]), Convert.ToInt32(Row["color1"]), Convert.ToInt32(Row["color2"]), (Convert.ToString(Row["has1color"]) == "1"), (Convert.ToString(Row["has2color"]) == "1"));
                        if (!_races.Contains(Race))
                            _races.Add(Race);
                    }
                }
            }
        }

        public List<PetRace> GetRacesForRaceId(int RaceId)
        {
            return _races.Where(Race => Race.RaceId == RaceId).ToList();
        }
    }
}