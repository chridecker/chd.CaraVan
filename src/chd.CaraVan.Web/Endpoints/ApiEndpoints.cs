using chd.CaraVan.Contracts.Dtos;
using chd.CaraVan.Devices;
using chd.CaraVan.UI.Implementations;
using System.Runtime.CompilerServices;

namespace chd.CaraVan.Web.Endpoints
{
    public static class ApiEndpoints
    {
        public static IEndpointRouteBuilder AddApi(this IEndpointRouteBuilder app)
        {
            var api = app.MapGroup("/api");

            api.MapGet("/RuuviSensorData/{id:int}", async (int id, IRuuviTagDataService svc) =>
            {
                return await svc.GetData(id);
            });
            api.MapGet("/RuuviSensorDevices", async (IRuuviTagDataService svc) =>
            {
                return await svc.Devices;
            });
            api.MapGet("/VotronicSolar", async (IVotronicDataService svc) =>
            {
                return await svc.GetSolarData();
            });
            api.MapGet("/VotronicBattery", async (IVotronicDataService svc) =>
            {
                return await svc.GetBatteryData();
            });
            api.MapGet("/Victron", async (IVictronDataService svc) =>
            {
                return await svc.GetData();
            });

            var pi = api.MapGroup("/pi");
            pi.MapGet("/Read/{pin:int}", async (int pin, IPiManager manager) => await manager.Read(pin));
            pi.MapPost("/Write", async (PinWriteDto dto, IPiManager manager) => await manager.Write(dto.Pin, dto.Value));
            pi.MapGet("/Settings", async (IPiManager manager, CancellationToken cancellationToken) => await manager.GetSettings(cancellationToken));

            var aes = api.MapGroup("/aes");

            aes.MapGet("/IsActive", async (IAESManager manager) => await manager.IsActive);
            aes.MapGet("/SolarAesOffSince", async (IAESManager manager) => await manager.SolarAesOffSince);
            aes.MapGet("/BatteryLimit", async (IAESManager manager) => await manager.BatteryLimit);
            aes.MapGet("/AesTimeout", async (IAESManager manager) => await manager.AesTimeout);
            aes.MapGet("/SolarAmpLimit", async (IAESManager manager) => await manager.SolarAmpLimit);

            aes.MapPost("/SetActive", async (IAESManager manager) => await manager.SetActive());
            aes.MapPost("/CheckForActive", async (IAESManager manager) => await manager.CheckForActive());
            aes.MapPost("/Off", async (IAESManager manager) => await manager.Off());

            return app;
        }
    }
}
