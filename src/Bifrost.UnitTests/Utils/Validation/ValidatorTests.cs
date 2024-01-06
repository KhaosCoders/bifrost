using Bifrost.Client.Utils.Validation;

namespace Bifrost.UnitTests.Utils.Validation;

public class ValidatorTests
{
    public int Number { get; } = 1;

    public string Text { get; set; } = "test";

    [Test]
    public void Validate_ReturnsIsValid_ForJustValidRules()
    {
        // Arrange
        ValidationRule[] rules =
        [
            new ValidationRule<string>(() => Text, _ => true),
            new ValidationRule<int>(() => Number, _ => true)
        ];

        // Act
        var result = Validator.Validate(rules);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
        result.Faults.Should().BeEmpty();
    }

    [Test]
    public void Validate_ReturnsIsNotValid_ForJustInvalidRules()
    {
        // Arrange
        ValidationRule[] rules =
        [
            new ValidationRule<string>(() => Text, _ => false),
            new ValidationRule<int>(() => Number, _ => false)
        ];

        // Act
        var result = Validator.Validate(rules);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Faults.Should().HaveCount(2);
    }

    [Test]
    public void Validate_ReturnsIsNotValid_ForMixedRules()
    {
        // Arrange
        ValidationRule[] rules =
        [
            new ValidationRule<string>(() => Text, _ => true),
            new ValidationRule<int>(() => Number, _ => false)
        ];

        // Act
        var result = Validator.Validate(rules);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Faults.Should().HaveCount(1);
    }

    [Test]
    public void Validate_ReturnsIsValid_ForNoRules()
    {
        // Arrange
        ValidationRule[] rules = [];

        // Act
        var result = Validator.Validate(rules);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
        result.Faults.Should().BeEmpty();
    }
}
