using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;

namespace ReSharperPlugin.GitIntegration.Git;

public class GitRepositoryChecker(ISolution solution)
{
    public async Task<bool> IsPartOfGitRepository(CancellationToken cancellationToken)
    {
        try
        {
            var output = await GitCommandExecutor.ExecuteCommandAsync("status",
                solution.SolutionDirectory.FullPath, cancellationToken);
            Console.WriteLine(output);
            return true;
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Git operation was canceled.");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Git operation failed: {ex.Message}");
            return false;
        }
    }
    
}