using Bifrost.Shared;

namespace Bifrost.Features;

internal record ServiceResult(bool IsSuccess, ErrorDetails? ErrorDetails = default);
