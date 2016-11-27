using ProjectWerner.SwitchApplication.Interfaces;

namespace ProjectWerner.SwitchApplication.Model
{
    public class ProcessInfo : IProcessInfo
    {

        /// <summary>
        /// Exe Filename, e.g. notepad.exe
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Path to execution location of <seealso cref="IProcessInfo.FileName"/>
        /// </summary>
        public string ExecutionPath { get; set; }

        /// <summary>
        /// Arguments to push into / during startup
        /// </summary>
        public string Arguments { get; set; }

        /// <summary>
        /// ProcessId
        /// </summary>
        public int ProcessId { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; set; }

    }
}
