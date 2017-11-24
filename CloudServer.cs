using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using MySql.Data.MySqlClient;
using Cloud.Core;
using Cloud.HabboHotel;
using Cloud.HabboHotel.GameClients;
using Cloud.HabboHotel.Users;
using Cloud.HabboHotel.Users.UserData;
using Cloud.Communication.RCON;
using Cloud.Communication.ConnectionManager;
using Cloud.Utilities;
using log4net;
using System.Collections.Concurrent;
using Cloud.Communication.Packets.Outgoing.Moderation;
using Cloud.Communication.Encryption.Keys;
using Cloud.Communication.Encryption;
using Cloud.Database.Interfaces;
using Cloud.HabboHotel.Cache.Type;
using Cloud.Database;

namespace Cloud
{
    public static class CloudServer
    {
        private static readonly ILog log = LogManager.GetLogger("Cloud.CloudServer");
        public static string HotelName;
        public static string Licenseto;
        public static bool IsLive;
		public static string CurrentTime = DateTime.Now.ToString("hh:mm:ss tt" + "- [DUAL] ");
		public const string PrettyVersion = "Dual Server ";
        public const string PrettyBuild = " 4.0.8 ";
        public const string ServerVersion = " 4.0.8 ";
        public const string VersionCloud = "OREO";
        public const string LastUpdate = " 26/08/2017 ";

        private static Encoding _defaultEncoding;
        public static CultureInfo CultureInfo;

        internal static object UnixTimeStampToDateTime(double timestamp)
        {
            throw new NotImplementedException();
        }

        private static Game _game;
        private static ConfigurationData _configuration;
        private static ConnectionHandling _connectionManager;
        private static DatabaseManager _manager;
        private static RCONSocket _rcon;

        // TODO: Get rid?
        public static bool Event = false;
        public static DateTime lastEvent;
        public static DateTime ServerStarted;

        private static readonly List<char> Allowedchars = new List<char>(new[]
            {
                'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l',
                'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x',
                'y', 'z', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '-', '.'
            });

        private static ConcurrentDictionary<int, Habbo> _usersCached = new ConcurrentDictionary<int, Habbo>();
        public static string SWFRevision = "";

        public static void Initialize()
        {
            ServerStarted = DateTime.Now;
            Console.WindowWidth = 110;
            Console.WindowHeight = 45;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine();
            Console.WriteLine(@" Dual Server" + PrettyBuild +" "+ VersionCloud +" / Créditos: Xjoao,Paulo!");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Gray;

            Console.WriteLine(
                Console.LargestWindowWidth > 30
                ? @"-------------------------------------------------------------------------------------------------------------"
                : @"");
            Console.WriteLine("");
            _defaultEncoding = Encoding.Default;

            Console.Title = "Dual Server | Carregando...";
            CultureInfo = CultureInfo.CreateSpecificCulture("en-GB");
            try
            {
                _configuration = new ConfigurationData(Path.Combine(Application.StartupPath, @"Settings/config.ini"));

                var connectionString = new MySqlConnectionStringBuilder
                {
                    ConnectionTimeout = 10,
                    Database = GetConfig().data["db.name"],
                    DefaultCommandTimeout = 30,
                    Logging = false,
                    MaximumPoolSize = uint.Parse(GetConfig().data["db.pool.maxsize"]),
                    MinimumPoolSize = uint.Parse(GetConfig().data["db.pool.minsize"]),
                    Password = GetConfig().data["db.password"],
                    Pooling = true,
                    Port = uint.Parse(GetConfig().data["db.port"]),
                    Server = GetConfig().data["db.hostname"],
                    UserID = GetConfig().data["db.username"],
                    AllowZeroDateTime = true,
                    ConvertZeroDateTime = true,
                };

                _manager = new DatabaseManager(connectionString.ToString());

                if (!_manager.IsConnected())
                {
                    log.Warn("» Ya existe una conexión a la base de datos o hay un problema al conectarse con ella.");
                    Console.ReadKey(true);
                    Environment.Exit(1);
                    return;
                }

                log.Info("» Conectado a la Base de datos!");

                #region Add 2016
                HotelName = Convert.ToString(GetConfig().data["hotel.name"]);
                Licenseto = Convert.ToString(GetConfig().data["license"]);
                #endregion Add 2016

                //Reset our statistics first.
                using (IQueryAdapter dbClient = GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.runFastQuery("TRUNCATE `catalog_marketplace_data`");
                    dbClient.runFastQuery("UPDATE `rooms` SET `users_now` = '0' WHERE `users_now` > '0'");
                    dbClient.runFastQuery("UPDATE `users` SET `online` = '0' WHERE `online` = '1'");
                    dbClient.runFastQuery("UPDATE `server_status` SET `users_online` = '0', `loaded_rooms` = '0', `status` = '1'");
                }

                _game = new Game();
                _game.ContinueLoading();

                //Have our encryption ready.
                HabboEncryptionV2.Initialize(new RSAKeys());

                //Make sure MUS is working.
                _rcon = new RCONSocket(GetConfig().data["mus.tcp.bindip"], int.Parse(GetConfig().data["mus.tcp.port"]), GetConfig().data["mus.tcp.allowedaddr"].Split(Convert.ToChar(";")));

                //Accept connections.
                _connectionManager = new ConnectionHandling(int.Parse(GetConfig().data["game.tcp.port"]), int.Parse(GetConfig().data["game.tcp.conlimit"]), int.Parse(GetConfig().data["game.tcp.conperip"]), GetConfig().data["game.tcp.enablenagles"].ToLower() == "true");
                _connectionManager.Init();

                //_game.StartGameLoop();
                TimeSpan TimeUsed = DateTime.Now - ServerStarted;

                Console.WriteLine();

                Console.ForegroundColor = ConsoleColor.Green;
                log.Info("» CLOUD SERVER -> LISTO!! (" + TimeUsed.Seconds + " s, " + TimeUsed.Milliseconds + " ms)");
                Console.ResetColor();
                IsLive = true;

            }
            catch (KeyNotFoundException e)
            {
                log.ErrorFormat("Please check your configuration file - some values appear to be missing.", ConsoleColor.Red);
                log.Error("Press any key to shut down ...");
                ExceptionLogger.LogException(e);
                Console.ReadKey(true);
                Environment.Exit(1);
                return;
            }
            catch (InvalidOperationException e)
            {
                log.Error("Failed to initialize CloudServer: " + e.Message);
                log.Error("Press any key to shut down ...");
                Console.ReadKey(true);
                Environment.Exit(1);
                return;
            }
            catch (Exception e)
            {
                log.Error("Fatal error during startup: " + e);
                log.Error("Press a key to exit");

                Console.ReadKey();
                Environment.Exit(1);
            }
        }

