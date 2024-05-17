// See https://aka.ms/new-console-template for more information
using chd.CaraVan.Devices;

Console.WriteLine("Hello, World!");

var dev = new RuuviTag(new chd.CaraVan.Devices.Contracts.Dtos.RuvviTag.RuvviTagConfiguration
{
    
});

await dev.ConnectAsync();