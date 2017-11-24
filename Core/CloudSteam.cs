#region

using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using log4net;

#endregion

namespace Cloud.Core
{
    class CloudSteam
    {
        public static string LICENSE = "";
        private static readonly ILog log = LogManager.GetLogger("Cloud.CloudServer");

        public static bool RunLicenseKey()
        {
            if (!File.Exists("Settings/license.ini"))
                return false;
            foreach (var @params in from line in File.ReadAllLines("Settings/license.ini", Encoding.Default) where !String.IsNullOrWhiteSpace(line) && line.Contains("=") select line.Split('='))
            {
                switch (@params[0])
                {
                    case "license":
                        LICENSE = @params[1];
                        break;
                }
            }
            return true;
        }
    }
}