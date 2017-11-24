using System.Data;
using System.Collections.Generic;
using log4net;
using Cloud.Database.Interfaces;

namespace Cloud.Core.Language
{
    public class LanguageManager
    {
        private Dictionary<string, string> _values = new Dictionary<string, string>();

        private static readonly ILog log = LogManager.GetLogger("Cloud.Core.Language.LanguageManager");

        public LanguageManager()
        {
            this._values = new Dictionary<string, string>();
        }

        public void Init()
        {
            if (this._values.Count > 0)
                this._values.Clear();

            using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `server_locale`");
                DataTable Table = dbClient.getTable();

                if (Table != null)
                {
                    foreach (DataRow Row in Table.Rows)
                    {
                        this._values.Add(Row["key"].ToString(), Row["value"].ToString());
                    }
                }
            }

            log.Info("» Cargado(s): " + this._values.Count + " idiomas locales.");
        }

        public string TryGetValue(string value)
        {
            return this._values.ContainsKey(value) ? this._values[value] : "No language locale found for [" + value + "]";
        }
    }
}
