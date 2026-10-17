namespace ProizvodiAPI.Domain.Models
{
    public class Proizvod
    {
        public int Id { get; set; }

        public string Naziv { get; set; } = string.Empty;

        public string Opis { get; set; } = string.Empty;

        public double Cena { get; set; }

        public int Kolicina { get; set; }

        public string Kategorija { get; set; } = string.Empty;

        public DateTime KreiranDatum { get; set; }
    }
}
