using System;
using System.Data;
using System.Collections.Generic;
using Cloud.Database.Interfaces;

namespace Cloud.HabboHotel.Catalog.Clothing
{
    public class ClothingManager
    {
        private Dictionary<int, ClothingItem> _clothing;

        public ClothingManager()
        {
            _clothing = new Dictionary<int, ClothingItem>();
           
            Init();
        }

        public void Init()
        {
            if (_clothing.Count > 0)
                _clothing.Clear();

            DataTable GetClothing = null;
            using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `id`,`clothing_name`,`clothing_parts` FROM `catalog_clothing`");
                GetClothing = dbClient.getTable();
            }

            if (GetClothing != null)
            {
                foreach (DataRow Row in GetClothing.Rows)
                {
                    _clothing.Add(Convert.ToInt32(Row["id"]), new ClothingItem(Convert.ToInt32(Row["id"]), Convert.ToString(Row["clothing_name"]), Convert.ToString(Row["clothing_parts"])));
                }
            }
        }

        public bool TryGetClothing(int ItemId, out ClothingItem Clothing)
        {
            if (_clothing.TryGetValue(ItemId, out Clothing))
                return true;
            return false;
        }

        public ICollection<ClothingItem> GetClothingAllParts
        {
            get { return _clothing.Values; }
        }
    }
}
