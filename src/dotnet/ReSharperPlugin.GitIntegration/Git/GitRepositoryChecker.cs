using System;
using JetBrains.ProjectModel;

namespace ReSharperPlugin.GitIntegration.Git;

public class GitRepositoryChecker(ISolution solution)
{
    public bool IsPartOfGitRepository()
    {
        try
        {
            GitCommandExecutor.ExecuteCommand("status",
                solution.SolutionDirectory.FullPath);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($@"Git operation failed: {ex.Message}");
            return false;
        }
    }
}