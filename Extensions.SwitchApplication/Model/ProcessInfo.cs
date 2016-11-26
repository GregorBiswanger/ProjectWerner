using ProjectWerner.SwitchApplication.Interfaces;

namespace ProjectWerner.SwitchApplication.Model
{
    public class ProcessInfo : IProcessInfo
    {
        #region Implementation of IProcessInfo

        /// <summary>
        /// Exe Filename, e.g. notepad.exe
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// Path to execution location of <seealso cref="IProcessInfo.FileName"/>
        /// </summary>
        public string ExecutionPath { get; private set; }

        /// <summary>
        /// Arguments to push into / during startup
        /// </summary>
        public string Arguments { get; private set; }

        /// <summary>
        /// ProcessId
        /// </summary>
        public int ProcessId { get; set; }

        #endregion

        public ProcessInfo(string fileName, string executionPath, string arguments, int processId)
        {
            this.FileName = fileName;
            this.ExecutionPath = executionPath;
            this.Arguments = arguments;
            this.ProcessId = processId;
        }
    }
}
