using UserApp.Entities.Dtos;

namespace UserApp.Web.Models
{
    public class KullaniciListViewModel
    {
        public List<KullaniciListItemDto> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 8;
        public string? SearchTerm { get; set; }
        public string? SortBy { get; set; }

        public int ToplamKullanici { get; set; }
        public int DepartmanSayisi { get; set; }
        public string? SonEklenenAdSoyad { get; set; }

        public int TotalPages => PageSize == 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);
    }
}