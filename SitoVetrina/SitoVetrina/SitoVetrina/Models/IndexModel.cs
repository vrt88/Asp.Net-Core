using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SitoVetrina.Areas.Identity.Data;
using SitoVetrina.Contracts;
using System.Configuration;
using System.Security.Policy;
using ConfigurationManager = Microsoft.Extensions.Configuration.ConfigurationManager;

namespace SitoVetrina.Models
{
    public class IndexModel : IIndexModel
    {
        public string testoRicerca { get; set; }
        public List<DatiProdotto> ListProdotti = new List<DatiProdotto>();
        public List<string> Immagini = new List<string>();
        public IndexModel(string url)
        {
            VisualizzaProdotti(url);
        }
        public void VisualizzaProdotti(string url)
        {
            var configuration = new ConfigurationBuilder().AddJsonFile($"appsettings.json");
            var config = configuration.Build();
            bool filtro = false;
            string parametroRicerca = "";
            if (url.Contains("?testoRicerca="))
            {
                parametroRicerca = url.Substring(url.IndexOf("?testoRicerca=") + 14);
                if (parametroRicerca.Count() >= 3)
                {
                    filtro = true;
                }
            }
            string connectionString = config.GetConnectionString("SitoVetrinaContextConnection");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                FormattableString sql;
                connection.Open();
                if (filtro)
                {
                    sql = $"SELECT CodiceProdotto,Nome,Prezzo,Immagine FROM Prodotti WHERE Nome LIKE '%{parametroRicerca}%'";
                }
                else
                {
                    sql = $"SELECT CodiceProdotto,Nome,Prezzo,Immagine FROM Prodotti";
                }
                using (SqlCommand command = new SqlCommand(sql.ToString(), connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DatiProdotto prodotto = new DatiProdotto();
                            prodotto.CodiceProdotto = reader.GetGuid(0);
                            prodotto.Nome = reader.GetString(1).Replace('"', '\'');
                            prodotto.Prezzo = reader.GetDecimal(2);
                            prodotto.Immagine = reader.GetString(3).Replace('"', '\'');
                            Immagini.Add(prodotto.Immagine);
                            ListProdotti.Add(prodotto);
                        }
                    }
                }
                if (!filtro)
                {
                    ControllaImmagini();
                }
                connection.Close();
            }
        }
        public void ControllaImmagini()
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo("wwwroot\\Images\\");
                foreach (FileInfo file in dir.GetFiles())
                {
                    if (!Immagini.Contains(file.Name))
                    {
                        file.Refresh();
                        file.Delete();
                    }
                }
            }
            catch
            {
            }
        }
    }
}
