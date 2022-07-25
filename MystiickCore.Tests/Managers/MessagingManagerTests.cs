using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;

using MystiickCore.Managers;
using MystiickCore.Models;

namespace MystiickCore.Tests.Managers;

[TestClass]
public class MessagingManagerTests
{

    #region | Subscribe |
    /// <summary>
    /// You can add multiple subscribers to the same event type, and both handlers will be called
    /// </summary>
    [TestMethod]
    public void MessagingManager_Subscribe_MultipleEqualEventTypes()
    {
        // Arrange
        var manager = new MessagingManager();
        Subscription[] output;

        // Act
        output = new Subscription[] {
            manager.Subscribe(EventType.Test, (sender, args) => { }, Guid.NewGuid()),
            manager.Subscribe(EventType.Test, (sender, args) => { }, Guid.NewGuid())
        };

        // Assert
        Assert.AreNotEqual(output[0].ID, output[1].ID, "Distinct GUIDs should be assigned for each subscription");
    }

    /// <summary>
    /// You can add event types of UI, Input...
    /// </summary>
    [TestMethod]
    public void MessagingManager_Subscribe_MultipleDifferentEventTypes()
    {
        // Arrange
        var manager = new MessagingManager();
        Subscription[] output;

        // Act
        output = new Subscription[] {
            manager.Subscribe(EventType.Test, (sender, args) => { }, Guid.NewGuid()),
            manager.Subscribe(EventType.UserInterface, (sender, args) => { }, Guid.NewGuid())
        };

        // Assert
        Assert.AreNotEqual(output[0].ID, output[1].ID, "Distinct GUIDs should be assigned for each subscription");
        Assert.AreNotEqual(output[0].Event, output[1].Event, "Different event types should persist");
    }
    #endregion

    #region | Unsubscribe |
    [TestMethod]
    public void MessagingManager_Unsubscribe_Pass()
    {
        // Arrange
        var manager = new MessagingManager();
        Subscription[] subscriptions;
        bool output;

        subscriptions = new Subscription[] {
            manager.Subscribe(EventType.Test, (sender, args) => { }, Guid.NewGuid()),
            manager.Subscribe(EventType.Test, (sender, args) => { }, Guid.NewGuid())
        };

        // Act
        output = manager.Unsubscribe(subscriptions[0].ID);

        // Assert
        Assert.IsTrue(output, "Unsubscribe should return True if an ID is removed");
        Assert.AreEqual(1, subscriptions.Length - 1, "Total count should be one less than how many were input");
    }

    [TestMethod]
    public void MessagingManager_Unsubscribe_ReturnsFalseOnNoSubscriptionsFound()
    {
        // Arrange
        var manager = new MessagingManager();
        bool output;

        // Act
        output = manager.Unsubscribe(Guid.NewGuid());

        // Assert
        Assert.IsFalse(output, "Unsubscribe should return False if nothing was unsubscribed");
        Assert.AreEqual(0, manager.TotalSubscriptions, "Total count should be zero since nothing was ever subscribed");
    }

    [TestMethod]
    public void MessagingManager_Unsubscribe_FailOnThrowTrue()
    {
        // Arrange
        var manager = new MessagingManager();
        ArgumentException output = null;

        // Act
        try
        {
            manager.Unsubscribe(Guid.NewGuid(), true);
        }
        catch (ArgumentException ex)
        {
            output = ex;
        }

        // Assert
        Assert.IsNotNull(output);
    }

    [TestMethod]
    public void MessagingManager_Unsubscribe_PassOnThrowFalse()
    {
        // Arrange
        var manager = new MessagingManager();
        bool output;

        // Act
        output = manager.Unsubscribe(Guid.NewGuid(), false);

        // Assert
        Assert.IsFalse(output, "Unsubscribe should return False if nothing was unsubscribed");
        Assert.AreEqual(0, manager.TotalSubscriptions, "Total count should be zero since nothing was ever subscribed");
    }
    #endregion

    #region | SendMessage |
    /// <summary>
    /// You can call SendMessage when there are 0 total subscribers, and 0 subscribers for your event
    /// </summary>
    [TestMethod]
    public void MessagingManager_SendMessage_NoSubscibers()
    {
        // Arrange
        var manager = new MessagingManager();

        // Act
        manager.SendMessage(EventType.Test, null, this, "Pass");

        // Assert
        // Nothing to assert, the fact that SendMessage didn't throw an exception is enough
    }

