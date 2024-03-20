using Bifrost.Models.Portals;
using Bifrost.Utils.Validation;
using Bifrost.Features.PortalDefinitions.Services;
using Bifrost.Commands.Portals;

namespace Bifrost.UnitTests.Features.PortalDefinitions.Services;

[TestClass]
public class PortalDefinitionServiceTests
{
    readonly CreatePortalCommand ValidCreateCommand = new("UnitTest-Portal", 1, "utVPN", "Some config");

    readonly CreatePortalCommand EmptyCreateCommand = new(string.Empty, 0, string.Empty, string.Empty);

    readonly UpdatePortalCommand ValidUpdateCommand = new("UnitTest-Portal", 1, "utVPN", "Some config");

    readonly UpdatePortalCommand EmptyUpdateCommand = new(string.Empty, 0, string.Empty, string.Empty);

    [TestMethod]
    public void ValidateRequest_ReturnsTrue_ForValidRequest()
    {
        // Arrange
        PortalDefinitionService service = new(Mock.Of<IPortalDefinitionRepository>());

        // Act
        var result = ((IRequestValidator<PortalCommandBase>)service).ValidateRequest(ValidUpdateCommand);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
        result.Faults.Should().BeEmpty();
    }

    [DataTestMethod]
    [DataRow("UnitTests", 1, "Any", null)]
    [DataRow("UnitTests", 1, null, "Config")]
    [DataRow("UnitTests", 0, "Any", "Config")]
    [DataRow(null, 1, "Any", "Config")]
    public void ValidateRequest_ReturnsFailures_ForInvalidRequest(string? name, int instanceCount, string? vpnType, string? vpnConfig)
    {
        // Arrange
        PortalDefinitionService service = new(Mock.Of<IPortalDefinitionRepository>());
        CreatePortalCommand request = new(name!, instanceCount, vpnType!, vpnConfig!);

        // Act
        var result = ((IRequestValidator<PortalCommandBase>)service).ValidateRequest(request);

        // Assert
        result.Should().NotBeNull();
        result.IsValid!.Should().BeFalse();
        result.Faults.Should().NotBeEmpty();
    }

    [TestMethod]
    public async Task CreatePortalAsync_Fails_WhenValidationFails()
    {
        // Arrange
        (var fault, var validatorMock, var repoMock, var service) = PrepareServiceWithValidationFault();

        // Act
        var result = await service.CreatePortalAsync(EmptyCreateCommand, "Dummy-User");

        // Assert
        validatorMock.Verify(x => x.ValidateRequest(EmptyCreateCommand), Times.Once);
        repoMock.Verify(x => x.CreateAsync(It.IsAny<PortalDefinition>(), It.IsAny<bool>()), Times.Never);
        repoMock.VerifyNoOtherCalls();
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.ErrorDetails.Should().NotBeNullOrEmpty();
        result.ErrorDetails!.First().Key.Should().BeEquivalentTo(fault.Property);
        result.ErrorDetails!.First().Value.Should().BeEquivalentTo(fault.Message);
    }

    private static (ValidationFault, Mock<IRequestValidator<PortalCommandBase>>, Mock<IPortalDefinitionRepository>, PortalDefinitionService) PrepareServiceWithValidationFault()
    {
        ValidationFault fault = new("UnitTest", "failing?");
        Bifrost.Utils.Validation.ValidationResult validationResult = new()
        {
            IsValid = false,
            Faults = [fault]
        };
        Mock<IRequestValidator<PortalCommandBase>> validatorMock = new();
        validatorMock.Setup(x => x.ValidateRequest(It.IsAny<PortalCommandBase>()))
            .Returns(validationResult)
            .Verifiable();
        Mock<IPortalDefinitionRepository> repoMock = new();
        repoMock.Setup(x => x.CreateAsync(It.IsAny<PortalDefinition>(), It.IsAny<bool>())).Verifiable();
        repoMock.Setup(x => x.UpdateAsync(It.IsAny<PortalDefinition>(), It.IsAny<bool>())).Verifiable();
        PortalDefinitionService service = new(repoMock.Object)
        {
            Validator = validatorMock.Object
        };
        return (fault, validatorMock, repoMock, service);
    }

