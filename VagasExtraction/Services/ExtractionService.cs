using HtmlAgilityPack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.DevTools.V116.Runtime;
using OpenQA.Selenium.Interactions;
using System.Diagnostics;
using System.Text.RegularExpressions;
using VagasExtraction.Models;
using VagasExtraction.Selenium;
using VagasExtraction.Tools;
using Excel = Microsoft.Office.Interop.Excel;

namespace VagasExtraction.Services;

public class ExtractionService : IHostedService
{
    private int vacanciesLimit = 6000;
    private int suffixesLimit = 6000;

    private DriverHelper driverHelper;
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
        Console.WriteLine($"\nServiço iniciado de extração..\n");
        await ExecuteServiceSeleniumExtractionAsync();
        Console.WriteLine($"\nSalvando {vacancies.Count} vagas na planilha..\n");
        var isSalve = true;

        while (isSalve)
        {
            var isSuccess = SendVacanciesToPlanilha();

            if (!isSuccess)
            {
                Console.WriteLine($"\nARQUIVO NÃO SALVO! DESEJA TENTAR SALVAR NOVAMENTE? S\\N\n");
                var response = Console.ReadLine() ?? "X";

                switch (response.Trim().ToUpper())
                {
                    case "S":
                        Console.WriteLine($"\n---\n");
                        break;
                    case "N":
                        Console.WriteLine($"\nOK, ARQUIVO NÃO SALVO\n");
                        isSalve = false;
                        break;
                    default:
                        Console.WriteLine($"\nESCOLHA DESCONHECIDA\n");
                        Console.WriteLine($"\nO ARQUIVO NÃO FOI SALVO\n");
                        isSalve = false;
                        break;
                }
            }
            else
            {
                isSalve = false;
            }
        }
        
