using Bifrost.Guards;

namespace Bifrost.Tests.Guards;

[TestClass]
public class GuardAgainstEmptyStringTests
{
    [TestMethod]
    public void StringIsNullOrWhitespace_ReturnsTrue_ForNonEmptyStrings()
    {
        // Arrange
        const string nonEmpty = "non-empty";

        // Act
        bool result = Guard.Against.StringIsNullOrWhitespace(nonEmpty, throws: false);

        // Assert
        result.Should().BeTrue();
    }

    [TestMethod]
    public void StringIsNullOrWhitespace_ReturnsFalse_ForEmptyStrings()
    {
        // Arrange
        const string empty = "";

        // Act
        bool result = Guard.Against.StringIsNullOrWhitespace(empty, throws: false);

        // Assert
        result.Should().BeFalse();
    }

    [TestMethod]
    public void StringIsNullOrWhitespace_ReturnsFalse_ForNullStrings()
    {
        // Arrange
        const string? nullString = null;

        // Act
        bool result = Guard.Against.StringIsNullOrWhitespace(nullString, throws: false);

        // Assert
        result.Should().BeFalse();
    }

    [TestMethod]
    public void StringIsNullOrWhitespace_ThrowsArgumentException_ForEmptyStrings()
    {
        // Arrange
        const string empty = "";

        // Act
        Action act = () => Guard.Against.StringIsNullOrWhitespace(empty);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [TestMethod]
    public void StringIsNullOrWhitespace_ThrowsArgumentNullException_ForNullStrings()
    {
        // Arrange
        const string? nullString = null;

        // Act
        Action act = () => Guard.Against.StringIsNullOrWhitespace(nullString);

        // Assert
        act.Should().Throw<ArgumentException>();
    }
}
