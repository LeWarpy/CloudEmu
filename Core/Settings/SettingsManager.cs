using System.Data;
using System.Collections.Generic;
using log4net;
using Cloud.Database.Interfaces;

namespace Cloud.Core.Settings
{
    public class SettingsManager
    {
        private Dictionary<string, string> _settings = new Dictionary<string, string>();

        private static readonly ILog log = LogManager.GetLogger("Cloud.Core.Settings.SettingsManager");

        public SettingsManager()
        {
            _settings = new Dictionary<string, string>();
        }

        public void Init()
        {
            if (_settings.Count > 0)
                _settings.Clear();

            using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `server_settings`");
                DataTable Table = dbClient.getTable();

                if (Table != null)
                {
                    foreach (DataRow Row in Table.Rows)
                    {
                        _settings.Add(Row["key"].ToString().ToLower(), Row["value"].ToString().ToLower());
                    }
                }
            }

            log.Info("» Cargada(s): " + _settings.Count + " configuraciones del servidor.");
        }

        public string TryGetValue(string value)
        {
            return _settings.ContainsKey(value) ? _settings[value] : "0";
        }
    }
}
