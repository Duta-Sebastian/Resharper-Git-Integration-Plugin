using NUnit.Framework;
using ReSharperPlugin.GitIntegration;
namespace ReSharperPlugin.GitIntegration.Tests;

[TestFixture]
public class GitMessageBusTest
{
    /// Test: Subscribing to a message type and receiving it when published
    [Test]
    public void PublishShouldInvokeSubscriberAction()
    {
        // Arrange
        const string testMessage = "Hello, World!";
        var messageReceived = string.Empty;

        // Act
        GitMessageBus.Subscribe<string>(message => messageReceived = message);
        GitMessageBus.Publish(testMessage);

        // Assert
        Assert.AreEqual(testMessage, messageReceived);
    }
    
    /// Test: Publishing a message with no subscribers should not throw an exception
    [Test]
    public void Publish_ShouldNotThrowWhenNoSubscribers()
    {
        // Act & Assert
        Assert.DoesNotThrow(() => GitMessageBus.Publish("NoSubscribersMessage"));
    }
}