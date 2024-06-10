
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VertMarketsServices.Services;
using VertMarketsServices.Interfaces;
using VertMarketsServices.ConfigModels;


var ConfigBuilder = new ConfigurationBuilder().AddJsonFile("AppSettings.json") ;
var config = ConfigBuilder.Build();

//service provider additions 
var ServiceProvider = new ServiceCollection();
ServiceProvider.AddScoped<IVertMarketsService, VertMarketsService>();
ServiceProvider.Configure<EndPointsOptions>(config.GetSection(EndPointsOptions.configSection));
ServiceProvider.Configure<VertMarketsApiOptions>(config.GetSection(VertMarketsApiOptions.configSection));
ServiceProvider.AddHttpClient(config.GetValue<string>($"{VertMarketsApiOptions.configSection}:name"), client =>
{
    client.BaseAddress = new Uri(config.GetValue<string>($"{VertMarketsApiOptions.configSection}:baseUrl"));
});
ServiceProvider.AddMemoryCache();

var services = ServiceProvider.BuildServiceProvider();


var VertMarketService = services.GetRequiredService<IVertMarketsService>();
var result = await VertMarketService.IdentifySubscribersWhoSubscribedAllCategories();
Console.WriteLine("========================Final Response=====================================");
Console.WriteLine(result);
Console.WriteLine("============================================================================");