        public static bool EnumToBool(string Enum)
        {
            return (Enum == "1");
        }

        public static string BoolToEnum(bool Bool)
        {
            return (Bool == true ? "1" : "0");
        }

        public static int GetRandomNumber(int Min, int Max)
        {
            return RandomNumber.GenerateNewRandom(Min, Max);
        }

        public static string Rainbow()
        {
            int numColors = 1000;
            var colors = new List<string>();
            var random = new Random();
            for (int i = 0; i < numColors; i++)
            {
                colors.Add(String.Format("#{0:X2}{1:X2}00", i, random.Next(0x1000000) - i));
            }

            int index = 0;
            string rainbow = colors[index];

            if (index > numColors)
                index = 0;
             else
                index++;

            return rainbow;
        }

        public static string RainbowT()
        {
            int numColorst = 1000;
            var colorst = new List<string>();
            var randomt = new Random();
            for (int i = 0; i < numColorst; i++)
            {
                colorst.Add(String.Format("#{0:X6}", randomt.Next(0x1000000)));
            }

            int indext = 0;
            string rainbowt = colorst[indext];

            if (indext > numColorst)
                indext = 0;
            else
                indext++;

            return rainbowt;
        }

        public static double GetUnixTimestamp()
        {
            TimeSpan ts = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
            return ts.TotalSeconds;
        }

        internal static int GetIUnixTimestamp()
        {
            var ts = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
            var unixTime = ts.TotalSeconds;
            return Convert.ToInt32(unixTime);
        }

        private static readonly DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static long CurrentTimeMillis()
        {
            return (long)(DateTime.UtcNow - Jan1st1970).TotalMilliseconds;
        }


        public static long Now()
        {
            TimeSpan ts = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
            double unixTime = ts.TotalMilliseconds;
            return (long)unixTime;
        }

        public static string FilterFigure(string figure)
        {
            foreach (char character in figure)
            {
                if (!IsValid(character))
                    return "sh-3338-93.ea-1406-62.hr-831-49.ha-3331-92.hd-180-7.ch-3334-93-1408.lg-3337-92.ca-1813-62";
            }

            return figure;
        }

        private static bool IsValid(char character)
        {
            return Allowedchars.Contains(character);
        }

