namespace ProjectWerner.SwitchApplication.Interfaces
{
    public interface IProcessInfo
    {
        /// <summary>
        /// Exe Filename, e.g. notepad.exe
        /// </summary>
        string FileName { get; }
        
        /// <summary>
        /// Path to execution location of <seealso cref="FileName"/>
        /// </summary>
        string ExecutionPath { get; }

        /// <summary>
        /// Arguments to push into / during startup
        /// </summary>
        string Arguments { get; }

        /// <summary>
        /// ProcessId of one of the current running processes
        /// </summary>
        int ProcessId { get; }
    }
}
