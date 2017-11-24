using System;
using log4net;
using Cloud.Communication.Packets.Outgoing.Catalog;

using Cloud.Communication.Packets.Outgoing.Moderation;

namespace Cloud.Core
{
    public class ConsoleCommandHandler
    {
        private static readonly ILog log = LogManager.GetLogger("Cloud.Core.ConsoleCommandHandler");

        public static void InvokeCommand(string inputData)
        {
            if (string.IsNullOrEmpty(inputData))
                return;

            try
            {
                #region Command parsing
                string[] parameters = inputData.Split(' ');

                switch (parameters[0].ToLower())
                {
                    #region targeted
                    case "relampago":
                    case "targeted":
                        CloudServer.GetGame().GetTargetedOffersManager().Initialize(CloudServer.GetDatabaseManager().GetQueryReactor());
                        break;
                    #endregion targeted
                    #region stop
                    case "stop":
                    case "shutdown":
                        ExceptionLogger.DisablePrimaryWriting(true);
                        log.Warn("The server is saving users furniture, rooms, etc. WAIT FOR THE SERVER TO CLOSE, DO NOT EXIT THE PROCESS IN TASK MANAGER!!");
                        CloudServer.PerformShutDown(false);
                        break;
                    #endregion
                    #region actualizarcatalogo
                    case "catalog":
                    case "catalogue":
                    case "actualizarcatalogo":
                        CloudServer.GetGame().GetCatalog().Init(CloudServer.GetGame().GetItemManager());
                        CloudServer.GetGame().GetClientManager().SendMessage(new CatalogUpdatedComposer());
                        break;
                    #endregion
                    #region actualizaritems
                    case "items":
                    case "furnis":
                    case "furniture":
                        CloudServer.GetGame().GetItemManager().Init();
                        break;
                    #endregion
                    #region restart
                    case "restart":
                    case "reiniciar":
                        ExceptionLogger.DisablePrimaryWriting(true);
                        log.Warn("The server is saving users furniture, rooms, etc. WAIT FOR THE SERVER TO CLOSE, DO NOT EXIT THE PROCESS IN TASK MANAGER!!");
                        CloudServer.PerformShutDown(true);
                        break;
                    #endregion
                    #region Clear Console
                    case "clear":
                        Console.Clear();
                        break;
                    #endregion
                    #region alert
                    case "alert":
                        string Notice = inputData.Substring(6);
                        CloudServer.GetGame().GetClientManager().SendMessage(new BroadcastMessageAlertComposer(CloudServer.GetGame().GetLanguageManager().TryGetValue("server.console.alert") + "\n\n" + Notice));
                        log.Info("Alerta enviada correctamente.");
                        break;
                    #endregion
                    #region navigator
                    case "navi":
                    case "navegador":
                    case "navigator":
                        CloudServer.GetGame().GetNavigator().Init();
                        break;
                    #endregion
                    #region configs
                    case "config":
                    case "settings":
                        CloudServer.GetGame().GetSettingsManager().Init();
                        ExtraSettings.RunExtraSettings();
                        CatalogSettings.RunCatalogSettings();
                        NotificationSettings.RunNotiSettings();
                        CloudServer.GetGame().GetTargetedOffersManager().Initialize(CloudServer.GetDatabaseManager().GetQueryReactor());
                        break;
                    #endregion

                    default:
                        log.Error(parameters[0].ToLower() + " is an unknown or unsupported command. Type help for more information");
                        break;
                }
                #endregion
            }
            catch (Exception e)
            {
                log.Error("Error in command [" + inputData + "]: " + e);
            }
        }
    }
}
