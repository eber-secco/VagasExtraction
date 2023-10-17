using OpenQA.Selenium.Chrome;

namespace VagasExtraction.Selenium;

public class DriverOptions
{
    public DriverOptions(bool headLess = true, bool disableGPU = false, string pathDownload = "LinuxPath")
    {
        HeadLess = headLess;
        DisableGPU = disableGPU;
        PathDownload = pathDownload;
    }

    public bool HeadLess { get; }
    public bool DisableGPU { get; set; }
    public string PathDownload { get; }

    public ChromeOptions GetDriverOptions()
    {
        var options = new ChromeOptions();
        options.AddArgument("--whitelisted-ips=");
        options.AddUserProfilePreference("download.prompt_for_download", false);
        options.AddUserProfilePreference("download.directory_upgrade", true);
        options.AddUserProfilePreference("intl.accept_languages", "pt");
        options.AddUserProfilePreference("disable-popup-blocking", "true");
        options.AddUserProfilePreference("download.default_directory", PathDownload);
        options.AddArgument("--no-sandbox");
        options.AddArgument("--disable-dev-shm-usage");
        options.AddArgument("--disable-popup-blocking");

        if (HeadLess)
            options.AddArgument("--headless");

        if (DisableGPU)
            options.AddArgument("--disable-gpu");

        options.AcceptInsecureCertificates = true;

        return options;
    }
}
