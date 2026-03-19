namespace WineryAPI.DTOs
{
    public class ParcelaDto
    {
        public int Idp { get; set; }
        public int Brojcokota { get; set; }
        public decimal Povrsina { get; set; }
        public string Nazivparcele { get; set; } = string.Empty;
        public int VinogradIdv { get; set; }
        public int? SortagrozdjaIdsrt { get; set; }
        public string? SortaNaziv { get; set; }
    }

    public class CreateParcelaDto
    {
        public int Brojcokota { get; set; }
        public decimal Povrsina { get; set; }
        public string Nazivparcele { get; set; } = string.Empty;
        public int? SortagrozdjaIdsrt { get; set; }
    }

    public class UpdateParcelaDto
    {
        public int Brojcokota { get; set; }
        public decimal Povrsina { get; set; }
        public string Nazivparcele { get; set; } = string.Empty;
        public int? SortagrozdjaIdsrt { get; set; }
    }
}

