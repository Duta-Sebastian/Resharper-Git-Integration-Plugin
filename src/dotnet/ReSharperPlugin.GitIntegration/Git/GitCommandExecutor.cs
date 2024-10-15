using System;
using System.Diagnostics;

namespace ReSharperPlugin.GitIntegration.Git;

public static class GitCommandExecutor
{
    public static string ExecuteCommand(string command, string workingDirectory)
    {
        var processInfo = new ProcessStartInfo("git", command)
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        if (!string.IsNullOrEmpty(workingDirectory)) processInfo.WorkingDirectory = workingDirectory;

        using var process = new Process();
        process.StartInfo = processInfo;
        process.Start();

        var output = process.StandardOutput.ReadToEnd();
        var error = process.StandardError.ReadToEnd();


        process.WaitForExit();

        if (process.ExitCode != 0) throw new Exception($"Git command failed: {error}");

        return output;
    }
}