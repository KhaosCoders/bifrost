using Bifrost.Commands.Portals;
using Bifrost.Queries.Portals;

namespace Bifrost.Tests.Queries.Portals;

[TestClass]
public class GetPortalsQueryValidationTests
{
    private static GetPortalsQuery ValidQuery => new(25, 25);

    private readonly GetPortalsQueryValidator Validator = new();

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
    public void Validate_ShouldFail_ForNegativeOffset()
    {
        // Arrange
        var command = ValidQuery with { Offset = -1 };

        // Act
        var result = Validator.Validate(command);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(GetPortalsQuery.Offset));
    }

    [TestMethod]
    public void Validate_ShouldFail_ForNegativeLimit()
    {
        // Arrange
        var command = ValidQuery with { Limit = -1 };

        // Act
        var result = Validator.Validate(command);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(GetPortalsQuery.Limit));
    }

    [TestMethod]
    public void Validate_ShouldFail_ForZeroLimit()
    {
        // Arrange
        var command = ValidQuery with { Limit = 0 };

        // Act
        var result = Validator.Validate(command);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(GetPortalsQuery.Limit));
    }
}
