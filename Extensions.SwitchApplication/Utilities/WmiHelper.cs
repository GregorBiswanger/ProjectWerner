using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using ProjectWerner.SwitchApplication.Interfaces;
using ProjectWerner.SwitchApplication.Model;

namespace ProjectWerner.SwitchApplication.Utilities
{
    public static class WmiHelper
    {
        private static readonly string Scope = $@"\\{Environment.MachineName}\root\CIMV2";
        private static readonly string QuerySelectAll = "SELECT * FROM Win32_Process";
        private static readonly string QuerySelectByProcessId = "SELECT * FROM Win32_Process WHERE ProcessId = '{0}'";
        private const string Name = "Name";
        private const string ExecutablePath = "ExecutablePath";
        private const string ProcessId = "ProcessId";
        private const string Description = "Description";
        private const string Caption = "Caption";
        private const string CSName = "CSName";

        public static dynamic GetProcessInfo(int processId)
        {
            var scope = new ManagementScope(Scope);
            scope.Connect();
            var query = new SelectQuery(string.Format(QuerySelectByProcessId, processId));

            var mos = new ManagementObjectSearcher(scope, query);
            var queryCollection = mos.Get();

            var programFiles = (Environment.Is64BitOperatingSystem 
                ? Environment.ExpandEnvironmentVariables("%ProgramFiles%") 
                : Environment.ExpandEnvironmentVariables("%ProgramFiles(x86)%")) + @"\";

            var list = (from ManagementBaseObject foundObject in queryCollection
                        orderby foundObject[Name]
                        let tmp = foundObject[ExecutablePath]?.ToString() ?? string.Empty
                        let name = foundObject[Name]?.ToString() ?? string.Empty
                        let pId = (uint?)foundObject[ProcessId] ?? 0
                        select new ProcessInfo
                        {
                            FileName = foundObject[Name]?.ToString(),
                            Caption = foundObject[Caption]?.ToString(),
                            CSName = foundObject[CSName]?.ToString(),
                            Description = foundObject[Description]?.ToString(),
                            ProcessId = (int)pId,
                            ExecutionPath = tmp,
                            DisplayName = tmp.Length < programFiles.Length || string.IsNullOrEmpty(tmp)
                            ? string.Empty
                            : (tmp.Replace(programFiles, "").Replace(name, ""))
                            .Trim('\\', '/', ' ')
                        }).ToList();
            return list;
        }

        public static IEnumerable<IProcessInfo> GetAllCurrentlyRunningProcesses()
        {
            var scope = new ManagementScope(Scope);
            scope.Connect();

            var query = new SelectQuery(QuerySelectAll);

            var mos = new ManagementObjectSearcher(scope, query);
            var queryCollection = mos.Get();

            var system32 = Environment.ExpandEnvironmentVariables("%windir%");
            var commonFilesX64 = Environment.ExpandEnvironmentVariables("CommonProgramFiles");
            var commonFilesX86 = Environment.ExpandEnvironmentVariables("%CommonProgram(x86)%");
            var programmData = Environment.ExpandEnvironmentVariables("%ProgramData%");

            var programFiles = (Environment.Is64BitOperatingSystem
                ? Environment.ExpandEnvironmentVariables("%ProgramFiles%")
                : Environment.ExpandEnvironmentVariables("%ProgramFiles(x86)%")) + @"\";

            var list =
            (from ManagementBaseObject foundObject in queryCollection
             orderby foundObject[Name]
             let tmp = foundObject[ExecutablePath]?.ToString() ?? string.Empty
             let name = foundObject[Name]?.ToString() ?? string.Empty
             let pId = (uint?)foundObject[ProcessId] ?? 0
             select new ProcessInfo
             {
                 FileName = name,
                 Name = name,
                 Caption = foundObject[Caption]?.ToString(),
                 CSName = foundObject[CSName]?.ToString(),
                 Description = foundObject[Description]?.ToString(),
                 ProcessId = (int)pId,
                 ExecutionPath = tmp,
                 DisplayName = tmp.Length < programFiles.Length || string.IsNullOrEmpty(tmp)
                 ? string.Empty
                 : (tmp.Replace(programFiles, "").Replace(name, ""))
                 .Trim('\\', '/', ' ')
             }).ToList();
            //(from ManagementBaseObject foundObject in queryCollection
            // orderby foundObject[Name]
            // let executablePath = foundObject[ExecutablePath]?.ToString() ?? string.Empty
            // let fileName = foundObject[Name]?.ToString() ?? string.Empty
            // let processId = (uint?)foundObject[ProcessId] ?? 0
            // where foundObject[Name] != null && !string.IsNullOrEmpty(executablePath)
            // select new ProcessInfo
            // {
            //     FileName = fileName,
            //     ExecutionPath = executablePath,
            //     Arguments = string.Empty,
            //     ProcessId = (int)processId,
            //     Description = foundObject[Description]?.ToString() ?? string.Empty,

            // })
            //        .ToList();

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
