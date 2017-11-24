using log4net;
using Cloud.Database.Interfaces;
using System.Collections.Generic;
using System.Data;

namespace Cloud.HabboHotel.Items.Crafting
{
    public class CraftingManager
    {
        private static readonly ILog log = LogManager.GetLogger("Cloud.HabboHotel.Items.Crafting.CraftingManager");
        internal Dictionary<string, CraftingRecipe> CraftingRecipes;
        internal List<string> CraftableItems;

        public CraftingManager()
        {
            CraftingRecipes = new Dictionary<string, CraftingRecipe>();
            CraftableItems = new List<string>();
        }

        internal void Init()
        {
            using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
            {
                CraftingRecipes.Clear();
                dbClient.SetQuery("SELECT * FROM crafting_recipes");
                var recipes = dbClient.getTable();
                foreach (DataRow recipe in recipes.Rows)
                {
                    CraftingRecipe value = new CraftingRecipe((string)recipe["id"], (string)recipe["items"], (string)recipe["result"], (int)recipe["type"]);
                    CraftingRecipes.Add((string)recipe["id"], value);
                }

                CraftableItems.Clear();
                dbClient.SetQuery("SELECT * FROM crafting_items");
                var items = dbClient.getTable();
                foreach (DataRow item in items.Rows)
                {
                    CraftableItems.Add((string)item["itemName"]);
                }
            }

            log.Info("» Crafring Manager -> CARGADO");
        }

        internal CraftingRecipe GetRecipe(string name)
        {
            if (CraftingRecipes.ContainsKey(name))
                return CraftingRecipes[name];
            else
                return null;
        }

        internal CraftingRecipe GetRecipeByPrize(string name)
        {
            foreach(CraftingRecipe c in CraftingRecipes.Values)
            {
                if (c.Result == name)
                    return c;
            }
            return null;
        }
    }
}