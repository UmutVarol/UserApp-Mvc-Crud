using System.Collections.Generic;
using System.Linq;
using UserApp.Data;
using UserApp.Entities;

namespace UserApp.Services
{
    public class UserService
    {
        private readonly AppDbContext _context;
        public UserService(AppDbContext context) { _context = context; }
        
        public List<Kullanici> GetAll() => _context.Kullanicilar.ToList();
        
        public void Add(Kullanici kullanici)
        {
            _context.Kullanicilar.Add(kullanici);
            _context.SaveChanges();
        }
        
        public Kullanici GetById(int id) => _context.Kullanicilar.Find(id);

        public void Update(Kullanici kullanici)
        {
            _context.Kullanicilar.Update(kullanici);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var kullanici = GetById(id);
            if (kullanici != null)
            {
                _context.Kullanicilar.Remove(kullanici);
                _context.SaveChanges();
            }
        }
    }
}