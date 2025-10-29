using CsvHelper.Configuration.Attributes;

namespace PedidoKit.Models
{
    public class Kit
    {
        [Ignore]
        public int Id { get; set; }

        [Name("KIT")]
        public string Nome { get; set; } = string.Empty;

        [Name("DESCRICAO")]
        public string Descricao { get; set; } = string.Empty;

        [Name("PECAS")]
        public string Pecas { get; set; } = string.Empty;

        [Name("TOTAL")]
        public string Total { get; set; } = string.Empty;
    }
}
