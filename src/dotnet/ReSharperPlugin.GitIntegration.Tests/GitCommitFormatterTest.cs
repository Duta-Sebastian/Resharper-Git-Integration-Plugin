using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using ReSharperPlugin.GitIntegration.Git;

namespace ReSharperPlugin.GitIntegration.Tests;

[TestFixture]
public class GitCommitFormatterTest
{

    private const string TestStringInput = """
                                      <COMMIT>995fbbc 
                                      Message: Fixed a critical bug in user login process
                                      Test/Program.cs
                                      
                                      <COMMIT>29f64d1
                                      Message: Updated README with installation instructions
                                      Test/Class1.cs
                                      
                                      <COMMIT>41c8ba6
                                      Message: Implemented new feature for data export
                                      Test/DataExporter.cs
                                      
                                      <COMMIT>8792a6c
                                      Message: Improved performance of data processing algorithm
                                      Test/Processor.cs
                                      
                                      <COMMIT>ee75118
                                      Message: Refactored code to enhance readability and maintainability
                                      Test/Main.cs
                                      """;
    private static readonly Dictionary<string, string> TestDictionaryOutput = new()
    {
        { @"TestPath:\File\Test\Program.cs", "995fbbc - Fixed a critical bug in user login process" },
        { @"TestPath:\File\Test\Class1.cs", "29f64d1 - Updated README with installation instructions" },
        { @"TestPath:\File\Test\DataExporter.cs", "41c8ba6 - Implemented new feature for data export" },
        { @"TestPath:\File\Test\Processor.cs", "8792a6c - Improved performance of data processing algorithm" },
        { @"TestPath:\File\Test\Main.cs", "ee75118 - Refactored code to enhance readability and maintainability" }
    };
    /// Test : Formatting the git commit messages in the proper format for the daemon
        [Test]
    public void FormatCommit()
    {
        //Arrange
        var gitCommitFormatter = new GitCommitFormatter(TestStringInput,"TestPath:\\File"); 
        
        //Act
        var outputDictionary = gitCommitFormatter.FormatCommitMessage();
        
        //Assert
        CollectionAssert.AreEquivalent(TestDictionaryOutput.ToList(), outputDictionary.ToList(),
            "Dictionaries do not match.");
    }
}