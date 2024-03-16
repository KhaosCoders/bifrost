using Bifrost;
using Bifrost.Actions;
using Bifrost.Client;
using Bifrost.Features.Identity.Actions;
using MediatR;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.FluentUI.AspNetCore.Components;

// Shows up in browser console (easy hint on WASM)
Console.WriteLine("Running in WASM");

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Authentication
builder.Services.AddOptions();
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddSingleton<AuthenticationStateProvider, PersistentAuthenticationStateProvider>();

// HttpClient
builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// FluentUI
builder.Services.AddFluentUIComponents();

// MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RpcBehavior<,>));

// Client-Side-Requests
builder.Services.AddScoped<ILoginAction, ClientsideLoginAction>();

await builder.Build().RunAsync();
