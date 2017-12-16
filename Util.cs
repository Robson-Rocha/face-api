namespace Exemplos.FaceApi
{
    using System;
    using System.IO;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json;

    public static class Util
    {
        //Obtém as configurações do arquivo appSettings.json
        private static readonly Lazy<IConfiguration> _configuration = new Lazy<IConfiguration>(() => 
            new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appSettings.json")
                .Build()
        );
        public static IConfiguration Configuration => _configuration.Value;

        //Fornece uma instância única do HttpClient -- Múltiplas instâncias podem provocar um Connection Leak
        private static readonly Lazy<HttpClient> _httpClient = new Lazy<HttpClient>();
        public static HttpClient HttpClient => _httpClient.Value;

        // Fornece uma nova instância do contexto do banco de dados de teste
        public static Db GetDb() => new Db(Configuration.GetConnectionString("HR"));

        //Adiciona as imagens de teste da pasta ./Pictures ao banco de dados
        public async static Task AddPicturesToDb() {
            // async Task<(string name, string gender)> GetRandomData()
            // {
            //     var response = await HttpClient.GetAsync("https://randomuser.me/api/?inc=name,age,gender&nat=br&noinfo");
            //     string contentString = await response.Content.ReadAsStringAsync();
            //     dynamic obj = JsonConvert.DeserializeObject(contentString);
            //     return (obj.results[0].name.first + " " + obj.results[0].name.last,
            //             obj.results[0].gender.ToString().Substring(0,1).ToLower());
            // }

            // Random rnd = new Random();

            using(Db db = GetDb())
            {
                db.Persons.RemoveRange(db.Persons);
                db.SaveChanges();

                foreach (string picturePath in Directory.EnumerateFiles(@".\Pictures"))
                {
                    // (string name, string gender) personData = await GetRandomData();
                    var person = new Person {
                        // Name = personData.name,
                        // Age = rnd.Next(20,50),
                        // Gender = personData.gender,
                        Picture = await File.ReadAllBytesAsync(picturePath)
                    };
                    db.Persons.Add(person);
                }
                await db.SaveChangesAsync();
            }
        }
    }
}