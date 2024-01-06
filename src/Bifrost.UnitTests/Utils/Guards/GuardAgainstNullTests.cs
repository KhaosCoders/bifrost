using Bifrost.Client.Utils.Guards;

namespace Bifrost.UnitTests.Utils.Guards;

public class GuardAgainstNullTests
{
    [Test]
    public void IsNull_ThrowsArgumentNullException_ForNullValue()
    {
        // Arrange
        const string? nullString = null;

        // Act
        Action act = () => Guard.Against.IsNull(nullString, nameof(nullString));

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void IsNull_ReturnsFalse_ForNullValue()
    {
        // Arrange
        const string? nullString = null;

        // Act
        bool result = Guard.Against.IsNull(nullString, nameof(nullString), throws: false);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void IsNull_ReturnsTrue_ForNonNullValue()
    {
        // Arrange
        const string nonNull = "non-null";

        // Act
        bool result = Guard.Against.IsNull(nonNull, nameof(nonNull));

        // Assert
        result.Should().BeTrue();
    }
}
