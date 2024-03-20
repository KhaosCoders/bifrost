using Bifrost.Guards;

namespace Bifrost.Tests.Guards;

[TestClass]
public class GuardAgainstLessThanTests
{
    [TestMethod]
    public void IsLessThan_ThrowsArgumentException_ForLessThanValue()
    {
        // Arrange
        const int value = 1;
        const int minimum = 2;

        // Act
        Action act = () => Guard.Against.IsLessThan(value, minimum);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [TestMethod]
    public void IsLessThan_ReturnsFalse_ForLessThanValue()
    {
        // Arrange
        const int value = 1;
        const int minimum = 2;

        // Act
        bool result = Guard.Against.IsLessThan(value, minimum, throws: false);

        // Assert
        result.Should().BeFalse();
    }

    [TestMethod]
    public void IsLessThan_ReturnsTrue_ForEqualValue()
    {
        // Arrange
        const int value = 2;
        const int minimum = 2;

        // Act
        bool result = Guard.Against.IsLessThan(value, minimum);

        // Assert
        result.Should().BeTrue();
    }

    [TestMethod]
    public void IsLessThan_ReturnsTrue_ForGreaterThanValue()
    {
        // Arrange
        const int value = 3;
        const int minimum = 2;

        // Act
        bool result = Guard.Against.IsLessThan(value, minimum);

        // Assert
        result.Should().BeTrue();
    }

}
