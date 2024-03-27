using Bifrost.Behaviors;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace Bifrost.Tests.Behaviors;

[TestClass]
public class ValidationBehaviorTests
{
    [TestMethod]
    public async Task Handle_CallsAllValidatorsAndReturnsResult()
    {
        // Arrange
        DummyResult dummyResult = new();
        List<Mock<IValidator<DummyCommand>>> validators =
            Enumerable.Range(1, 3)
                .Select(_ =>
                {
                    Mock<IValidator<DummyCommand>> mock = new();
                    mock.Setup(x => x.ValidateAsync(It.IsAny<IValidationContext>(), It.IsAny<CancellationToken>()))
                        .Returns(Task.FromResult(new FluentValidation.Results.ValidationResult()))
                        .Verifiable(Times.Once);
                    return mock;
                })
                .ToList();
        Task<DummyResult> next() => Task.FromResult(dummyResult);

        ValidationBehavior<DummyCommand, DummyResult> behavior = new(validators.Select(v => v.Object));
        DummyCommand cmd = new();

        // Act
        var result = await behavior.Handle(cmd, next, CancellationToken.None);

        // Assert
        validators.ForEach(v => v.Verify());
        result.Should().Be(dummyResult);
    }

    [TestMethod]
    public async Task Handle_ThrowsValidationException_OnFailedValidation()
    {
        // Arrange
        ValidationFailure failure = new("Prop", "Error");
        Mock<IValidator<DummyCommand>> validatorMock = new();
        validatorMock.Setup(x => x.ValidateAsync(It.IsAny<IValidationContext>(), It.IsAny<CancellationToken>()))
                        .Returns(Task.FromResult(new FluentValidation.Results.ValidationResult([failure])));
        static Task<DummyResult> next() => Task.FromResult(new DummyResult());

        ValidationBehavior<DummyCommand, DummyResult> behavior = new([validatorMock.Object]);
        DummyCommand cmd = new();

        // Act
        var act = () => behavior.Handle(cmd, next, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    public record DummyCommand() : IRequest<DummyResult>;

    public record DummyResult();
}
