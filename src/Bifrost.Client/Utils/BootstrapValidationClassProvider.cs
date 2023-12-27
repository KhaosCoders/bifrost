using Microsoft.AspNetCore.Components.Forms;

namespace Bifrost.Client.Utils;

public class BootstrapValidationClassProvider : FieldCssClassProvider
{
    public override string GetFieldCssClass(EditContext editContext,
        in FieldIdentifier fieldIdentifier)
    {
        var isValid = !editContext.GetValidationMessages(fieldIdentifier).Any();

        return isValid ? "" : "is-invalid";
    }
}
