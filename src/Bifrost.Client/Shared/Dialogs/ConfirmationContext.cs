namespace Bifrost.Shared.Dialogs;

public record ConfirmationContext(object Context, string Question)
{
    public TaskCompletionSource ConfirmationHandledSource { get; } = new();
}
