using Frontend.Components;
using Frontend.Services;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSingleton<AuthService>();
builder.Services.AddSingleton<IUserService, MockUserService>();

var app = builder.Build();

// Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Korrekt placering af routing og antiforgery
app.UseRouting();
app.UseAntiforgery();  // <-- VIGTIGT, placeres her

// Map Razor Components
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();



