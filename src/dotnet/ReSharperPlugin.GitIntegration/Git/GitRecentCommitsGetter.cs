using System;
using JetBrains.ProjectModel;

namespace ReSharperPlugin.GitIntegration.Git;

public class GitRecentCommitsGetter(ISolution solution, int commitCount)
{
    public string GetGitRecentCommits()
    {
        try
        {
            var output = GitCommandExecutor.ExecuteCommand(
                $"log -n {commitCount} --name-status --pretty=format:\"%H - %s\" --abbrev-commit",
                solution.SolutionDirectory.FullPath);
            return output;
        }
        catch (Exception e)
        {
            Console.WriteLine($@"Git operation failed: {e.Message}");
            return null;
        }
    }
}