using System.Threading;
using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.ReSharper.Feature.Services;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using JetBrains.TestFramework;
using JetBrains.TestFramework.Application.Zones;
using NUnit.Framework;

[assembly: Apartment(ApartmentState.STA)]

namespace ReSharperPlugin.GitIntegration.Tests;

[ZoneDefinition]
public class GitIntegrationTestEnvironmentZone : ITestsEnvZone, IRequire<PsiFeatureTestZone>,
    IRequire<IGitIntegrationZone>
{
}

[ZoneMarker]
public class ZoneMarker : IRequire<ICodeEditingZone>, IRequire<ILanguageCSharpZone>,
    IRequire<GitIntegrationTestEnvironmentZone>
{
}

[SetUpFixture]
public class GitIntegrationTestsAssembly : ExtensionTestEnvironmentAssembly<GitIntegrationTestEnvironmentZone>
{
}