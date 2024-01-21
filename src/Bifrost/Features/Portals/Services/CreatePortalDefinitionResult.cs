﻿using Bifrost.Client.Utils.Validation;
using Bifrost.Features.Portals.Model;

namespace Bifrost.Features.Portals.Services;

internal record CreatePortalDefinitionResult(bool IsSuccess, PortalDefinition? Portal, IEnumerable<ValidationFault> Faults)
    : ServiceResultBase(IsSuccess, Faults);
