using System;
using System.IO;

namespace ReSharperPlugin.GitIntegration.Git;

public class GitChangesWatcher
{
    private readonly FileSystemWatcher _fileSystemWatcher;

    public GitChangesWatcher(string gitDirectory)
    {
        _fileSystemWatcher = new FileSystemWatcher(gitDirectory)
        {
            Filter = "*.*",
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.DirectoryName
        };

        _fileSystemWatcher.Changed += OnChanged;
        _fileSystemWatcher.Created += OnChanged;
        _fileSystemWatcher.Deleted += OnChanged;
        _fileSystemWatcher.EnableRaisingEvents = true;
    }

    public event EventHandler ChangesDetected;

    private void OnChanged(object sender, FileSystemEventArgs e)
    {
        if (e.FullPath.EndsWith("HEAD") || e.FullPath.Contains("refs"))
            ChangesDetected?.Invoke(this, EventArgs.Empty);
    }
}