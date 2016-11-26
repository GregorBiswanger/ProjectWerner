using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;

namespace ProjectWerner.SwitchApplication.Utilities
{
    public static class WmiHelper
    {
        private static readonly string Scope = $@"\\{Environment.MachineName}\root\CIMV2";
        private static readonly string Query = "SELECT * FROM Win32_Process";
        private const string Name = "Name";

        public static IEnumerable<string> ShowAllCurrentlyRunningProcesses()
        {
            var scope = new ManagementScope(Scope);
            scope.Connect();

            var query = new SelectQuery(Query);

            var mos = new ManagementObjectSearcher(scope, query);
            var queryCollection = mos.Get();

            var list = (from ManagementBaseObject foundObject in queryCollection
                        orderby foundObject[Name]
                        where foundObject[Name] != null
                        select foundObject[Name].ToString())
                        .Distinct()
                        .ToList();
            return list;
        }
    }
}
