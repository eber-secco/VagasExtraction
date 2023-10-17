using Microsoft.Extensions.Configuration;
using OpenQA.Selenium.Chrome;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace VagasExtraction.Selenium;

public class DriverHelper
{
    private string? driverPath;
    private Dictionary<string, ChromeDriver> drivers = new Dictionary<string, ChromeDriver>();

    public DriverHelper(IConfiguration configuration)
    {
        driverPath = configuration["Driver:Path"];
    }

    #region Open driver
    public ChromeDriver? GetDriver(string key, DriverOptions driverConfig)
    {
        try
        {
            if (string.IsNullOrEmpty(driverPath))
                throw new NullReferenceException(nameof(driverPath));

            string fullPath = Regex.Replace(Directory.GetCurrentDirectory(), @"\\bin.+", "") + driverPath;

            Console.WriteLine($"Caminho do Selleniun driver:\n{fullPath}\n");

            if (!drivers.ContainsKey(key))
            {
                var newDriver = new ChromeDriver(fullPath, driverConfig.GetDriverOptions(), TimeSpan.FromSeconds(180));
                drivers.Add(key, newDriver);
            }

            return drivers[key];
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return null;
        }
    }

    public ChromeDriver? RestartDriver(string key, DriverOptions driverConfig)
    {
        try
        {
            string fullPath = Regex.Replace(Directory.GetCurrentDirectory(), @"\\bin.+", "") + driverPath;

            var newDriver = new ChromeDriver(fullPath, driverConfig.GetDriverOptions());

            if (drivers.ContainsKey(key))
            {
                FreeDriver(key);
                drivers.Add(key, newDriver);
            }

            return drivers[key];
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return null;
        }
    }
    #endregion

    #region Close drivers
    public void Dispose()
    {
        foreach (var driver in drivers)
            FreeDriver(driver.Key);
        
        foreach (var chromeDriverProcess in Process.GetProcessesByName("chromedriver"))
            chromeDriverProcess.Kill();
    }
    public void FreeDriver(string key)
    {
        try
        {
            if (drivers.ContainsKey(key))
            {
                drivers[key].Close();
                drivers[key].Quit();
                drivers[key].Dispose();

                drivers.Remove(key);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    #endregion
}
