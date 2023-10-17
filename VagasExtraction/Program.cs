using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.RegularExpressions;
using VagasExtraction.Config;
using VagasExtraction.Services;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

var directory = Regex.Replace(Directory.GetCurrentDirectory(), @"\\bin.+", "");
var config = new ConfigurationBuilder()
        .SetBasePath(directory)
        .AddJsonFile("appsettings.json", optional: true)
        .Build();

builder.Services.AddServicesInjection(config);
builder.Services.AddHostedService<ExtractionService>();

using IHost host = builder.Build();

await host.RunAsync();