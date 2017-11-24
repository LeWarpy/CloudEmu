using System;
using System.Linq;
using System.Data;
using System.Collections.Generic;
using Cloud.Database.Interfaces;
using log4net;

namespace Cloud.HabboHotel.Games
{
    public class GameDataManager
    {
        private static readonly ILog log = LogManager.GetLogger("Cloud.HabboHotel.Games.GameDataManager");

        private readonly Dictionary<int, GameData> _games;

        public GameDataManager()
        {
            _games = new Dictionary<int, GameData>();

            Init();
        }

        public void Init()
        {
            if (_games.Count > 0)
                _games.Clear();

            using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
            {
                DataTable GetData = null;
                dbClient.SetQuery("SELECT `id`,`name`,`colour_one`,`colour_two`,`resource_path`,`string_three`,`game_swf`,`game_assets`,`game_server_host`,`game_server_port`,`socket_policy_port`,`game_enabled` FROM `games_config`");
                GetData = dbClient.getTable();

                if (GetData != null)
                {
                    foreach (DataRow Row in GetData.Rows)
                    {
                        _games.Add(Convert.ToInt32(Row["id"]), new GameData(Convert.ToInt32(Row["id"]), Convert.ToString(Row["name"]), Convert.ToString(Row["colour_one"]), Convert.ToString(Row["colour_two"]), Convert.ToString(Row["resource_path"]), Convert.ToString(Row["string_three"]), Convert.ToString(Row["game_swf"]), Convert.ToString(Row["game_assets"]), Convert.ToString(Row["game_server_host"]), Convert.ToString(Row["game_server_port"]), Convert.ToString(Row["socket_policy_port"]), CloudServer.EnumToBool(Row["game_enabled"].ToString())));
                    }
                }
            }

            log.Info("» Juegos -> CARGADO");
        }

        public bool TryGetGame(int GameId, out GameData GameData)
        {
            if (_games.TryGetValue(GameId, out GameData))
                return true;
            return false;
        }

        public int GetCount()
        {
            int GameCount = 0;
            foreach (GameData Game in _games.Values.ToList())
            {
                if (Game.GameEnabled)
                    GameCount += 1;
            }
            return GameCount;
        }

        public ICollection<GameData> GameData
        {
            get
            {
                return _games.Values;
            }
        }
    }
}