using Bifrost.Client.Utils.Guards;

namespace Bifrost.UnitTests.Utils.Guards;
public class GuardAgainstLessThanTests
{
    [Test]
    public void IsLessThan_ThrowsArgumentException_ForLessThanValue()
    {
        // Arrange
        const int value = 1;
        const int minimum = 2;

        // Act
        Action act = () => Guard.Against.IsLessThan(value, minimum, nameof(value));

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Test]
    public void IsLessThan_ReturnsFalse_ForLessThanValue()
    {
        // Arrange
        const int value = 1;
        const int minimum = 2;

        // Act
        bool result = Guard.Against.IsLessThan(value, minimum, nameof(value), throws: false);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void IsLessThan_ReturnsTrue_ForEqualValue()
    {
        // Arrange
        const int value = 2;
        const int minimum = 2;

        // Act
        bool result = Guard.Against.IsLessThan(value, minimum, nameof(value));

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void IsLessThan_ReturnsTrue_ForGreaterThanValue()
    {
        // Arrange
        const int value = 3;
        const int minimum = 2;

        // Act
        bool result = Guard.Against.IsLessThan(value, minimum, nameof(value));

        // Assert
        result.Should().BeTrue();
    }

}
