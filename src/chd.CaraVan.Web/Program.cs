using chd.CaraVan.UI.Extensions;
using chd.CaraVan.UI.Components;
using chd.CaraVan.Web.Components;
using System.Diagnostics;
using NLog.Extensions.Logging;
using chd.CaraVan.UI.Hubs;
using chd.CaraVan.Web.Endpoints;

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


builder.Services.AddSignalR();
builder.Services.AddUi(builder.Configuration);
builder.Services.AddRazorComponents().AddInteractiveServerComponents().AddCircuitOptions(opt =>
{
    opt.DetailedErrors = true;
});

builder.Services.AddEndpointsApiExplorer();

builder.Host.UseSystemd();

var app = builder.Build();



app.UseExceptionHandler("/Error", createScopeForErrors: true);

app.UseStaticFiles();
app.UseAntiforgery();

app.AddApi();


app.MapHub<DataHub>("data-hub");

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode().
    AddAdditionalAssemblies(typeof(Routes).Assembly);

app.Run();
