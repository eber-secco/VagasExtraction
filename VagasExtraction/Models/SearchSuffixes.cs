using OpenQA.Selenium.DevTools.V116.SystemInfo;
using VagasExtraction.Enums;

namespace VagasExtraction.Models
{
    public class SearchSuffixes
    {
        public TypesSearchEnum Type;
        public string Suffix;

        public SearchSuffixes(TypesSearchEnum type, string suffix)
        {
            Type = type;
            Suffix = suffix;
        }

        public static IEnumerable<SearchSuffixes> Suffixes
        {
            get => new List<SearchSuffixes>
            {
                new SearchSuffixes(TypesSearchEnum.Regions, "Curitiba, PR"),
                new SearchSuffixes(TypesSearchEnum.Regions, "Ijuí, RS"),
                new SearchSuffixes(TypesSearchEnum.Regions, "Joinville - Pirabeiraba, Joinville - SC"),
                new SearchSuffixes(TypesSearchEnum.Regions, "Piracicaba, SP"),
                new SearchSuffixes(TypesSearchEnum.Regions, "Rio de Janeiro, RJ"),
                new SearchSuffixes(TypesSearchEnum.Regions, "São Luís, MA"),
                new SearchSuffixes(TypesSearchEnum.Regions, "São Paulo, SP"),
                new SearchSuffixes(TypesSearchEnum.Regions, "Ananindeua, PA"),
                new SearchSuffixes(TypesSearchEnum.Regions, "Bento Gonçalves, RS"),
                new SearchSuffixes(TypesSearchEnum.Regions, "Caxias do Sul - Galópolis, Caxias do Sul - RS"),
                new SearchSuffixes(TypesSearchEnum.Regions, "Itaguaí, RJ"),
                new SearchSuffixes(TypesSearchEnum.Regions, "Manaus, AM"),
                new SearchSuffixes(TypesSearchEnum.Regions, "Palmares, PE"),
                new SearchSuffixes(TypesSearchEnum.Regions, "Paulínia - João Aranha, Paulínia - SP"),
                new SearchSuffixes(TypesSearchEnum.Regions, "Poços de Caldas - Rabelo, Poços de Caldas - MG"),
                new SearchSuffixes(TypesSearchEnum.Regions, "Vila Velha - Ibes, Vila Velha - ES"),
                new SearchSuffixes(TypesSearchEnum.Regions, "Anchieta, ES"),
                new SearchSuffixes(TypesSearchEnum.Regions, "Angra dos Reis, RJ"),
                new SearchSuffixes(TypesSearchEnum.Regions, "Araucária, PR"),
                new SearchSuffixes(TypesSearchEnum.Regions, "Assis, SP"),
                new SearchSuffixes(TypesSearchEnum.Regions, "Atibaia, SP"),
                new SearchSuffixes(TypesSearchEnum.Regions, "Barcarena, PA"),
                new SearchSuffixes(TypesSearchEnum.Regions, "Cambé, PR"),
                new SearchSuffixes(TypesSearchEnum.Regions, "Campina Grande, PB"),
                new SearchSuffixes(TypesSearchEnum.Regions, "Campinas, SP"),
                new SearchSuffixes(TypesSearchEnum.Regions, "Catalão, GO"),
                new SearchSuffixes(TypesSearchEnum.Regions, "Caucaia - Jurema, Caucaia - CE"),
                new SearchSuffixes(TypesSearchEnum.Regions, "Congonhas, MG"),
                new SearchSuffixes(TypesSearchEnum.Regions, "Divinópolis, MG"),
                new SearchSuffixes(TypesSearchEnum.Regions, "Fortaleza - Zone 1, Fortaleza - CE"),
                new SearchSuffixes(TypesSearchEnum.Regions, "Gramado, RS"),
                new SearchSuffixes(TypesSearchEnum.Regions, "Gravataí, RS"),
                new SearchSuffixes(TypesSearchEnum.Regions, "Guarulhos, SP"),
                new SearchSuffixes(TypesSearchEnum.Regions, "Ipiranga, PR"),
                new SearchSuffixes(TypesSearchEnum.Regions, "Itapevi, SP"),
                new SearchSuffixes(TypesSearchEnum.Regions, "Itaúna, MG"),
                new SearchSuffixes(TypesSearchEnum.Regions, "Jaraguá do Sul - Barra do Rio do Cerro, Jaraguá do Sul - SC"),
                new SearchSuffixes(TypesSearchEnum.Regions, "João Pessoa, PB"),
                new SearchSuffixes(TypesSearchEnum.Regions, "Lages, SC"),
                new SearchSuffixes(TypesSearchEnum.Regions, "Lavras, MG"),
                new SearchSuffixes(TypesSearchEnum.Regions, "Londrina, PR"),
                new SearchSuffixes(TypesSearchEnum.Regions, "Macaé, RJ"),
                new SearchSuffixes(TypesSearchEnum.Regions, "Mogi das Cruzes - Brás Cubas, Mogi das Cruzes - SP"),
                new SearchSuffixes(TypesSearchEnum.Regions, "Montes Claros, MG"),
                new SearchSuffixes(TypesSearchEnum.Regions, "Navegantes, SC"),
                new SearchSuffixes(TypesSearchEnum.Regions, "Nova Friburgo, RJ"),
                new SearchSuffixes(TypesSearchEnum.Regions, "Nova Lima, MG"),
                new SearchSuffixes(TypesSearchEnum.Regions, "Nova Mutum, MT"),
                new SearchSuffixes(TypesSearchEnum.Regions, "Novo Gama, GO"),
                new SearchSuffixes(TypesSearchEnum.Regions, "Rio Grande Do Sul, SC"),
                new SearchSuffixes(TypesSearchEnum.Regions, "Brasil"),
                new SearchSuffixes(TypesSearchEnum.Regions, "Norte do Brasil"),
                new SearchSuffixes(TypesSearchEnum.Regions, "Sul do Brasil"),
                new SearchSuffixes(TypesSearchEnum.Regions, "Leste do Brasil"),
                new SearchSuffixes(TypesSearchEnum.Regions, "Oeste do Brasil"),
                new SearchSuffixes(TypesSearchEnum.Functions, "TI"),
                new SearchSuffixes(TypesSearchEnum.Functions, "Vendedor"),
                new SearchSuffixes(TypesSearchEnum.Functions, "Técnico"),
                new SearchSuffixes(TypesSearchEnum.Functions, "Manual"),
                new SearchSuffixes(TypesSearchEnum.Functions, "Estagio"),
                new SearchSuffixes(TypesSearchEnum.Functions, "Auxiliar"),
                new SearchSuffixes(TypesSearchEnum.Functions, "Junior"),
                new SearchSuffixes(TypesSearchEnum.Functions, "Pleno"),
                new SearchSuffixes(TypesSearchEnum.Functions, "Senior"),
                new SearchSuffixes(TypesSearchEnum.Functions, "Medicina"),
                new SearchSuffixes(TypesSearchEnum.Functions, "Domestica ou Domestico"),
                new SearchSuffixes(TypesSearchEnum.Functions, "Rural"),
                new SearchSuffixes(TypesSearchEnum.Functions, "Veterianio ou Veterinaria"),
                new SearchSuffixes(TypesSearchEnum.Functions, "Escritorio"),
                new SearchSuffixes(TypesSearchEnum.Functions, "Home Office ou Remoto"),
                new SearchSuffixes(TypesSearchEnum.Functions, "Presencial"),
                new SearchSuffixes(TypesSearchEnum.Functions, "Caixa"),
                new SearchSuffixes(TypesSearchEnum.Functions, "Mercado"),
                new SearchSuffixes(TypesSearchEnum.Functions, "Transporte"),
                new SearchSuffixes(TypesSearchEnum.Functions, "Relacionado a Arte"),
            };
        }
    }
}
