namespace ProizvodiAPI.Application.DTOs
{
    public class KreirajProizvodDTO
    {
        public string Naziv { get; set; } = string.Empty;

        public string Opis { get; set; } = string.Empty;

        public double Cena { get; set; }

        public int Kolicina { get; set; }

        public string Kategorija { get; set; } = string.Empty;
    }
}
