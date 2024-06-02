using chd.CaraVan.DataAccess;
using chd.CaraVan.Worker;



using var ctx = new InfluxContext();

await ctx.WriteSolarData(DateTime.Now, 3m, 9);


var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
