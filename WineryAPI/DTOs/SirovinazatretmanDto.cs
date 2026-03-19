namespace WineryAPI.DTOs
{
    public class SirovinazatretmanDto
    {
        public int Idsir { get; set; }
        public string Naziv { get; set; } = string.Empty;
        public int BrojKoriscenjaUTretmanima { get; set; }
    }

    public class CreateSirovinazatretmanDto
    {
        public string Naziv { get; set; } = string.Empty;
    }

    public class UpdateSirovinazatretmanDto
    {
        public string Naziv { get; set; } = string.Empty;
    }
}

