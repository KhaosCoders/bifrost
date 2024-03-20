using Bifrost.Data.Base;
using Bifrost.Shared;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Bifrost.Data;

public static partial class RepositoryExceptionHandler
{
    public static ErrorDetails? ToErrorDetails(Exception ex)
    {
        // EntityNotFoundException
        switch (ex)
        {
            case EntityNotFoundException entityNotFoundException:
                return ErrorDetails.SingleError("NotFound", "Entity not found");
            case DbUpdateException dbUpdateException when dbUpdateException.InnerException is Exception innerEx:
                if (IsUniqueConstraint().Match(innerEx.Message) is Match match && match.Success)
                {
                    return ErrorDetails.SingleError("DuplicateEntry", match.Groups[1].Value);
                }
                break;
        }

#if DEBUG
        // Unknown exception in repository, analyze in debug mode
        if (System.Diagnostics.Debugger.IsAttached)
        {
            System.Diagnostics.Debugger.Break();
        }
#endif

        return null;
    }

    [GeneratedRegex(@"SQLite Error 19: 'UNIQUE constraint failed: ([\w\.\d]+)'", RegexOptions.Compiled)]
    private static partial Regex IsUniqueConstraint();
}
