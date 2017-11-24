using System.Collections.Generic;

namespace Cloud.HabboHotel.Users.Permissions
{
    public sealed class PermissionComponent
    {
        private readonly List<string> _permissions;

        private readonly List<string> _commands;

        public PermissionComponent()
        {
            this._permissions = new List<string>();
            this._commands = new List<string>();
        }

        public bool Init(Habbo Player)
        {
            if (this._permissions.Count > 0)
                this._permissions.Clear();

            if (this._commands.Count > 0)
                this._commands.Clear();

            this._permissions.AddRange(CloudServer.GetGame().GetPermissionManager().GetPermissionsForPlayer(Player));
            this._commands.AddRange(CloudServer.GetGame().GetPermissionManager().GetCommandsForPlayer(Player));
            return true;
        }

        public bool HasRight(string Right)
        {
            return this._permissions.Contains(Right);
        }

        public bool HasCommand(string Command)
        {
            return this._commands.Contains(Command);
        }

        public void Dispose()
        {
            this._permissions.Clear();
        }
    }
}
