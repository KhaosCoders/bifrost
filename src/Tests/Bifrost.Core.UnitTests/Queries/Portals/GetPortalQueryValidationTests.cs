using Bifrost.Queries.Portals;

namespace Bifrost.Tests.Queries.Portals;

[TestClass]
public class GetPortalQueryValidationTests
{
    private static GetPortalQuery ValidQuery => new("id-1");

    private readonly GetPortalQueryValidator Validator = new();

    [TestMethod]
    public void Validate_ShouldSucceed_ForValidCommand()
    {
        // Arrange
        var command = ValidQuery;

        // Act
        var result = Validator.Validate(command);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [TestMethod]
    public void Validate_ShouldFail_WhenIdIsEmpty()
    {
        // Arrange
        var command = ValidQuery with { Id = string.Empty };

        // Act
        var result = Validator.Validate(command);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(GetPortalQuery.Id));
    }

    [TestMethod]
    public void Validate_ShouldFail_WhenIdIsWhitespace()
    {
        // Arrange
        var command = ValidQuery with { Id = " " };

        // Act
        var result = Validator.Validate(command);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(GetPortalQuery.Id));
    }
}
