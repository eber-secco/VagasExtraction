
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VagasExtraction.Config;

public class Program
{
    private static void Main(string[] args)
    {
        var collection = new ServiceCollection();
        IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

        collection.AddServicesInjection(config);
        //IServiceProvider = collection.BuildServiceProvider();
    }

}