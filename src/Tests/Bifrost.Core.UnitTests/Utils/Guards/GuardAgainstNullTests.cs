using Bifrost.Utils.Guards;

namespace Bifrost.UnitTests.Utils.Guards;

[TestClass]
public class GuardAgainstNullTests
{
    [TestMethod]
    public void IsNull_ThrowsArgumentNullException_ForNullValue()
    {
        // Arrange
        const string? nullString = null;

        // Act
        Action act = () => Guard.Against.IsNull(nullString);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [TestMethod]
    public void IsNull_ReturnsFalse_ForNullValue()
    {
        // Arrange
        const string? nullString = null;

        // Act
        bool result = Guard.Against.IsNull(nullString, throws: false);

        // Assert
        result.Should().BeFalse();
    }

    [TestMethod]
    public void IsNull_ReturnsTrue_ForNonNullValue()
    {
        // Arrange
        const string nonNull = "non-null";

        // Act
        bool result = Guard.Against.IsNull(nonNull);

        // Assert
        result.Should().BeTrue();
    }
}
