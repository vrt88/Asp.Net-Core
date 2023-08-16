using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace SitoVetrina.Areas.Identity.Data
{
    public class DatiProdotto
    {
        public Guid? CodiceProdotto { get; set; }
        public string? Nome { get; set; }
        public string? Descrizione { get; set; }
        public decimal? Prezzo { get; set; }
        public string? Immagine { get; set; }
    }
}
