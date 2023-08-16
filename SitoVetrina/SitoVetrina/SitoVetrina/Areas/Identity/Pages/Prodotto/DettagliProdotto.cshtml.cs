using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SitoVetrina.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Web;
using System.Xml.Linq;

namespace SitoVetrina.Areas.Identity.Pages.Prodotto
{
    public class DettagliProdottoModel : PageModel
    {
        public DettagliProdottoModel(IWebHostEnvironment hostEnvironment)
        {
            this.hostEnvironment = hostEnvironment;
        }
        public IWebHostEnvironment hostEnvironment;
        public DatiProdotto prodotto = new DatiProdotto();
        public string CodiceProdotto;
        public string alert { get; set; }
        public string testoRicerca { get; set; }
        [BindProperty]
        public InputModel Input { get; set; }
        public class InputModel
        {
            [Required]
            [StringLength(100)]
            [DataType(DataType.Text)]
            [Display(Name = "NomeProdotto")]
            public string NomeProdotto { get; set; }

            [Required]
            [StringLength(255)]
            [DataType(DataType.Text)]
            [Display(Name = "Descrizione")]
            public string Descrizione { get; set; }

            [Required]
            [DataType(DataType.Upload)]
            [Display(Name = "Image")]
            public IFormFile Image { get; set; }

            [Required]
            [DataType(DataType.Currency)]
            [Display(Name = "Prezzo")]
            public string Prezzo { get; set; }
            public string elimina { get; set; }
        }
        public void OnGet()
        {
            var configuration = new ConfigurationBuilder().AddJsonFile($"appsettings.json");
            var config = configuration.Build();
            string url = HttpContext.Request.GetDisplayUrl();
            CodiceProdotto = url.Substring(url.IndexOf("id=")+3);
            string connectionString = config.GetConnectionString("SitoVetrinaContextConnection");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                FormattableString sql = $"SELECT CodiceProdotto,Nome,Descrizione,Prezzo,Immagine FROM Prodotti WHERE Prodotti.CodiceProdotto='{CodiceProdotto.ToUpper()}';";
                using (SqlCommand command = new SqlCommand(sql.ToString(), connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        { 
                            prodotto.CodiceProdotto = reader.GetGuid(0);
                            prodotto.Nome = reader.GetString(1).Replace('"','\'');
                            prodotto.Descrizione = reader.GetString(2).Replace('"','\'');
                            prodotto.Prezzo = reader.GetDecimal(3);                           
                            prodotto.Immagine = "\\Images\\" + reader.GetString(4).Replace('"','\'');
                        }
                    }
                }
                connection.Close();
            }
        }
        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                var configuration = new ConfigurationBuilder().AddJsonFile($"appsettings.json");
                var config = configuration.Build();
                string immagineNuova= "";
                if (Input.Image != null)
                {
                    var uniqueFileName = GetUniqueFileName(Input.Image.FileName);
                    var uploads = Path.Combine(this.hostEnvironment.WebRootPath, "images");
                    var filePath = Path.Combine(uploads, uniqueFileName);
                    Input.Image.CopyTo(new FileStream(filePath, FileMode.Create));
                    immagineNuova = ",Immagine='" +uniqueFileName.Replace('\'', '"') + "'";                   
                }
                string DescrizioneNuova = "";
                if (Input.Descrizione != null)
                {
                    DescrizioneNuova = ",Descrizione='" + Input.Descrizione.Replace('\'', '"') + "'";
                }
                string url = HttpContext.Request.GetDisplayUrl();
                CodiceProdotto = url.Substring(url.IndexOf("id=") + 3);
                string connectionString = config.GetConnectionString("SitoVetrinaContextConnection");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    FormattableString sql;
                    if (Input.elimina == "Elimina")
                    {
                        sql = $"DELETE FROM Prodotti WHERE CodiceProdotto='{CodiceProdotto.ToUpper()}'";
                    }
                    else
                    {
                        sql = $"UPDATE Prodotti SET Nome = '{Input.NomeProdotto.Replace('\'', '"')}'{DescrizioneNuova},Prezzo={Input.Prezzo.Replace(',', '.')+immagineNuova} WHERE Prodotti.CodiceProdotto='{CodiceProdotto.ToUpper()}'";
                    }
                    using (SqlCommand command = new SqlCommand(sql.ToString(), connection))
                    {
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
                return RedirectToPage("/");
            }
            catch (Exception ex)
            {
                alert = ex.Message;
                return Page();
            }
        }
        private string GetUniqueFileName(string fileName)
        {
            fileName = Path.GetFileName(fileName);
            return Path.GetFileNameWithoutExtension(fileName)
                      + "_"
                      + Guid.NewGuid().ToString().Substring(0, 4)
                      + Path.GetExtension(fileName);
        }
    }
}
