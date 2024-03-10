using Bifrost.Client;
using Bifrost.Client.Features.Identity.Actions;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.FluentUI.AspNetCore.Components;

// Shows up in browser console (easy hint on WASM)
Console.WriteLine("Running in WASM");

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddOptions();
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddSingleton<AuthenticationStateProvider, PersistentAuthenticationStateProvider>();

// HttpClient
builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// FluentUI
builder.Services.AddFluentUIComponents();

// Client-Side-Requests
builder.Services.AddScoped<ILoginAction, ClientsideLoginAction>();

await builder.Build().RunAsync();
