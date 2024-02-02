using Bifrost.Client.Features.Portals.DTO;
using Bifrost.Client.Utils.Validation;
using Bifrost.Features.Identity.Model;
using Bifrost.Features.PortalDefinitions.Model;
using Bifrost.Features.PortalDefinitions.Services;

namespace Bifrost.UnitTests.Features.PortalDefinitions.Services;

[TestClass]
public class PortalDefinitionServiceTests
{
    readonly PortalRequest ValidRequest = new ()
    {
        Name = "UnitTest-Portal",
        MaxInstanceCount = 1,
        VpnType = "utVPN",
        VpnConfig = "Some config"
    };
    readonly PortalRequest EmptyRequest = new()
    {
        Name = "",
        MaxInstanceCount = 0,
        VpnType = "",
        VpnConfig = ""
    };

    [TestMethod]
    public void ValidateRequest_ReturnsTrue_ForValidRequest()
    {
        // Arrange
        PortalDefinitionService service = new(Mock.Of<IPortalDefinitionRepository>());

        // Act
        var result = ((IRequestValidator<PortalRequest>)service).ValidateRequest(ValidRequest);

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
        PortalRequest request = new()
        {
            Name = name!,
            MaxInstanceCount = instanceCount,
            VpnType = vpnType!,
            VpnConfig = vpnConfig!
        };

        // Act
        var result = ((IRequestValidator<PortalRequest>)service).ValidateRequest(request);

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
        ApplicationUser user = new();

        // Act
        var result = await service.CreatePortalAsync(EmptyRequest, user);

        // Assert
        validatorMock.Verify(x => x.ValidateRequest(EmptyRequest), Times.Once);
        repoMock.Verify(x => x.CreateAsync(It.IsAny<PortalDefinition>(), It.IsAny<bool>()), Times.Never);
        repoMock.VerifyNoOtherCalls();
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Faults.Should().NotBeEmpty();
        result.Faults.First().Should().BeEquivalentTo(fault);
    }

    private static (ValidationFault, Mock<IRequestValidator<PortalRequest>>, Mock<IPortalDefinitionRepository>, PortalDefinitionService) PrepareServiceWithValidationFault()
    {
        ValidationFault fault = new("UnitTest", "failing?");
        Client.Utils.Validation.ValidationResult validationResult = new()
        {
            IsValid = false,
            Faults = [fault]
        };
        Mock<IRequestValidator<PortalRequest>> validatorMock = new();
        validatorMock.Setup(x => x.ValidateRequest(It.IsAny<PortalRequest>()))
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
        Mock<IRequestValidator<PortalRequest>> validatorMock = new();
        validatorMock.Setup(x => x.ValidateRequest(It.IsAny<PortalRequest>()))
            .Returns(new Client.Utils.Validation.ValidationResult() { IsValid = true, Faults = [] })
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
        ApplicationUser user = new() { UserName = "UnitTest-User" };

        // Act
        var result = await service.CreatePortalAsync(ValidRequest, user);

        // Assert
        validatorMock.Verify(x => x.ValidateRequest(ValidRequest), Times.Once);
        repoMock.Verify(x => x.CreateAsync(It.IsAny<PortalDefinition>(), false), Times.Once);
        repoMock.VerifyNoOtherCalls();
        result.IsSuccess.Should().BeTrue();
        result.Faults.Should().BeEmpty();
        result.Portal.Should().Be(resultPortalDefinition);
        resultPortalDefinition.Should().NotBeNull();
        resultPortalDefinition!.Name.Should().Be(ValidRequest.Name);
        resultPortalDefinition!.MaxInstanceCount.Should().Be(ValidRequest.MaxInstanceCount);
        resultPortalDefinition!.VpnType.Should().Be(ValidRequest.VpnType);
        resultPortalDefinition!.VpnConfig.Should().Be(ValidRequest.VpnConfig);
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
        Func<Task> act = async () => await service.UpdatePortalAsync(id, ValidRequest);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>().WithParameterName("id");
    }

    [TestMethod]
    public async Task UpdatePortalAsync_Fails_WhenValidationFails()
    {
        // Arrange
        (var fault, var validatorMock, var repoMock, var service) = PrepareServiceWithValidationFault();

        // Act
        var result = await service.UpdatePortalAsync("some-id", EmptyRequest);

        // Assert
        validatorMock.Verify(x => x.ValidateRequest(EmptyRequest), Times.Once);
        repoMock.Verify(x => x.UpdateAsync(It.IsAny<PortalDefinition>(), It.IsAny<bool>()), Times.Never);
        repoMock.VerifyNoOtherCalls();
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Faults.Should().NotBeEmpty();
        result.Faults.First().Should().BeEquivalentTo(fault);
    }

    [TestMethod]
    public async Task UpdatePortalAsync_Fails_ForUnknownId()
    {
        // Arrange
        Mock<IRequestValidator<PortalRequest>> validatorMock = new();
        validatorMock.Setup(x => x.ValidateRequest(It.IsAny<PortalRequest>()))
            .Returns(new Client.Utils.Validation.ValidationResult() { IsValid = true, Faults = [] })
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
        var result = await service.UpdatePortalAsync("invalid-id", ValidRequest);

        // Assert
        validatorMock.Verify(x => x.ValidateRequest(ValidRequest), Times.Once);
        repoMock.Verify(x => x.GetByIdAsync("invalid-id"), Times.Once);
        repoMock.Verify(x => x.UpdateAsync(It.IsAny<PortalDefinition>(), It.IsAny<bool>()), Times.Never);
        repoMock.VerifyNoOtherCalls();
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Faults.Should().NotBeEmpty();
        result.Faults.Should().ContainSingle(f => f.Message == "Portal not found");
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
        Mock<IRequestValidator<PortalRequest>> validatorMock = new();
        validatorMock.Setup(x => x.ValidateRequest(It.IsAny<PortalRequest>()))
            .Returns(new Client.Utils.Validation.ValidationResult() { IsValid = true, Faults = [] })
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
        var result = await service.UpdatePortalAsync("some-id", ValidRequest);

        // Assert
        validatorMock.Verify(x => x.ValidateRequest(ValidRequest), Times.Once);
        repoMock.Verify(x => x.GetByIdAsync("some-id"), Times.Once);
        repoMock.Verify(x => x.UpdateAsync(It.IsAny<PortalDefinition>(), false), Times.Once);
        repoMock.VerifyNoOtherCalls();
        result.IsSuccess.Should().BeTrue();
        result.Faults.Should().BeEmpty();
        resultPortalDefinition.Should().NotBeNull();
        resultPortalDefinition!.Name.Should().Be(ValidRequest.Name);
        resultPortalDefinition!.MaxInstanceCount.Should().Be(ValidRequest.MaxInstanceCount);
        resultPortalDefinition!.VpnType.Should().Be(ValidRequest.VpnType);
        resultPortalDefinition!.VpnConfig.Should().Be(ValidRequest.VpnConfig);
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
