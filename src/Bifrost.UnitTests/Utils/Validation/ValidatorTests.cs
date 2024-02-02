﻿using Bifrost.Client.Utils.Validation;

namespace Bifrost.UnitTests.Utils.Validation;

[TestClass]
public class ValidatorTests
{
    public int Number { get; } = 1;

    public string Text { get; set; } = "test";

    [TestMethod]
    public void Rule_Returns_NewValidationRule()
    {
        // Arrange

        // Act
        var result = Client.Utils.Validation.Validation.Rule(() => Text, _ => true);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<ValidationRule<string>>();
    }

    [TestMethod]
    public void Validate_ReturnsIsValid_ForJustValidRules()
    {
        // Arrange
        ValidationRuleBase[] rules =
        [
            new ValidationRule<string>(() => Text, _ => true),
            new ValidationRule<int>(() => Number, _ => true)
        ];

        // Act
        var result = Client.Utils.Validation.Validation.Validate(rules);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
        result.Faults.Should().BeEmpty();
    }

    [TestMethod]
    public void Validate_ReturnsIsNotValid_ForJustInvalidRules()
    {
        // Arrange
        ValidationRuleBase[] rules =
        [
            new ValidationRule<string>(() => Text, _ => false),
            new ValidationRule<int>(() => Number, _ => false)
        ];

        // Act
        var result = Client.Utils.Validation.Validation.Validate(rules);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Faults.Should().HaveCount(2);
    }

    [TestMethod]
    public void Validate_ReturnsIsNotValid_ForMixedRules()
    {
        // Arrange
        ValidationRuleBase[] rules =
        [
            new ValidationRule<string>(() => Text, _ => true),
            new ValidationRule<int>(() => Number, _ => false)
        ];

        // Act
        var result = Client.Utils.Validation.Validation.Validate(rules);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Faults.Should().HaveCount(1);
    }

    [TestMethod]
    public void Validate_ReturnsIsValid_ForNoRules()
    {
        // Arrange
        ValidationRuleBase[] rules = [];

        // Act
        var result = Client.Utils.Validation.Validation.Validate(rules);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
        result.Faults.Should().BeEmpty();
    }
}
