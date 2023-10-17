
namespace VagasExtraction.Models;

public class Vacancy
{
    public Vacancy(string id, string url, List<VacancyAccess> vacanciesAccess, DateTime searchDate)
    {
        ID = id;
        Url = url;
        VacanciesAccess = vacanciesAccess;
        SearchDate = searchDate;
    }

    public string ID { get; set; }
    public string Url { get; set; }
    public List<VacancyAccess> VacanciesAccess { get; set; }
    public DateTime SearchDate { get; set; }
}

public class VacancyAccess
{
    public VacancyAccess(string nameAccess, string url)
    {
        NameAccess = nameAccess;
        Url = url;
    }

    public string NameAccess { get; set; }
    public string Url { get; set; }
}
