using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using JetBrains.Application.Settings;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.Util;
using ReSharperPlugin.GitIntegration.Highlighting;

namespace ReSharperPlugin.GitIntegration.Git;

[DaemonStage]
public class GitDaemon : IDaemonStage
{
    private string _commitMessage = string.Empty;
    public GitDaemon()
    {
        MessageBus.Subscribe<string>(OnCommitMessageReceived);
    }
    
    private void OnCommitMessageReceived(string commitMessage)
    {
        _commitMessage = commitMessage;
    }
    
    public IEnumerable<IDaemonStageProcess> CreateProcess(IDaemonProcess process,
        IContextBoundSettingsStore settings, DaemonProcessKind processKind)
    {
        
        return new List<IDaemonStageProcess>
        {
            new GitHighlightingDaemonStageProcess(process, _commitMessage) // Pass the commits here
        };
    }
}

public class GitHighlightingDaemonStageProcess(IDaemonProcess daemonProcess, string commitMessage)
    : IDaemonStageProcess
{
    public IDaemonProcess DaemonProcess { get; } = daemonProcess;

    public void Execute(Action<DaemonStageResult> committer)
    {
        var document = DaemonProcess.Document;
        var documentPath = DaemonProcess.SourceFile;
        var text = document.GetText();
        var highlights = new List<HighlightingInfo>();
        Console.WriteLine(commitMessage);
        var whitespaceCount = 0;
        var startIndex = -1;

        for (var i = 0; i < text.Length; i++)
        {
            if (char.IsWhiteSpace(text[i]))
            {
                if (whitespaceCount == 0)
                {
                    startIndex = i;
                }

                whitespaceCount++;

                if (whitespaceCount < 5) continue; // Continue if less than 5 whitespaces
                
                var range = new DocumentRange(document, new TextRange(startIndex, i + 1));
                highlights.Add(new HighlightingInfo(range, new GitHighlighting(range, commitMessage)));
                break; // Stop after highlighting the first 5 contiguous whitespaces
            }
            else
            {
                whitespaceCount = 0; // Reset count if not whitespace
                startIndex = -1;
            }
        }

        // Optionally use _recentCommits here for further logic
        // Example: Console.WriteLine("Recent commits count: " + _recentCommits.Count);

        if (highlights.Any())
        {
            committer(new DaemonStageResult(highlights)); // Commit the highlights
        }
    }
}