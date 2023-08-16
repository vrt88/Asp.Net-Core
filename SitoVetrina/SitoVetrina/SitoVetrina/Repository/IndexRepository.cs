using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata;
using SitoVetrina.Areas.Identity.Data;
using SitoVetrina.Context;
using SitoVetrina.Contracts;

namespace SitoVetrina.Repository
{
    public class IndexRepository : IIndexModel
    {
        public string testoRicerca { get; set; }
        public List<DatiProdotto> ListProdotti = new List<DatiProdotto>();
        private readonly DapperContext _context;
        public IndexRepository(DapperContext context, string url)
        {
            _context = context;
            VisualizzaProdotti(url);
        }
        public void VisualizzaProdotti(string url)
        {
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
            using (var connection = _context.CreateConnection())
            {
                FormattableString sql;
                if (filtro)
                {
                    sql = $"SELECT CodiceProdotto,Nome,Prezzo,Immagine FROM Prodotti WHERE Nome LIKE '%{parametroRicerca}%'";
                }
                else
                {
                    sql = $"SELECT CodiceProdotto,Nome,Prezzo,Immagine FROM Prodotti";
                }
                var prodotti = connection.Query<DatiProdotto>(sql.ToString());
                ListProdotti = prodotti.ToList();
            }
        }
    }
}
