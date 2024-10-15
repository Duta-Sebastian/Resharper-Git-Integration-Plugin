using System;
using JetBrains.ProjectModel;

namespace ReSharperPlugin.GitIntegration.Git;

public class GitRecentCommitsGetter(ISolution solution, int commitCount)
{
    public string GetGitRecentCommits()
    {
        try
        {
            return GitCommandExecutor.ExecuteCommand(
                $"log -n {commitCount} --name-only " +
                $"--pretty=format:\"<COMMIT>%h%nMessage: %s\"",
                solution.SolutionDirectory.FullPath);
        }
        catch (Exception e)
        {
            Console.WriteLine($@"Git operation failed: {e.Message}");
            return null;
        }
    }
}