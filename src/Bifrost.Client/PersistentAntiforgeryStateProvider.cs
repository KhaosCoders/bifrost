using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Bifrost.Client;

public class PersistentAntiforgeryStateProvider : AntiforgeryStateProvider
{
    private const string PersistenceKey = $"__internal__{nameof(AntiforgeryRequestToken)}";

    private readonly AntiforgeryRequestToken? _token;

    public PersistentAntiforgeryStateProvider(PersistentComponentState state)
    {
        Console.WriteLine("Read Antiforgery...");
        state.TryTakeFromJson<AntiforgeryRequestToken>(PersistenceKey, out _token);
        Console.WriteLine($"Read: {_token}");
    }

    public override AntiforgeryRequestToken? GetAntiforgeryToken() => _token;
}