        public static bool IsValidAlphaNumeric(string inputStr)
        {
            inputStr = inputStr.ToLower();
            if (string.IsNullOrEmpty(inputStr))
            {
                return false;
            }

            for (int i = 0; i < inputStr.Length; i++)
            {
                if (!IsValid(inputStr[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public static string GetUsernameById(int UserId)
        {
            string Name = "Unknown User";

            GameClient Client = GetGame().GetClientManager().GetClientByUserID(UserId);
            if (Client != null && Client.GetHabbo() != null)
                return Client.GetHabbo().Username;

            UserCache User = GetGame().GetCacheManager().GenerateUser(UserId);
            if (User != null)
                return User.Username;

            using (IQueryAdapter dbClient = GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `username` FROM `users` WHERE id = @id LIMIT 1");
                dbClient.AddParameter("id", UserId);
                Name = dbClient.getString();
            }

            if (string.IsNullOrEmpty(Name))
                Name = "Unknown User";

            return Name;
        }

        public static bool ShutdownStarted { get; set; }
		
        public static Habbo GetHabboById(int UserId)
        {
            try
            {
                GameClient Client = GetGame().GetClientManager().GetClientByUserID(UserId);
                if (Client != null)
                {
                    Habbo User = Client.GetHabbo();
                    if (User != null && User.Id > 0)
                    {
                        if (_usersCached.ContainsKey(UserId))
                            _usersCached.TryRemove(UserId, out User);
                        return User;
                    }
                }
                else
                {
                    try
                    {
                        if (_usersCached.ContainsKey(UserId))
                            return _usersCached[UserId];
                        else
                        {
                            UserData data = UserDataFactory.GetUserData(UserId);
                            if (data != null)
                            {
                                Habbo Generated = data.user;
                                if (Generated != null)
                                {
                                    Generated.InitInformation(data);
                                    _usersCached.TryAdd(UserId, Generated);
                                    return Generated;
                                }
                            }
                        }
                    }
                    catch { return null; }
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public static Habbo GetHabboByUsername(String UserName)
        {
            try
            {
                using (IQueryAdapter dbClient = GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("SELECT `id` FROM `users` WHERE `username` = @user LIMIT 1");
                    dbClient.AddParameter("user", UserName);
                    int id = dbClient.getInteger();
                    if (id > 0)
                        return GetHabboById(Convert.ToInt32(id));
                }
                return null;
            }
            catch { return null; }
        }



        public static void PerformShutDown()
        {
            PerformShutDown(false);
        }

        public static void PerformRestart()
        {
            PerformShutDown(true);
            using (IQueryAdapter dbClient = _manager.GetQueryReactor())
            {
                dbClient.runFastQuery("UPDATE `server_status` SET `status` = '1'");
            }
        }
		
        public static void PerformShutDown(bool restart)
        {
            Console.Clear();
            log.Info("Apagando el servidor...");
            Console.Title = "CLOUD: APAGANDO!";

            ShutdownStarted = true;

            GetGame().GetClientManager().SendMessage(new BroadcastMessageAlertComposer(GetGame().GetLanguageManager().TryGetValue("server.shutdown.message")));
            GetGame().StopGameLoop();
            System.Threading.Thread.Sleep(2500);
            GetConnectionManager().Destroy();//Stop listening.
            GetGame().GetPacketManager().UnregisterAll();//Unregister the packets.
            GetGame().GetPacketManager().WaitForAllToComplete();
            GetGame().GetClientManager().CloseAll();//Close all connections
            GetGame().GetRoomManager().Dispose();//Stop the game loop.

            GetConnectionManager().Destroy();

            using (IQueryAdapter dbClient = _manager.GetQueryReactor())
            {
                dbClient.runFastQuery("TRUNCATE `catalog_marketplace_data`");
                dbClient.runFastQuery("UPDATE `users` SET online = '0', `auth_ticket` = NULL");
                dbClient.runFastQuery("UPDATE `rooms` SET `users_now` = '0'");
            }

            _connectionManager.Destroy();
            _game.Destroy();

            log.Info("Cloud Server Fue Apagado con Exito.");

            if (!restart)
                log.WarnFormat("Apagado Completo. Presione una tecla para continuar...", ConsoleColor.DarkRed);

            if (!restart)
                Console.ReadKey();

            IsLive = false;

            if (restart)
                Process.Start(Assembly.GetEntryAssembly().Location);

            if (restart)
                Console.WriteLine("Reiniciando...");
            else
                Console.WriteLine("Cerrando...");

            System.Threading.Thread.Sleep(1000);
            Environment.Exit(0);
        }

        public static ConfigurationData GetConfig()
        {
            return _configuration;
        }

        public static Encoding GetDefaultEncoding()
        {
            return _defaultEncoding;
        }

        public static ConnectionHandling GetConnectionManager()
        {
            return _connectionManager;
        }

        public static Game GetGame()
        {
            return _game;
        }

        public static RCONSocket GetRCONSocket()
        {
            return _rcon;
        }

        public static DatabaseManager GetDatabaseManager()
        {
            return _manager;
        }

        public static ICollection<Habbo> GetUsersCached()
        {
            return _usersCached.Values;
        }

        public static bool RemoveFromCache(int Id, out Habbo Data)
        {
            return _usersCached.TryRemove(Id, out Data);
        }
    }
}