    [TestMethod]
    public async Task CreatePortalAsync_CallsCreateAsync_OnValidationSuccess()
    {
        // Arrange
        PortalDefinition? resultPortalDefinition = null;
        Mock<IRequestValidator<PortalCommandBase>> validatorMock = new();
        validatorMock.Setup(x => x.ValidateRequest(It.IsAny<CreatePortalCommand>()))
            .Returns(new Bifrost.Utils.Validation.ValidationResult() { IsValid = true, Faults = [] })
            .Verifiable();
        Mock<IPortalDefinitionRepository> repoMock = new();
        repoMock.Setup(x => x.CreateAsync(It.IsAny<PortalDefinition>(), false))
            .Returns(Task.CompletedTask)
            .Callback((PortalDefinition portal, bool _) => resultPortalDefinition = portal)
            .Verifiable();
        PortalDefinitionService service = new(repoMock.Object)
        {
            Validator = validatorMock.Object
        };

        // Act
        var result = await service.CreatePortalAsync(ValidCreateCommand, "Dummy-User");

        // Assert
        validatorMock.Verify(x => x.ValidateRequest(ValidCreateCommand), Times.Once);
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
        (var fault, var validatorMock, var repoMock, var service) = PrepareServiceWithValidationFault();

        // Act
        var result = await service.UpdatePortalAsync("some-id", EmptyUpdateCommand);

        // Assert
        validatorMock.Verify(x => x.ValidateRequest(EmptyUpdateCommand), Times.Once);
        repoMock.Verify(x => x.UpdateAsync(It.IsAny<PortalDefinition>(), It.IsAny<bool>()), Times.Never);
        repoMock.VerifyNoOtherCalls();
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.ErrorDetails.Should().NotBeNullOrEmpty();
        result.ErrorDetails!.First().Key.Should().BeEquivalentTo(fault.Property);
        result.ErrorDetails!.First().Value.Should().BeEquivalentTo(fault.Message);
    }

    [TestMethod]
    public async Task UpdatePortalAsync_Fails_ForUnknownId()
    {
        // Arrange
        Mock<IRequestValidator<PortalCommandBase>> validatorMock = new();
        validatorMock.Setup(x => x.ValidateRequest(It.IsAny<PortalCommandBase>()))
            .Returns(new Bifrost.Utils.Validation.ValidationResult() { IsValid = true, Faults = [] })
            .Verifiable();
        Mock<IPortalDefinitionRepository> repoMock = new();
        repoMock.Setup(x => x.GetByIdAsync(It.IsAny<string>()))
            .Returns(Task.FromResult((PortalDefinition?)null))
            .Verifiable();
        PortalDefinitionService service = new(repoMock.Object)
        {
            Validator = validatorMock.Object
        };

        // Act
        var result = await service.UpdatePortalAsync("invalid-id", ValidUpdateCommand);

        // Assert
        validatorMock.Verify(x => x.ValidateRequest(ValidUpdateCommand), Times.Once);
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
        Mock<IRequestValidator<PortalCommandBase>> validatorMock = new();
        validatorMock.Setup(x => x.ValidateRequest(It.IsAny<PortalCommandBase>()))
            .Returns(new Bifrost.Utils.Validation.ValidationResult() { IsValid = true, Faults = [] })
            .Verifiable();
        Mock<IPortalDefinitionRepository> repoMock = new();
        repoMock.Setup(x => x.GetByIdAsync(It.IsAny<string>()))
            .Returns(Task.FromResult((PortalDefinition?)emptyPortalDefinition))
            .Verifiable();
        repoMock.Setup(x => x.UpdateAsync(It.IsAny<PortalDefinition>(), false))
            .Returns(Task.CompletedTask)
            .Callback((PortalDefinition portal, bool _) => resultPortalDefinition = portal)
            .Verifiable();
        PortalDefinitionService service = new(repoMock.Object)
        {
            Validator = validatorMock.Object
        };

        // Act
        var result = await service.UpdatePortalAsync("some-id", ValidUpdateCommand);

        // Assert
        validatorMock.Verify(x => x.ValidateRequest(ValidUpdateCommand), Times.Once);
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
