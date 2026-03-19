namespace WineryAPI.DTOs
{
    public class SortagrozdjaDto
    {
        public int Idsrt { get; set; }
        public string Nazivsorte { get; set; } = string.Empty;
        public string Bojasorte { get; set; } = string.Empty;
        public string Porijeklosorte { get; set; } = string.Empty;
        public string Periodsazr { get; set; } = string.Empty;
        public decimal Kiselost { get; set; }
        public int BrojParcela { get; set; }
    }

    public class SortagrozdjaDetailDto
    {
        public int Idsrt { get; set; }
        public string Nazivsorte { get; set; } = string.Empty;
        public string Bojasorte { get; set; } = string.Empty;
        public string Porijeklosorte { get; set; } = string.Empty;
        public string Periodsazr { get; set; } = string.Empty;
        public decimal Kiselost { get; set; }
        public List<ParcelaInfoDto> Parcele { get; set; } = new();
    }

    public class CreateSortagrozdjaDto
    {
        public string Nazivsorte { get; set; } = string.Empty;
        public string Bojasorte { get; set; } = string.Empty;
        public string Porijeklosorte { get; set; } = string.Empty;
        public string Periodsazr { get; set; } = string.Empty;
        public decimal Kiselost { get; set; }
    }

    public class UpdateSortagrozdjaDto
    {
        public string Nazivsorte { get; set; } = string.Empty;
        public string Bojasorte { get; set; } = string.Empty;
        public string Porijeklosorte { get; set; } = string.Empty;
        public string Periodsazr { get; set; } = string.Empty;
        public decimal Kiselost { get; set; }
    }

    public class ParcelaInfoDto
    {
        public int Idp { get; set; }
        public string Nazivparcele { get; set; } = string.Empty;
        public decimal Povrsina { get; set; }
        public int Brojcokota { get; set; }
        public string VinogradNaziv { get; set; } = string.Empty;
        public int VinogradIdv { get; set; }
    }
}