        await StopAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("\nFinalizado com sucesso!\n");
        return Task.CompletedTask;
    }


    /// <summary>
    /// Salva as vagas obtidas em uma planilha de Excel
    /// </summary>
    /// <returns>isSuccess</returns>
    private bool SendVacanciesToPlanilha()
    {
        var isSuccess = false;
        try
        {
            // Inicia o componente Excel
            Excel.Application xlApp;
            Excel.Workbook xlWorkBook;
            Excel.Worksheet xlWorkSheet;
            object misValue = System.Reflection.Missing.Value;
            //Cria uma planilha temporária na memória do computador
            xlApp = new Excel.Application();
            xlWorkBook = xlApp.Workbooks.Add(misValue);
            xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

            //incluindo dados
            //xlWorkSheet.Cells[1, 1] = "Dados do cliente:";
            //xlWorkSheet.Cells[1, 2] = Value;
            //xlWorkSheet.Cells[1, 3] = Value;

            //For all Vacancies
            for (var i = 0; i < vacancies.Count; i++)
            {
                var line = i + 1;

                #region Adiciona Titles
                if (line == 1)
                {
                    var columnTitle = 1;
                    xlWorkSheet.Cells[line, columnTitle] = "Data de pesquisa";

                    columnTitle++;
                    xlWorkSheet.Cells[line, columnTitle] = "Link no Jobs/BNE";

                    columnTitle++;
                    xlWorkSheet.Cells[line, columnTitle] = "Código da vaga";

                    columnTitle++;
                    xlWorkSheet.Cells[line, columnTitle] = "Sites que possuem a vaga";

                    columnTitle++;
                    xlWorkSheet.Cells[line, columnTitle] = "Existe Link BNE?";

                    columnTitle++;
                    xlWorkSheet.Cells[line, columnTitle] = "Vaga manual ou indexada";

                    columnTitle++;
                    xlWorkSheet.Cells[line, columnTitle] = "Data de publicação";

                    columnTitle++;
                    xlWorkSheet.Cells[line, columnTitle] = "Filtro";

                    columnTitle++;
                    xlWorkSheet.Cells[line, columnTitle] = "Navegador";
                }
                #endregion

                #region Adiciona os valores das linhas
                var column = 1;
                line++;

                xlWorkSheet.Cells[line, column] = vacancies[i].SearchDate.ToString("dd/MM/yyyy"); //"Data de pesquisa";

                column++;
                xlWorkSheet.Cells[line, column] = vacancies[i].Url; //"Link no Jobs/BNE";

                column++;
                xlWorkSheet.Cells[line, column] = string.IsNullOrEmpty(vacancies[i].ID) ? "Não encontrado" : vacancies[i].ID; // "Código da vaga";

                column++;
                xlWorkSheet.Cells[line, column] = string.Join(", ", vacancies[i].VacanciesAccess.Select(x => x.NameAccess)); //"Sites que possuem a vaga";

                column++;
                xlWorkSheet.Cells[line, column] = string.IsNullOrEmpty(vacancies[i].ID) ? "Não tem no BNE" : "Tem"; //"Existe Link BNE?";

                column++;
                xlWorkSheet.Cells[line, column] = string.Empty; //"Vaga manual ou indexada";

                column++;
                xlWorkSheet.Cells[line, column] = string.Empty; //"Data de publicação";

                column++;
                xlWorkSheet.Cells[line, column] = string.Empty; //"Filtro";

                column++;
                xlWorkSheet.Cells[line, column] = "Chrome"; //"Navegador";
                #endregion
            }

            //Salva o arquivo de acordo com a documentação do Excel.
            var fileName = $"arquivo.xls";
            var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var fullPath = Path.Combine(path, fileName);

            if (File.Exists(fullPath))
            {
                Console.WriteLine($"\n!!JÁ EXISTE O ARQUIVO ({fileName}) LOCALIZADO EM ({fullPath})!!");
                Console.WriteLine($"\nESCOLHA PROXIMO PASSO DIGITANDO O LETRA CORRESPONDENTE: " +
                    $"\n---SUBSTIUIR O ARQUIVO: A" +
                    $"\n---SALVAR COM OUTRO NOME: B" +
                    $"\n---NÃO SALVAR: C");

                Console.Write($"Sua Resposta: ");
                string response = Console.ReadLine() ?? "X";

                switch (response.Trim().ToUpper())
                {
                    case "A":
                        File.Delete(fullPath);
                        break;
                    case "B":
                        Console.Write($"\nDIGITE O NOME DO ARQUIVO: ");
                        fileName = Console.ReadLine() ?? "arquivo2";

                        if (!fileName.Contains(".xls"))
                            fileName = fileName + ".xls";

                        fullPath = Path.Combine(path, fileName);
                        break;
                    case "C":
                        Console.WriteLine($"\nARQUIVO NÃO SALVO\n");
                        return false;
                    default:
                        Console.WriteLine($"\nESCOLHA DESCONHECIDA\n");
                        Console.WriteLine($"\nARQUIVO NÃO SALVO\n");
                        return false;
                }
            }

            xlWorkBook.SaveAs(fileName, Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue,
            Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
            xlWorkBook.Close(true, misValue, misValue);
            xlApp.Quit();

            //o arquivo foi salvo na pasta Meus Documentos.
            Console.WriteLine($"\nConcluído. Verifique em {fullPath}\n");
            isSuccess = true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nDeu problema ao enviar resultado da extração para planilha\nERRO: {ex.Message}");
        }

        return isSuccess;
    }


    /// <summary>
    /// Realizar o serviço de extração das vagas no Google usando Selenium
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NullReferenceException"></exception>
    private async Task ExecuteServiceSeleniumExtractionAsync()
    {
        try
        {
            #region Set Options
            var parallelOptions = new ParallelOptions()
            {
                //Para testar use 1
                //MaxDegreeOfParallelism = 1,
                MaxDegreeOfParallelism = 4,
            };

            var driverOptions = new Selenium.DriverOptions();
#if DEBUG
            driverOptions = new Selenium.DriverOptions(headLess: false, disableGPU: true);
#endif
            #endregion

            await Parallel.ForEachAsync<SearchSuffixes>(SearchSuffixes.Suffixes, parallelOptions, async (suffix, _) =>
            {
                if (vacancies.Count >= vacanciesLimit)
                    return;

                var count = 0;
                var limit = suffixesLimit;
                var suffixFormated = Tool.RemoveSpecialChars(Regex.Replace((string)suffix.Suffix.Trim(), @"\s{1,}", "+")).ToLower();

                await Task.Delay(new Random().Next(1, 8) * 1000);
                var driver = driverHelper.GetDriver(suffix.Suffix, driverOptions);
                await Task.Delay(1000);

                try
                {
                    if (driver is null)
                        throw new NullReferenceException(nameof(driver));

                    var timer = Stopwatch.StartNew();
                    var triesCaptha = 3;

                    while (true)
                    {
                        if (driver is null)
                        {
                            triesCaptha--;

                            if (triesCaptha <= 0)
                                break;

                            continue;
                        }

                        driver.Navigate().GoToUrl($"https://www.google.com.br/search?q=vagas+de+emprego+{suffixFormated}");
                        await Task.Delay(2500);

                        if (triesCaptha <= 0)
                            break;

                        if (!driver.Url.Contains("sorry/index?continue"))
                            break;
                        else
                        {
                            triesCaptha--;
                            driver = driverHelper.RestartDriver(suffix.Suffix, driverOptions);
                            Console.WriteLine($"Restart ({triesCaptha}) {suffix.Suffix}");
                        }
                    }
                    

                    if (driver is null || driver.Url.Contains("sorry/index?continue"))
                    {
#if DEBUG
                        if (driver is null)
                            return;

                        Console.WriteLine($"Resolva o Capchta para Continuar {suffix.Suffix}");

                        while (true)
                        {
                            await Task.Delay(2500);
                            if (!driver.Url.Contains("sorry/index?continue"))
                                break;
                        }
#else
                        return;
#endif
                    }

                    driver.FindElement(By.XPath("//div[@class='nJXhWc']//g-link/a")).Click();
                    await Task.Delay(2500);
                    driver.FindElement(By.XPath("//div[@class='TRwkpf GbaVB yjYmLb']")).Click();
                    await Task.Delay(2500);

                    var vacancyNodeCollection = await GetVacancyNodeCollectionAsync(driver);

                    if (vacancyNodeCollection is null)
                        throw new NullReferenceException(nameof(vacancyNodeCollection));

                    for (var i = 0; i < vacancyNodeCollection.Count; i++)
                    {
                        if (timer.Elapsed.TotalMinutes == 10 || vacancies.Count >= vacanciesLimit || count >= limit)
                            break;

                        var isClassificated = ClassificationVacancy(vacancyNodeCollection[i]);

                        if (isClassificated)
                        {
                            try
                            {
                                var element = driver.FindElement(By.XPath($"{vacancyNodeCollection[i].XPath}//div[@class='BjJfJf PUpOsf']"));
                                var actions = new Actions(driver);
                                actions.MoveToElement(element).Perform();

                                await Task.Delay(600);
                                element.Click();
                                await Task.Delay(600);

                                var vacancy = ExtractionVacancy(driver, vacancyNodeCollection[i]);

                                if (vacancy != null && !vacancies.Contains(vacancy))
                                {
                                    //Verifica se a vaga é repetida pelo ID da vaga no BNE
                                    var hasID = vacancies.FirstOrDefault(x => x != null && x.ID == vacancy.ID, null);

                                    if (hasID is null)
                                    {
                                        vacancies.Add(vacancy);
                                        count++;
                                    }
                                    else
                                        Console.Write(".");
                                }
                            }
                            catch (Exception ex)
                            {
                                await Console.Out.WriteLineAsync(ex.Message);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    await Console.Out.WriteLineAsync($"Deu problema no sufixo {suffix}\n{ex.Message}");
                }

                if (driver != null)
                    driverHelper.FreeDriver(suffix.Suffix);
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Deu problema no serviço de extração\n{ex.Message}");
        }

        driverHelper.Dispose();
    }


    /// <summary>
    /// Realiza a extração da vaga ou seja, obtem os campos de Vacancy
    /// </summary>
    /// <param name="driver"></param>
    /// <param name="vacancyNode"></param>
    /// <returns>Vacancy?</returns>
    private Vacancy? ExtractionVacancy(ChromeDriver driver, HtmlNode vacancyNode)
    {
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

        if (!string.IsNullOrEmpty(urlGoogle) && listVacanciesAccess.Count > 0)
            return new Vacancy(idBNE, urlGoogle, listVacanciesAccess, DateTime.Now);

        return null;
    }


    /// <summary>
    /// Desce tudo pra baixo e obtem o HtmlNodeCollection das vagas
    /// </summary>
    /// <returns>vacancyNodeCollection</returns>
    private async Task<HtmlNodeCollection?> GetVacancyNodeCollectionAsync(ChromeDriver driver)
    {
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
