using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Util;

namespace ReSharperPlugin.GitIntegration.Git;

public static class GitCommandExecutor
{
    public static async Task<string> ExecuteCommandAsync(string command, string workingDirectory,
        CancellationToken cancellationToken)
    {
        var processInfo = new ProcessStartInfo("git", command)
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        if (!string.IsNullOrEmpty(workingDirectory))
        {
            processInfo.WorkingDirectory = workingDirectory;
        }

        using var process = new Process();
        process.StartInfo = processInfo;
        process.Start();
        cancellationToken.Register(() =>
        {
            try
            {
                if (!process.HasExited)
                {
                    process.KillTree();
                }
            }
            catch
            {
                throw;
            }
        });
        
        var outputTask = process.StandardOutput.ReadToEndAsync();
        var errorTask = process.StandardError.ReadToEndAsync();

        
        await Task.WhenAll(outputTask, errorTask);

        if (process.ExitCode != 0)
        {
            throw new Exception($"Git command failed: { await errorTask}");
        }

        return await outputTask;
    }
}