using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using ProjectWerner.SwitchApplication.Interfaces;
using ProjectWerner.SwitchApplication.Model;

namespace ProjectWerner.SwitchApplication.Utilities
{
    public static class WmiHelper
    {
        private static readonly string Scope = $@"\\{Environment.MachineName}\root\CIMV2";
        private static readonly string Query = "SELECT * FROM Win32_Process";
        private const string Name = "Name";
        private const string ExecutablePath = "ExecutablePath";
        private const string ProcessId = "ProcessId";

        public static IEnumerable<IProcessInfo> GetAllCurrentlyRunningProcesses()
        {
            var scope = new ManagementScope(Scope);
            scope.Connect();

            var query = new SelectQuery(Query);

            var mos = new ManagementObjectSearcher(scope, query);
            var queryCollection = mos.Get();

            var system32 = Environment.ExpandEnvironmentVariables("%windir%");
            var commonFilesX64 = Environment.ExpandEnvironmentVariables("CommonProgramFiles");
            var commonFilesX86 = Environment.ExpandEnvironmentVariables("%CommonProgram(x86)%");
            var programmData = Environment.ExpandEnvironmentVariables("%ProgramData%");

            var list = (from ManagementBaseObject foundObject in queryCollection
                        orderby foundObject[Name]
                        let executablePath = foundObject[ExecutablePath]?.ToString() ?? string.Empty
                        let fileName = foundObject[Name]?.ToString() ?? string.Empty
                        let processId = (uint?)foundObject[ProcessId] ?? 0
                        where foundObject[Name] != null && !string.IsNullOrEmpty(executablePath)
                        select new ProcessInfo(fileName, executablePath, string.Empty, (int)processId))
                        .ToList();

            list = list.Where(l => l.ExecutionPath.StartsWith(system32) == false
                                   && l.ExecutionPath.StartsWith(commonFilesX86) == false
                                   && l.ExecutionPath.StartsWith(commonFilesX64) == false
                                   && l.ExecutionPath.StartsWith(programmData) == false)
            .ToList();
            
            var result = new List<ProcessInfo>();
            foreach (var processInfo in list)
            {
                if (result.Any(l => l.FileName == processInfo.FileName) == false)
                    result.Add(processInfo);
            }

            return result;
        }
    }
}
