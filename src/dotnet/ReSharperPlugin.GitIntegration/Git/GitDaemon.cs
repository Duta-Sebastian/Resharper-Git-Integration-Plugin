using System;
using System.Collections.Generic;
using JetBrains.Application.Settings;
using JetBrains.DocumentManagers;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.Util;
using ReSharperPlugin.GitIntegration.Highlighting;

namespace ReSharperPlugin.GitIntegration.Git;

[DaemonStage]
public class GitDaemon : IDaemonStage
{
    private Dictionary<string, string> _commitMessageDict;

    public GitDaemon()
    {
        GitMessageBus.Subscribe<Dictionary<string, string>>(OnCommitMessageReceived);
    }

    public IEnumerable<IDaemonStageProcess> CreateProcess(IDaemonProcess process,
        IContextBoundSettingsStore settings, DaemonProcessKind processKind)
    {
        return new List<IDaemonStageProcess>
        {
            new GitHighlightingDaemonStageProcess(process, _commitMessageDict)
        };
    }

    private void OnCommitMessageReceived(Dictionary<string, string> commitMessageDict)
    {
        _commitMessageDict = commitMessageDict;
    }
}

public class GitHighlightingDaemonStageProcess(
    IDaemonProcess daemonProcess,
    Dictionary<string, string> commitMessageDict)
    : IDaemonStageProcess
{
    public IDaemonProcess DaemonProcess { get; } = daemonProcess;

    public void Execute(Action<DaemonStageResult> committer)
    {
        var document = DaemonProcess.Document;
        var documentPath = DaemonProcess.Document.TryGetFilePath();
        var text = document.GetText();
        var highlights = new List<HighlightingInfo>();
        if (commitMessageDict.TryGetValue(documentPath.FullPath, out var commitMessages))
        {
            var nonWhitespaceCount = 0;
            var startIndex = -1;

            for (var i = 0; i < text.Length; i++)
                if (!char.IsWhiteSpace(text[i]))
                {
                    if (nonWhitespaceCount == 0) startIndex = i;

                    nonWhitespaceCount++;

                    if (nonWhitespaceCount < 5) continue;

                    var range = new DocumentRange(document, new TextRange(startIndex, i + 1));
                    highlights.Add(new HighlightingInfo(range, new GitHighlighting(range, commitMessages)));
                    break;
                }
                else
                {
                    nonWhitespaceCount = 0;
                    startIndex = -1;
                }
        }

        if (highlights.Any()) committer(new DaemonStageResult(highlights));
    }
}