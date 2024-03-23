﻿using Bifrost.Models.Portals;
using Bifrost.Features.PortalDefinitions.Services;
using Bifrost.Commands.Portals;

namespace Bifrost.UnitTests.Features.PortalDefinitions.Services;

[TestClass]
public class PortalDefinitionServiceTests
{
    readonly CreatePortalCommand ValidCreateCommand = new("UnitTest-Portal", 1, nameof(VpnTypes.OpenVPN), "Some config");

    readonly CreatePortalCommand EmptyCreateCommand = new(string.Empty, 0, string.Empty, string.Empty);

    readonly UpdatePortalCommand ValidUpdateCommand = new("UnitTest-Portal", 1, nameof(VpnTypes.OpenVPN), "Some config");

    readonly UpdatePortalCommand EmptyUpdateCommand = new(string.Empty, 0, string.Empty, string.Empty);

    [TestMethod]
    public async Task CreatePortalAsync_Fails_WhenValidationFails()
    {
        // Arrange
        (var repoMock, var service) = PrepareServiceWithValidationFault();

        // Act
        var result = await service.CreatePortalAsync(EmptyCreateCommand, "Dummy-User");

        // Assert
        repoMock.Verify(x => x.CreateAsync(It.IsAny<PortalDefinition>(), It.IsAny<bool>()), Times.Never);
        repoMock.VerifyNoOtherCalls();
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.ErrorDetails.Should().NotBeNullOrEmpty();
    }

    private static (Mock<IPortalDefinitionRepository>, PortalDefinitionService) PrepareServiceWithValidationFault()
    {
        Mock<IPortalDefinitionRepository> repoMock = new();
        repoMock.Setup(x => x.CreateAsync(It.IsAny<PortalDefinition>(), It.IsAny<bool>())).Verifiable();
        repoMock.Setup(x => x.UpdateAsync(It.IsAny<PortalDefinition>(), It.IsAny<bool>())).Verifiable();
        PortalDefinitionService service = new(repoMock.Object);
        return (repoMock, service);
    }

    [TestMethod]
    public async Task CreatePortalAsync_CallsCreateAsync_OnValidationSuccess()
    {
        // Arrange
        PortalDefinition? resultPortalDefinition = null;
        Mock<IPortalDefinitionRepository> repoMock = new();
        repoMock.Setup(x => x.CreateAsync(It.IsAny<PortalDefinition>(), false))
            .Returns(Task.CompletedTask)
            .Callback((PortalDefinition portal, bool _) => resultPortalDefinition = portal)
            .Verifiable();
        PortalDefinitionService service = new(repoMock.Object);

        // Act
        var result = await service.CreatePortalAsync(ValidCreateCommand, "Dummy-User");

        // Assert
        repoMock.Verify(x => x.CreateAsync(It.IsAny<PortalDefinition>(), false), Times.Once);
        repoMock.VerifyNoOtherCalls();
        result.IsSuccess.Should().BeTrue();
        result.ErrorDetails.Should().BeNullOrEmpty();
        result.Portal.Should().Be(resultPortalDefinition);
        resultPortalDefinition.Should().NotBeNull();
        resultPortalDefinition!.CreationUser.Should().Be("Dummy-User");
        resultPortalDefinition!.Name.Should().Be(ValidUpdateCommand.Name);
        resultPortalDefinition!.MaxInstanceCount.Should().Be(ValidUpdateCommand.MaxInstanceCount);
        resultPortalDefinition!.VpnType.Should().Be(ValidUpdateCommand.VpnType);
        resultPortalDefinition!.VpnConfig.Should().Be(ValidUpdateCommand.VpnConfig);
    }

    [DataTestMethod]
    [DataRow("")]
    [DataRow("   ")]
    [DataRow(null)]
    public async Task UpdatePortalAsync_ThrowsArgumentException_ForInvalidId(string id)
    {
        // Arrange
        PortalDefinitionService service = new(Mock.Of<IPortalDefinitionRepository>());

        // Act
        Func<Task> act = async () => await service.UpdatePortalAsync(id, ValidUpdateCommand);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>().WithParameterName("id");
    }

    [TestMethod]
    public async Task UpdatePortalAsync_Fails_WhenValidationFails()
    {
        // Arrange
        (var repoMock, var service) = PrepareServiceWithValidationFault();

        // Act
        var result = await service.UpdatePortalAsync("some-id", EmptyUpdateCommand);

        // Assert
        repoMock.Verify(x => x.UpdateAsync(It.IsAny<PortalDefinition>(), It.IsAny<bool>()), Times.Never);
        repoMock.VerifyNoOtherCalls();
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.ErrorDetails.Should().NotBeNullOrEmpty();
    }

