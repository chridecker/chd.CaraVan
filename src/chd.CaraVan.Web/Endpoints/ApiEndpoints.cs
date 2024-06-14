using chd.CaraVan.UI.Implementations;
using System.Runtime.CompilerServices;

namespace chd.CaraVan.Web.Endpoints
{
    public static class ApiEndpoints
    {
        public static IEndpointRouteBuilder AddApi(this IEndpointRouteBuilder app)
        {
            var api = app.MapGroup("/api");

            api.MapGet("/RuuviSensorData/{id:int}", (int id, IRuuviTagDataService svc) =>
            {
                return svc.GetData(id, Contracts.Enums.EDataType.Temperature);
            });
            api.MapGet("/RuuviSensorDataMinMax/{id:int}", (int id, IRuuviTagDataService svc) =>
            {
                return svc.GetMinMaxData(id, Contracts.Enums.EDataType.Temperature);
            });
            api.MapGet("/VotronicSolar", (int id, IVotronicDataService svc) =>
            {
                return svc.GetSolarData();
            });
            api.MapGet("/VotronicBattery", (int id, IVotronicDataService svc) =>
            {
                return svc.GetBatteryData();
            });



            return app;
        }
    }
}
