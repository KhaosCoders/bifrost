﻿using Bifrost.Features.Portals.Model;

namespace Bifrost.UnitTests.Features.Portals.Model;
internal class PortalInstanceTests
{
    private PortalDefinition portal;

    [SetUp]
    public void Setup()
    {
        portal = new()
        {
            Name = "UnitTests",
            CreationDate = DateTime.Now,
            CreationUser = "me",
            VpnType = "Any",
            VpnConfig = ""
        };
    }

    [Test]
    public void PortalInstance_CurrentState_ShouldReturn_Null_WhenNoHistory()
    {
        // Arrange
        PortalInstance instance = new()
        {
            Portal = portal,
            PortalId = portal.Id,
            History = new List<PortalHistory>()
        };

        // Act
        var state = instance.CurrentState;

        // Assert
        state.Should().BeNull();
    }

    [TestCase(PortalState.Pending)]
    [TestCase(PortalState.Creating)]
    [TestCase(PortalState.Open)]
    [TestCase(PortalState.Closing)]
    [TestCase(PortalState.Closed)]
    public void PortalInstance_CurrentState_ShouldReturn_LatestHistoryState(PortalState highestState)
    {
        // Arrange
        PortalInstance instance = new()
        {
            Portal = portal,
            PortalId = portal.Id,
            History = new List<PortalHistory>()
        };

        PortalState[] states = [
            PortalState.Pending,
            PortalState.Creating,
            PortalState.Open,
            PortalState.Closing,
            PortalState.Closed
        ];

        foreach (var x in states)
        {
            if (highestState >= x)
                instance.History.Add(CreateHistory(instance, x));
        }

        // Act
        var state = instance.CurrentState;

        // Assert
        state.Should().NotBeNull();
        state.Should().Be(highestState);
    }

    [Test]
    public void PortalInstance_CreationUser_ShouldReturn_FirstHistoryUser()
    {
        // Arrange
        PortalInstance instance = new()
        {
            Portal = portal,
            PortalId = portal.Id,
        };
        instance.History =
        [
            CreateHistory(instance, PortalState.Pending, b => b.CreationUser = "User1"),
            CreateHistory(instance, PortalState.Closing, b => b.CreationUser = "User2"),
        ];

        // Act
        var user = instance.CreationUser;

        // Assert
        user.Should().NotBeNull();
        user.Should().Be("User1");
    }

    [Test]
    public void PortalInstance_CreationUser_ShouldReturn_Null_WhenNoHistory()
    {
        // Arrange
        PortalInstance instance = new()
        {
            Portal = portal,
            PortalId = portal.Id,
        };

        // Act
        var user = instance.CreationUser;

        // Assert
        user.Should().BeNull();
    }

    [Test]
    public void PortalInstance_CreationDate_ShouldReturn_FirstHistoryDate()
    {
        // Arrange
        var creationDate = DateTime.Now.AddDays(-1);
        PortalInstance instance = new()
        {
            Portal = portal,
            PortalId = portal.Id,
        };
        instance.History =
        [
            CreateHistory(instance, PortalState.Pending, b => b.CreationDate = creationDate),
            CreateHistory(instance, PortalState.Closing, b => b.CreationDate = DateTime.Now),
        ];

        // Act
        var date = instance.CreationDate;

        // Assert
        date.Should().NotBeNull();
        date.Should().Be(creationDate);
    }

    [Test]
    public void PortalInstance_CreationDate_ShouldReturn_Null_WhenNoHistory()
    {
        // Arrange
        PortalInstance instance = new()
        {
            Portal = portal,
            PortalId = portal.Id,
        };

        // Act
        var date = instance.CreationDate;

        // Assert
        date.Should().BeNull();
    }

    private static PortalHistory CreateHistory(PortalInstance instance, PortalState state, Action<PortalHistory>? builder = default)
    {
        PortalHistory history = new()
        {
            Instance = instance,
            InstanceId = instance.Id,
            CreationDate = DateTime.Now,
            CreationUser = "Me",
            State = state
        };

        builder?.Invoke(history);

        return history;
    }
}
