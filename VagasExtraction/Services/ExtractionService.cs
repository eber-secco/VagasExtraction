using HtmlAgilityPack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using System.Diagnostics;
using System.Text.RegularExpressions;
using VagasExtraction.Models;
using VagasExtraction.Selenium;
using VagasExtraction.Tools;

namespace VagasExtraction.Services;

public class ExtractionService : IHostedService
{
    private DriverHelper driverHelper;
    private ChromeDriver? driver;
    private List<Vacancy> vacancies = new List<Vacancy>();
    private List<string> classificationsBNE = new List<string>
    {
        "via BNE",
        "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQIdYlIeHTbxbX2lTyqAy2MoX9PknCNqfgie9H5&s=0",
        "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSKwh-jfLUVPtkKYg1S2dlm-PiFS4J_l4WvWKBN&s=0",
        "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQw97D16uG0NDRESKpHWCUNpABqDzaNxcNGdN0K&s=0",
    };

    public ExtractionService(IConfiguration configuration)
    {
        driverHelper = new DriverHelper(configuration);
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {

            Selenium.DriverOptions driverOptions = new Selenium.DriverOptions();
#if DEBUG
            driverOptions = new Selenium.DriverOptions(headLess: false);
#endif

            driver = driverHelper.GetDriver("Chave01",  driverOptions);

            if (driver is null)
                throw new NullReferenceException(nameof(driver));

            foreach (var suffix in SearchSuffixes.Suffixes)
            {
                var timer = Stopwatch.StartNew();

                if (vacancies.Count >= 6000)
                    break;

                for (var i = 0; i < 3; i++)
                {
                    if (driver is null)
                        continue;

                    var suffixFormated = Tool.RemoveSpecialChars(Regex.Replace(suffix.Suffix.Trim(), @"\s{1,}", "+")).ToLower();
                    suffixFormated = Regex.Replace(suffixFormated, @",|\.", "");

                    driver.Navigate().GoToUrl($"https://www.google.com.br/search?q=vagas+de+emprego+{suffixFormated}");
                    await Task.Delay(2500);

                    if (!driver.Url.Contains("sorry/index?continue"))
                        break;
                    else
                        driver = driverHelper.RestartDriver("Chave01", driverOptions);
                }

                if (driver is null || driver.Url.Contains("sorry/index?continue"))
                    continue;

                driver.FindElement(By.XPath("//div[@class='nJXhWc']//g-link/a")).Click();
                await Task.Delay(2500);
                driver.FindElement(By.XPath("//div[@class='TRwkpf GbaVB yjYmLb']")).Click();
                await Task.Delay(2500);

                var vacancyNodeCollection = await GetVacancyNodeCollectionAsync();

                if (vacancyNodeCollection is null)
                    throw new NullReferenceException(nameof(vacancyNodeCollection));

                for (var i = 0; i < vacancyNodeCollection.Count; i++)
                {
                    if (timer.Elapsed.TotalMinutes == 10)
                        break;

                    var isClassificated = ClassificationVacancy(vacancyNodeCollection[i]);

                    if (isClassificated)
                    {
                        try
                        {
                            var element = driver.FindElement(By.XPath($"{vacancyNodeCollection[i].XPath}//div[@class='BjJfJf PUpOsf']"));
                            var actions = new Actions(driver);
                            actions.MoveToElement(element).Perform();

                            await Task.Delay(500);
                            element.Click();
                            await Task.Delay(500);

                            var vacancy = ExtractionVacancy(vacancyNodeCollection[i]);

                            if (vacancy != null && !vacancies.Contains(vacancy))
                                vacancies.Add(vacancy);
                        }
                        catch (Exception ex)
                        {
                            await Console.Out.WriteLineAsync(ex.Message);
                        }
                    }
                }
            }

            //manda pra planilha e dá tres pulinhos
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        if (driver != null)
            driver.Dispose();

        Console.WriteLine("Finalizado com sucesso!");

        return Task.CompletedTask;
    }

    private Vacancy? ExtractionVacancy(HtmlNode vacancyNode)
    {
        if (driver is null)
            return null;

        var idBNE = string.Empty;
        var listVacanciesAccess = new List<VacancyAccess>();

        var html = new HtmlDocument();
        html.LoadHtml(driver.PageSource);

        //Obtem os Access
        var AccessNodeCollection = html.DocumentNode.SelectNodes("//div[@class='whazf bD1FPe']//div[@class='EDblX kjqWgb']//a");

        //Obtem Url da vaga no google
        var urlGoogle = html.DocumentNode.SelectSingleNode("//div[@class='whazf bD1FPe']/div/div[1]/div").GetAttributeValue("data-share-url", "");
        urlGoogle = Regex.Replace(urlGoogle, @"amp;", "");

        //Obtem os VacancyAccess e Id BNE
        foreach (var accessNode in AccessNodeCollection)
        {
            var accessName = Regex.Replace(accessNode.InnerText, @"Acesse em ?", "");
            var url = accessNode.GetAttributeValue("href", "");

            url = Regex.Replace(url, @"amp;", "");

            if (accessName == "BNE")
                idBNE = Regex.Match(url, @"(\d+)\?").Groups[1].Value;

            if (!string.IsNullOrEmpty(accessName) && !string.IsNullOrEmpty(url))
                listVacanciesAccess.Add(new VacancyAccess(accessName, url));
        }

        if (!string.IsNullOrEmpty(idBNE) && listVacanciesAccess.Count > 0)
            return new Vacancy(idBNE, urlGoogle, listVacanciesAccess, DateTime.Now);

        return null;
    }

    /// <summary>
    /// Desce tudo pra baixo e obtem o HtmlNodeCollection das vagas
    /// </summary>
    /// <returns>vacancyNodeCollection</returns>
    private async Task<HtmlNodeCollection?> GetVacancyNodeCollectionAsync()
    {
        if (driver is null)
            return null;

        HtmlNodeCollection? vacancyNodeCollection = null;
        var tries = 3;

        var html = new HtmlDocument();
        html.LoadHtml(driver.PageSource);

        var actions = new Actions(driver);
        var js = (IJavaScriptExecutor)driver;

        for (var i = 0; i < 3; i++)
        {
            await Task.Delay(500);
            actions.SendKeys(Keys.Tab).Build().Perform();
        }

        //Scroll
        while (true)
        {
            if (tries <= 0)
                break;

            var vacancyNodeCollectionValidate = vacancyNodeCollection;

            //Testar
            //js.ExecuteScript($"window.scrollTo(0, document.body.scrollHeight)");
            for (var i = 0; i < 30; i++)
            {
                await Task.Delay(200);
                actions.SendKeys(Keys.ArrowDown).Build().Perform();
            }

            await Task.Delay(2500);

            html.LoadHtml(driver.PageSource);

            vacancyNodeCollection = html.DocumentNode.SelectNodes("//ul/li[@class='iFjolb gws-plugins-horizon-jobs__li-ed']");

            if (vacancyNodeCollectionValidate is null)
            {
                tries--;
                continue;
            }

            if (vacancyNodeCollectionValidate.Count == vacancyNodeCollection.Count)
                tries--;
        }

        if (vacancyNodeCollection is null)
            return null;
        else
            return vacancyNodeCollection;
    }

    /// <summary>
    /// Classsicação feita dentro de cada item da lista de vagas
    /// </summary>
    /// <returns>isClassificated</returns>
    private bool ClassificationVacancy(HtmlNode vacancyNode)
    {
        var classifications = new List<string>();
        var isClassificated = false;

        var html = new HtmlDocument();
        html.LoadHtml(vacancyNode.InnerHtml);

        //Obter classificação by Via 'Empresa'
        classifications.Add(html.DocumentNode.SelectSingleNode("//div[@class='Qk80Jf'][2]").InnerText);


        //Obter classificação by img (src)
        var img = html.DocumentNode.SelectSingleNode("//img");
        if (img != null)
            classifications.Add(img.GetAttributeValue("src", ""));

        foreach (var item in classifications)
        {
            var itemFormated = Regex.Replace(item, @"amp;", "");
            if (classificationsBNE.Contains(itemFormated))
                isClassificated = true;
        }

        return isClassificated;
    }
}
