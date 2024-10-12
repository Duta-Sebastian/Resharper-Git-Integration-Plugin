using JetBrains.Application.Settings;
using JetBrains.Application.Settings.WellKnownRootKeys;

namespace ReSharperPlugin.GitIntegration.SettingsPage;

[SettingsKey(
    typeof(EnvironmentSettings),
    "Git Commits Plugin")]
public class GitPluginSettings
{
    [SettingsEntry(5, "The number of commits highlighted in the project")]
    public int CommitCounter;
}