using Bifrost.Commands.Portals;
using Bifrost.Features.Portals.Commands;
using Bifrost.Models.Portals;
using Bifrost.Shared.Dialogs;
using MediatR;
using Microsoft.FluentUI.AspNetCore.Components;

namespace Bifrost.Features.Portals.Handlers;

public class TriggerPortalDeleteHandler(
    IDialogService dialogService,
    IToastService toastService,
    ISender mediator) : IRequestHandler<TriggerPortalDeleteCommand>
{
    private readonly IDialogService dialogService = dialogService;
    private readonly IToastService toastService = toastService;
    private readonly ISender mediator = mediator;

    public async Task Handle(TriggerPortalDeleteCommand request, CancellationToken cancellationToken)
    {
        var portal = request.Portal;
        ConfirmationContext ctx = new(portal, $"Delete portal {portal.Name}?");
        await dialogService.ShowDialogAsync<ConfirmationDialog>(ctx, new DialogParameters()
        {
            Title = $"Deleting {portal.Name}",
            OnDialogResult = dialogService.CreateDialogCallback(this, HandleDelete),
            PrimaryAction = "Yes",
            SecondaryAction = "No",
            TrapFocus = true,
            Modal = true,
        });

        await ctx.ConfirmationHandledSource.Task;
    }

    private async Task HandleDelete(DialogResult dialogResult)
    {
        if (dialogResult.Cancelled
            || dialogResult.Data is not ConfirmationContext ctx
            || ctx.Context is not PortalDefinition portal)
        {
            (dialogResult.Data as ConfirmationContext)?.ConfirmationHandledSource.TrySetResult();
            return;
        }

        string toastId = $"delete-portal-{portal.Id}";

        toastService.ShowProgressToast(new()
        {
            Id = toastId,
            Intent = ToastIntent.Progress,
            Title = "Deleting portal",
            Content = new()
            {
                Details = "Please wait while we delete the portal...",
            }
        });

        try
        {
            var result = await mediator.Send(new DeletePortalCommand(portal.Id));
            if (result.Success)
            {
                toastService.ShowSuccess("Portal deleted successfully");
            }
            else
            {
                toastService.ShowError($"Failed to delete portal: {result.Data.ErrorDetails?.FirstOrDefault().Value.FirstOrDefault() ?? "Unknown Error"}");
            }
        }
        catch (Exception ex)
        {
            toastService.ShowError($"Error: {ex.Message}");
        }
        finally
        {
            toastService.CloseToast(toastId);
            ctx.ConfirmationHandledSource.TrySetResult();
        }
    }
}