    [TestMethod]
    public async Task UpdatePortalAsync_Fails_ForUnknownId()
    {
        // Arrange
        Mock<IPortalDefinitionRepository> repoMock = new();
        repoMock.Setup(x => x.GetByIdAsync(It.IsAny<string>()))
            .Returns(Task.FromResult((PortalDefinition?)null))
            .Verifiable();
        PortalDefinitionService service = new(repoMock.Object);

        // Act
        var result = await service.UpdatePortalAsync("invalid-id", ValidUpdateCommand);

        // Assert
        repoMock.Verify(x => x.GetByIdAsync("invalid-id"), Times.Once);
        repoMock.Verify(x => x.UpdateAsync(It.IsAny<PortalDefinition>(), It.IsAny<bool>()), Times.Never);
        repoMock.VerifyNoOtherCalls();
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.ErrorDetails.Should().NotBeNullOrEmpty();
        result.ErrorDetails!.Should().ContainSingle(f => f.Value.Contains("Portal not found"));
    }

    [TestMethod]
    public async Task UpdatePortalAsync_CallsUpdateAsync_OnValidationSuccess()
    {
        // Arrange
        PortalDefinition? resultPortalDefinition = null;
        PortalDefinition emptyPortalDefinition = new()
        {
            CreationDate = DateTime.Now,
            CreationUser = "Me",
            Id = "some-id",
            Name = "",
            MaxInstanceCount = 0,
            VpnType = "",
            VpnConfig = ""
        };
        Mock<IPortalDefinitionRepository> repoMock = new();
        repoMock.Setup(x => x.GetByIdAsync(It.IsAny<string>()))
            .Returns(Task.FromResult((PortalDefinition?)emptyPortalDefinition))
            .Verifiable();
        repoMock.Setup(x => x.UpdateAsync(It.IsAny<PortalDefinition>(), false))
            .Returns(Task.CompletedTask)
            .Callback((PortalDefinition portal, bool _) => resultPortalDefinition = portal)
            .Verifiable();
        PortalDefinitionService service = new(repoMock.Object);

        // Act
        var result = await service.UpdatePortalAsync("some-id", ValidUpdateCommand);

        // Assert
        repoMock.Verify(x => x.GetByIdAsync("some-id"), Times.Once);
        repoMock.Verify(x => x.UpdateAsync(It.IsAny<PortalDefinition>(), false), Times.Once);
        repoMock.VerifyNoOtherCalls();
        result.IsSuccess.Should().BeTrue();
        result.ErrorDetails.Should().BeNullOrEmpty();
        resultPortalDefinition.Should().NotBeNull();
        resultPortalDefinition!.Name.Should().Be(ValidUpdateCommand.Name);
        resultPortalDefinition!.MaxInstanceCount.Should().Be(ValidUpdateCommand.MaxInstanceCount);
        resultPortalDefinition!.VpnType.Should().Be(ValidUpdateCommand.VpnType);
        resultPortalDefinition!.VpnConfig.Should().Be(ValidUpdateCommand.VpnConfig);
    }

    [DataTestMethod]
    [DataRow("")]
    [DataRow("   ")]
    [DataRow(null)]
    public async Task GetPortalAsync_ThrowsArgumentException_ForInvalidId(string id)
    {
        // Arrange
        PortalDefinitionService service = new(Mock.Of<IPortalDefinitionRepository>());

        // Act
        Func<Task> act = async () => await service.GetPortalAsync(id);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>().WithParameterName("id");
    }

    [TestMethod]
    public async Task GetPortalAsync_ReturnsPortalDefinition_ForId()
    {
        // Arrange
        PortalDefinition portalDefinition = new()
        {
            CreationDate = DateTime.Now,
            CreationUser = "Me",
            Name = "name",
            MaxInstanceCount = 1,
            VpnType = "Any",
            VpnConfig = ""
        };
        Mock<IPortalDefinitionRepository> repoMock = new();
        repoMock.Setup(x => x.GetByIdAsync(It.IsAny<string>()))
            .Returns(Task.FromResult((PortalDefinition?)portalDefinition))
            .Verifiable();
        PortalDefinitionService service = new(repoMock.Object);

        // Act
        var result = await service.GetPortalAsync("some-id");

        // Assert
        repoMock.Verify(x => x.GetByIdAsync("some-id"), Times.Once);
        repoMock.VerifyNoOtherCalls();
        result.Should().NotBeNull();
        result.Should().Be(portalDefinition);
    }

    [DataTestMethod]
    [DataRow("")]
    [DataRow("   ")]
    [DataRow(null)]
    public async Task DeletePortalAsync_ThrowsArgumentException_ForInvalidId(string id)
    {
        // Arrange
        PortalDefinitionService service = new(Mock.Of<IPortalDefinitionRepository>());

        // Act
        Func<Task> act = async () => await service.DeletePortalAsync(id);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>().WithParameterName("id");
    }

    [TestMethod]
    public async Task DeletePortalAsync_CallsDeleteAsync_ForId()
    {
        // Arrange
        Mock<IPortalDefinitionRepository> repoMock = new();
        repoMock.Setup(x => x.DeleteAsync(It.IsAny<string>(), It.IsAny<bool>()))
            .Returns(Task.CompletedTask)
            .Verifiable();
        PortalDefinitionService service = new(repoMock.Object);

        // Act
        await service.DeletePortalAsync("some-id");

        // Assert
        repoMock.Verify(x => x.DeleteAsync("some-id", false), Times.Once);
        repoMock.VerifyNoOtherCalls();
    }
}
