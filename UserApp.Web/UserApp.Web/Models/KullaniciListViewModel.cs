using UserApp.Entities;

namespace UserApp.Web.Models
{
    public class KullaniciListViewModel
    {
        public List<Kullanici> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 8;
        public string? SearchTerm { get; set; }
        public string? SortBy { get; set; }

        public int ToplamKullanici { get; set; }
        public int DepartmanSayisi { get; set; }
        public Kullanici? SonEklenen { get; set; }

        public int TotalPages => PageSize == 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);
    }
}