using System.Collections.Generic;
using System.Diagnostics;

/// <summary>
///  Class encompassing the optional settings for running processes.
/// </summary>
public class RunOptions
{
    /// <summary>
    ///  The working directory of the process.
    /// </summary>
    public string WorkingDirectory { get; set; }
    /// <summary>
    ///  Container logging the StandardOutput content.
    /// </summary>
    public IList<string> StandardOutputListing { get; set; }
    /// <summary>
    ///  Desired maximum time-out for the process
    /// </summary>
    public int TimeOut { get; set; }
}

/// <summary>
///  Wrapper for the exit code and state.
///  Used to query the result of an execution with method calls.
/// </summary>
public struct ExitStatus
{
    private int _code;
    private bool _timeOut;
    /// <summary>
    ///  Default constructor when the execution finished.
    /// </summary>
    /// <param name="code">The exit code</param>
    public ExitStatus(int code)
    {
        this._code = code;
        this._timeOut = false;
    }
    /// <summary>
    ///  Default constructor when the execution potentially timed out.
    /// </summary>
    /// <param name="code">The exit code</param>
    /// <param name="timeOut">True if the execution timed out</param>
    public ExitStatus(int code, bool timeOut)
    {
        this._code = code;
        this._timeOut = timeOut;
    }
    /// <summary>
    ///  Flag signalling that the execution timed out.
    /// </summary>
    public bool DidTimeOut { get { return _timeOut; } }
    /// <summary>
    ///  Implicit conversion from ExitStatus to the exit code.
    /// </summary>
    /// <param name="exitStatus">The exit status</param>
    /// <returns>The exit code</returns>
    public static implicit operator int(ExitStatus exitStatus)
    {
        return exitStatus._code;
    }
    /// <summary>
    ///  Trigger Exception for non-zero exit code.
    /// </summary>
    /// <param name="errorMessage">The message to use in the Exception</param>
    /// <returns>The exit status for further queries</returns>
    public ExitStatus ExceptionOnError(string errorMessage)
    {
        if (this._code != 0)
        {
            throw new Exception(errorMessage);
        }
        return this;
    }
}

/// <summary>
/// Run a command in the OS specific shell (powershell or bash).
/// </summary>
/// <param name="command">The command to execute in the shell</param>
/// <returns>The exit status for further queries</returns>
ExitStatus Shell(string command)
{
    return Shell(command, new RunOptions());
}

/// <summary>
/// Run a command in the OS specific shell (powershell or bash).
/// </summary>
/// <param name="command">The command to execute in the shell</param>
/// <param name="workingDirectory">Working directory</param>
/// <returns>The exit status for further queries</returns>
ExitStatus Shell(string command, string workingDirectory)
{
    return Shell(command,
        new RunOptions()
        {
            WorkingDirectory = workingDirectory
        });
}

/// <summary>
/// Run a command in the OS specific shell (powershell or bash).
/// </summary>
/// <param name="command">The command to execute in the shell</param>
/// <param name="runOptions">Optional settings</param>
/// <returns>The exit status for further queries</returns>
ExitStatus Shell(string command, RunOptions runOptions)
{
    Verbose($"Shell: {command}");
    var exec = IsRunningOnWindows() ? "powershell" : "bash";
    var args = IsRunningOnWindows() ? $"/Command {command}" : $"-C {command}";
    return Run(exec, args, runOptions);
}

/// <summary>
///  Run the given executable with the given arguments.
/// </summary>
/// <param name="exec">Executable to run</param>
/// <param name="args">Arguments</param>
/// <returns>The exit status for further queries</returns>
ExitStatus Run(string exec, string args)
{
    return Run(exec, args, new RunOptions());
}

/// <summary>
///  Run the given executable with the given arguments.
/// </summary>
/// <param name="exec">Executable to run</param>
/// <param name="args">Arguments</param>
/// <param name="workingDirectory">Working directory</param>
/// <returns>The exit status for further queries</returns>
ExitStatus Run(string exec, string args, string workingDirectory)
{
    return Run(exec, args,
        new RunOptions()
        {
            WorkingDirectory = workingDirectory
        });
}

/// <summary>
///  Run the given executable with the given arguments.
/// </summary>
/// <param name="exec">Executable to run</param>
/// <param name="args">Arguments</param>
/// <param name="runOptions">Optional settings</param>
/// <returns>The exit status for further queries</returns>
ExitStatus Run(string exec, string args, RunOptions runOptions)
{
    var workingDirectory = runOptions.WorkingDirectory ?? System.IO.Directory.GetCurrentDirectory();
    var process = System.Diagnostics.Process.Start(
            new ProcessStartInfo(exec, args)
            {
                WorkingDirectory = workingDirectory,
                UseShellExecute = false,
                RedirectStandardOutput = runOptions.StandardOutputListing != null
            });
    if (runOptions.StandardOutputListing != null)
    {
        process.OutputDataReceived += (s, e) =>
        {
            if (e.Data != null)
            {
                runOptions.StandardOutputListing.Add(e.Data);
            }
        };
        process.BeginOutputReadLine();
    }
    if (runOptions.TimeOut == 0)
    {
        process.WaitForExit();
        return new ExitStatus(process.ExitCode);
    }
    else
    {
        bool finished = process.WaitForExit(runOptions.TimeOut);
        if (finished)
        {
            return new ExitStatus(process.ExitCode);
        }
        else
        {
            KillProcessTree(process);
            return new ExitStatus(0, timeout: true);
        }
    }
}

/// <summary>
///  Kill the given process and all its child processes.
/// </summary>
/// <param name="process">Root process</param>
public void KillProcessTree(Process process)
{
    // Child processes are not killed on Windows by default
    // Use TASKKILL to kill the process hierarchy rooted in the process
    if (IsRunningOnWindows())
    {
        StartProcess($"TASKKILL",
            new ProcessSettings
            {
                Arguments = $"/PID {process.Id} /T /F",
            });
    }
    else
    {
        process.Kill();
    }
}