    /// <summary>
    /// You can call SendMessage when there are subscribers, and are handled
    /// </summary>
    [TestMethod]
    public void MessagingManager_SendMessage_Pass()
    {
        // Arrange
        var manager = new MessagingManager();
        string output = "Fail";

        manager.Subscribe(EventType.Test, (sender, args) => { output = args.ToString(); }, Guid.NewGuid());

        // Act
        manager.SendMessage(EventType.Test, null, this, "Pass");

        // Assert
        Assert.AreEqual("Pass", output, "The subscription should properly update Output");
    }

    /// <summary>
    /// You can call SendMessage when there are subscribers, and only your Arguments are called
    /// </summary>
    [TestMethod]
    public void MessagingManager_SendMessage_FilteredArguments()
    {
        // Arrange
        var manager = new MessagingManager();
        string output = "Pass";

        manager.Subscribe(EventType.Test, "UnitTest_One", (sender, args) => { output = args.ToString(); }, Guid.NewGuid());
        manager.Subscribe(EventType.UserInterface, (sender, args) => { output = args.ToString(); }, Guid.NewGuid());

        // Act
        manager.SendMessage(EventType.Test, null, this, "Fail");

        // Assert
        Assert.AreEqual("Pass", output, "The subscription should properly update Output");
    }

    /// <summary>
    /// You can call SendMessage when there are subscribers, and only your Arguments are called
    /// </summary>
    [TestMethod]
    public void MessagingManager_SendMessage_WithArguments()
    {
        // Arrange
        var manager = new MessagingManager();
        string output = "Fail";
        const string argument = "UnitTest_One";
        const string argument2 = "UnitTest_Two";

        manager.Subscribe(EventType.Test, argument, (sender, args) => { output = args.ToString(); }, Guid.NewGuid());
        manager.Subscribe(EventType.Test, argument2, (sender, args) => { throw new Exception("I should never be called"); }, Guid.NewGuid());
        manager.Subscribe(EventType.UserInterface, argument, (sender, args) => { throw new Exception("I should never be called"); }, Guid.NewGuid());

        // Act
        manager.SendMessage(EventType.Test, argument, this, "Pass");

        // Assert
        Assert.AreEqual("Pass", output, "The subscription should properly update Output");
    }

    /// <summary>
    /// You can call SendMessage when there are subscribers, and only your Arguments are called
    /// </summary>
    [TestMethod]
    public void MessagingManager_SendMessage_WithArguments_AlsoCallsNull()
    {
        // Arrange
        var manager = new MessagingManager();
        int output = 0;
        const string argument = "UnitTest_One";
        const string argument2 = "UnitTest_Two";

        manager.Subscribe(EventType.Test, argument, (sender, args) => { output++; }, Guid.NewGuid());
        manager.Subscribe(EventType.Test, (sender, args) => { output++; }, Guid.NewGuid());
        manager.Subscribe(EventType.Test, argument2, (sender, args) => { throw new Exception("I should never be called"); }, Guid.NewGuid());
        manager.Subscribe(EventType.UserInterface, argument, (sender, args) => { throw new Exception("I should never be called"); }, Guid.NewGuid());

        // Act
        manager.SendMessage(EventType.Test, argument, this, null);

        // Assert
        Assert.AreEqual(2, output, "The subscription should properly update output twice");
    }
    #endregion

    #region | UnsubscribeParent |
    /// <summary>
    /// 
    /// </summary>
    [TestMethod]
    public void MessagingManager_UnsubscribeParent_Pass()
    {
        // Arrange
        var manager = new MessagingManager();
        const string argument = "UnitTest_One";
        Guid parent = Guid.NewGuid();
        bool output;

        manager.Subscribe(EventType.Test, (sender, args) => { }, parent);
        manager.Subscribe(EventType.Test, argument, (sender, args) => { }, parent);
        manager.Subscribe(EventType.Test, (sender, args) => { }, Guid.NewGuid());

        // Act
        output = manager.UnsubscribeParent(parent);

        // Assert
        Assert.AreEqual(1, manager.TotalSubscriptions, "Only one subscription should remain after unsubscribing the parent");
        Assert.IsTrue(output, "The manager should return True since there was at least one subscription removed");
    }

    /// <summary>
    /// 
    /// </summary>
    [TestMethod]
    public void MessagingManager_UnsubscribeParent_NoMatch_Pass()
    {
        // Arrange
        var manager = new MessagingManager();
        Guid parent = Guid.NewGuid();
        bool output;

        // Act
        output = manager.UnsubscribeParent(parent);

        // Assert
        Assert.AreEqual(0, manager.TotalSubscriptions, "The manager should have not added any subscriptions");
        Assert.IsFalse(output, "The manager should return False since there should be no subscriptions removed");
    }
    #endregion
}
