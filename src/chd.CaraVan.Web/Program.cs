using chd.CaraVan.UI.Components;
using chd.CaraVan.Web.Components;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddUI

// Add services to the container.
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode().
    AddAdditionalAssemblies(typeof(Routes).Assembly);

app.Run();
