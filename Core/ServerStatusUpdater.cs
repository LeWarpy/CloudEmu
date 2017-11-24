using System;
using System.Threading;
using System.Diagnostics;
using log4net;
using Cloud.Database.Interfaces;
using Cloud.HabboHotel;

namespace Cloud.Core
{
    public class ServerStatusUpdater : IDisposable
    {
        private static ILog log = LogManager.GetLogger("Cloud.Core.ServerStatusUpdater");
        private const int UPDATE_IN_SECS = 15;
        public static int _userPeak;

        private static string _lastDate;

        private static bool isExecuted;

        private static Stopwatch lowPriorityProcessWatch;
        private static Timer _mTimer;

        public static void Init()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            log.Info("» Actualizador del servidor iniciado.");
            Console.ResetColor();
            using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.runFastQuery("UPDATE server_status SET status = '1'");
            }
            lowPriorityProcessWatch = new Stopwatch();
            lowPriorityProcessWatch.Start();
        }

        public static void StartProcessing()
        {
            _mTimer = new Timer(Process, null, 0, 10000);
        }

        internal static void Process(object caller)
        {
            if (lowPriorityProcessWatch.ElapsedMilliseconds >= 10000 || !isExecuted)
            {
                isExecuted = true;
                lowPriorityProcessWatch.Restart();

                var clientCount = CloudServer.GetGame().GetClientManager().Count;
                var loadedRoomsCount = CloudServer.GetGame().GetRoomManager().Count;
                var Uptime = DateTime.Now - CloudServer.ServerStarted;
                Game.SessionUserRecord = clientCount > Game.SessionUserRecord ? clientCount : Game.SessionUserRecord;
                Console.Title = string.Concat("Cloud Server [" + CloudServer.HotelName + "] » [" + clientCount + "] ON » [" + loadedRoomsCount + "] SALAS » [" + Uptime.Days + "] DÍAS » [" + Uptime.Hours + "] HORAS");

                using (var queryReactor = CloudServer.GetDatabaseManager().GetQueryReactor())
                {
                    if (clientCount > _userPeak) _userPeak = clientCount;

                    _lastDate = DateTime.Now.ToShortDateString();
                    queryReactor.runFastQuery(string.Concat("UPDATE `server_status` SET `status` = '2', `users_online` = '", clientCount, "', `loaded_rooms` = '", loadedRoomsCount, "'"));
                }
            }
        }

        public void Dispose()
        {
            using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.runFastQuery("UPDATE `server_status` SET `users_online` = '0', `loaded_rooms` = '0', `status` = '0'");
            }

            _mTimer.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}