using System;
using System.Linq;
using System.Collections.Generic;
using log4net;
using Cloud.Database.Interfaces;
using System.Data;
using Cloud.HabboHotel.Users;

namespace Cloud.HabboHotel.Permissions
{
    public sealed class PermissionManager
    {
        private static readonly ILog log = LogManager.GetLogger("Cloud.HabboHotel.Permissions.PermissionManager");
        private readonly Dictionary<int, Permission> Permissions = new Dictionary<int, Permission>();
        private readonly Dictionary<string, PermissionCommand> _commands = new Dictionary<string, PermissionCommand>();
        private readonly Dictionary<int, PermissionGroup> PermissionGroups = new Dictionary<int, PermissionGroup>();
        private readonly Dictionary<int, List<string>> PermissionGroupRights = new Dictionary<int, List<string>>();
        private readonly Dictionary<int, List<string>> PermissionSubscriptionRights = new Dictionary<int, List<string>>();

        public PermissionManager()
        {

        }

        public void Init()
        {
           Permissions.Clear();
           _commands.Clear();
           PermissionGroups.Clear();
           PermissionGroupRights.Clear();

            using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `permissions`");
                DataTable GetPermissions = dbClient.getTable();

                if (GetPermissions != null)
                {
                    foreach (DataRow Row in GetPermissions.Rows)
                    {
                        Permissions.Add(Convert.ToInt32(Row["id"]), new Permission(Convert.ToInt32(Row["id"]), Convert.ToString(Row["permission"]), Convert.ToString(Row["description"])));
                    }
                }
            }

            using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `permissions_commands`");
                DataTable GetCommands = dbClient.getTable();

                if (GetCommands != null)
                {
                    foreach (DataRow Row in GetCommands.Rows)
                    {
                         _commands.Add(Convert.ToString(Row["command"]), new PermissionCommand(Convert.ToString(Row["command"]), Convert.ToInt32(Row["group_id"]), Convert.ToInt32(Row["subscription_id"])));
                    }
                }
            }

            using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `permissions_groups`");
                DataTable GetPermissionGroups = dbClient.getTable();

                if (GetPermissionGroups != null)
                {
                    foreach (DataRow Row in GetPermissionGroups.Rows)
                    {
                        PermissionGroups.Add(Convert.ToInt32(Row["id"]), new PermissionGroup(Convert.ToString("name"), Convert.ToString("description"), Convert.ToString("badge")));
                    }
                }
            }

            using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `permissions_rights`");
                DataTable GetPermissionRights = dbClient.getTable();

                if (GetPermissionRights != null)
                {
                    foreach (DataRow Row in GetPermissionRights.Rows)
                    {
                        int GroupId = Convert.ToInt32(Row["group_id"]);
                        int PermissionId = Convert.ToInt32(Row["permission_id"]);

                        if (!PermissionGroups.ContainsKey(GroupId))
                        {
                            continue; // permission group does not exist
                        }

                        Permission Permission = null;

                        if (!Permissions.TryGetValue(PermissionId, out Permission))
                        {
                            continue; // permission does not exist
                        }

                        if (PermissionGroupRights.ContainsKey(GroupId))
                        {
                            PermissionGroupRights[GroupId].Add(Permission.PermissionName);
                        }
                        else
                        {
                            List<string> RightsSet = new List<string>()
                                {
                                    Permission.PermissionName
                                };

                            PermissionGroupRights.Add(GroupId, RightsSet);
                        }

                    }
                }
            }

            using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `permissions_subscriptions`");
                DataTable GetPermissionSubscriptions = dbClient.getTable();

                if (GetPermissionSubscriptions != null)
                {
                    foreach (DataRow Row in GetPermissionSubscriptions.Rows)
                    {
                        int PermissionId = Convert.ToInt32(Row["permission_id"]);
                        int SubscriptionId = Convert.ToInt32(Row["subscription_id"]);

                        Permission Permission = null;
                        if (!Permissions.TryGetValue(PermissionId, out Permission))
                            continue; // permission does not exist

                        if (PermissionSubscriptionRights.ContainsKey(SubscriptionId))
                        {
                            PermissionSubscriptionRights[SubscriptionId].Add(Permission.PermissionName);
                        }
                        else
                        {
                            List<string> RightsSet = new List<string>()
                                {
                                    Permission.PermissionName
                                };

                            PermissionSubscriptionRights.Add(SubscriptionId, RightsSet);
                        }
                    }
                }
            }

            log.Info("» Cargados: " + Permissions.Count + " permisos.");
            log.Info("» Cargados: " + PermissionGroups.Count + " grupo de permisos.");
            log.Info("» Cargados: " + PermissionGroupRights.Count + " derechos de los grupos de permisos.");
            log.Info("» Cargados: " + PermissionSubscriptionRights.Count + " permisos con derecho de suscripción.");
        }

        public bool TryGetGroup(int Id, out PermissionGroup Group)
        {
            return PermissionGroups.TryGetValue(Id, out Group);
        }

        public List<string> GetPermissionsForPlayer(Habbo Player)
        {
            List<string> PermissionSet = new List<string>();

            List<string> PermRights = null;
            if (PermissionGroupRights.TryGetValue(Player.Rank, out PermRights))
            {
                PermissionSet.AddRange(PermRights);
            }

            List<string> SubscriptionRights = null;
            if (PermissionSubscriptionRights.TryGetValue(Player.VIPRank, out SubscriptionRights))
            {
                PermissionSet.AddRange(SubscriptionRights);
            }

            return PermissionSet;
        }

        public List<string> GetCommandsForPlayer(Habbo Player)
        {
            return _commands.Where(x => Player.Rank >= x.Value.GroupId && Player.VIPRank >= x.Value.SubscriptionId).Select(x => x.Key).ToList();
        }
    }
}