using System;
using System.Collections.Generic;

namespace ReSharperPlugin.GitIntegration.Git;

public class GitCommitFormatter(string gitLogOutput, string solutionDirectoryPath)
{
    public Dictionary<string, string> FormatCommitMessage()
    {
        var gitLogDictionary = new Dictionary<string, string>();
        var lines = gitLogOutput.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

        string currentHash = null;
        string currentMessage = null;
        foreach (var line in lines)
            if (line.StartsWith("<COMMIT>"))
            {
                currentHash = line.Replace("<COMMIT>", "").Trim();
            }
            else if (line.StartsWith("Message:"))
            {
                currentMessage = line.Replace("Message:", "").Trim();
            }
            else if (!string.IsNullOrWhiteSpace(line) && currentHash != null && currentMessage != null)
            {
                var fileName = ($"{solutionDirectoryPath}/" + line.Trim()).Replace('/', '\\');
                var commitInfo = $"{currentHash} - {currentMessage}";
                if (gitLogDictionary.ContainsKey(fileName))
                    gitLogDictionary[fileName] += "\n" + commitInfo;
                else
                    gitLogDictionary[fileName] = commitInfo;

                currentHash = null;
                currentMessage = null;
            }

        return gitLogDictionary;
    }
}