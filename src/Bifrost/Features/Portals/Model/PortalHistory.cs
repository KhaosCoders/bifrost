﻿namespace Bifrost.Features.Portals.Model;

public class PortalHistory
{
    public string Id { get; set; } = default!;

    public required PortalInstance Instance { get; set; }

    public required string InstanceId { get; set; }

    public PortalState State { get; set; }

    public required DateTime CreationDate { get; set; }

    public required string CreationUser { get; set; }

    public DateTime? CloseDate { get; set; }

    public TimeSpan? Period => CloseDate == null ? default : CloseDate.Value - CreationDate;

    public PortalHistory()
    {
        Id = Guid.NewGuid().ToString();
    }
}
