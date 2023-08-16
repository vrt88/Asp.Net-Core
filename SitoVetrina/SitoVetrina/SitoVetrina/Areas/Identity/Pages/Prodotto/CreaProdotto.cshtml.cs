using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Hosting.Internal;
using SitoVetrina.Models;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Net;
using System.Xml.Linq;

namespace SitoVetrina.Areas.Identity.Pages.Prodotto
{
    public class CreaProdottoModel : PageModel
    {
        public CreaProdottoModel(IWebHostEnvironment hostEnvironment)
        {
            this.hostEnvironment = hostEnvironment;
        }
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
            public IFormFile MyImage { set; get; }

            [Required]
            [DataType(DataType.Currency)]
            [Display(Name = "Prezzo")]
            public string Prezzo { get; set; }
        }
        [BindProperty]
        public InputModel Input { get; set; }

        public string alert { get; set; }
        public IWebHostEnvironment hostEnvironment;
        public string testoRicerca { get; set; }
        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                var configuration = new ConfigurationBuilder().AddJsonFile($"appsettings.json");
                var config = configuration.Build();
                var uniqueFileName = GetUniqueFileName(Input.MyImage.FileName);
                var uploads = Path.Combine(this.hostEnvironment.WebRootPath, "images");
                var filePath = Path.Combine(uploads, uniqueFileName);
                Input.MyImage.CopyTo(new FileStream(filePath, FileMode.Create));
                string connectionString = config.GetConnectionString("SitoVetrinaContextConnection");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    FormattableString sql = $"INSERT INTO Prodotti(Nome,Descrizione,Prezzo,Immagine) VALUES ('{ Input.NomeProdotto.Replace('\'', '"')}','{ Input.Descrizione.Replace('\'', '"')}',{ Input.Prezzo.Replace(',', '.')},'{ uniqueFileName.Replace('\'', '"')}');";
                    using (SqlCommand command = new SqlCommand(sql.ToString(), connection))
                    {
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
                alert = "Prodotto creato con successo";
            }
            catch(Exception ex)
            {
                alert = ex.Message;
            }
            return Page();
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
