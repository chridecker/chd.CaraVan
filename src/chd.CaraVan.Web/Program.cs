using chd.CaraVan.UI.Extensions;
using chd.CaraVan.UI.Components;
using chd.CaraVan.Web.Components;
using System.Diagnostics;
using NLog.Extensions.Logging;

if (!(Debugger.IsAttached || args.Contains("--console")))
{
    var pathToExe = Process.GetCurrentProcess()?.MainModule?.FileName ?? string.Empty;
    var pathToContentRoot = Path.GetDirectoryName(pathToExe);
    if (!string.IsNullOrWhiteSpace(pathToContentRoot))
    {
        Directory.SetCurrentDirectory(pathToContentRoot);
    }
}


var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders().AddNLog();

builder.Services.AddUi(builder.Configuration);
builder.Services.AddRazorComponents().AddInteractiveServerComponents().AddCircuitOptions(opt =>
{
    opt.DetailedErrors = true;
});

var app = builder.Build();

app.UseExceptionHandler("/Error", createScopeForErrors: true);

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode().
    AddAdditionalAssemblies(typeof(Routes).Assembly);

app.Run();
