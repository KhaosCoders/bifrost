using Bifrost;
using Bifrost.Client;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.FluentUI.AspNetCore.Components;

// Shows up in browser console (easy hint on WASM)
Console.WriteLine("Running in WASM");

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Fluent Validators (Bifrost.Core)
builder.Services.AddValidatorsFromAssemblyContaining(typeof(VpnTypes));

// MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RpcBehavior<,>));

// Authentication
builder.Services.AddOptions();
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddSingleton<AuthenticationStateProvider, PersistentAuthenticationStateProvider>();

// HttpClient
builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// FluentUI
builder.Services.AddFluentUIComponents();

await builder.Build().RunAsync();
