﻿using System;
using System.Linq;
using System.Data;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Cloud.HabboHotel.Users.Clothing.Parts;
using Cloud.Database.Interfaces;

namespace Cloud.HabboHotel.Users.Clothing
{
     public sealed class ClothingComponent
    {
        private Habbo _habbo;
        private readonly ConcurrentDictionary<int, ClothingParts> _allClothing = new ConcurrentDictionary<int, ClothingParts>();

        public ClothingComponent()
        {
        }

        public bool Init(Habbo Habbo)
        {
            if (_allClothing.Count > 0)
                return false;

            DataTable GetClothing = null;
            using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `id`,`part_id`,`part` FROM `user_clothing` WHERE `user_id` = @id;");
                dbClient.AddParameter("id", Habbo.Id);
                GetClothing = dbClient.getTable();

                if (GetClothing != null)
                {
                    foreach (DataRow Row in GetClothing.Rows)
                    {
                        if (this._allClothing.TryAdd(Convert.ToInt32(Row["part_id"]), new ClothingParts(Convert.ToInt32(Row["id"]), Convert.ToInt32(Row["part_id"]), Convert.ToString(Row["part"]))))
                        {
                            //umm?
                        }
                    }
                }
            }

            this._habbo = Habbo;
            return true;
        }

        public void AddClothing(string ClothingName, List<int> PartIds)
        {
            foreach (int PartId in PartIds.ToList())
            {
                if (!this._allClothing.ContainsKey(PartId))
                {
                    int NewId = 0;
                    using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.SetQuery("INSERT INTO `user_clothing` (`user_id`,`part_id`,`part`) VALUES (@UserId, @PartId, @Part)");
                        dbClient.AddParameter("UserId", this._habbo.Id);
                        dbClient.AddParameter("PartId", PartId);
                        dbClient.AddParameter("Part", ClothingName);
                        NewId = Convert.ToInt32(dbClient.InsertQuery());
                    }

                    this._allClothing.TryAdd(PartId, new ClothingParts(NewId, PartId, ClothingName));
                }
            }
        }

        public bool TryGet(int PartId, out ClothingParts ClothingPart)
        {
            return this._allClothing.TryGetValue(PartId, out ClothingPart);
        }

        public ICollection<ClothingParts> GetClothingParts
        {
            get { return this._allClothing.Values; }
        }

        /// <summary>
        /// Disposes the ClothingComponent.
        /// </summary>
        public void Dispose()
        {
            this._allClothing.Clear();
        }
    }
}
