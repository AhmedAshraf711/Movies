using MVC_CRUD_OPERATION.Models;
using System.Collections.Generic;
using System.Linq;

namespace MVC_CRUD_OPERATION.Repository
{
    public class GenreRepo : IGeneraRepo
    {
        ApplicationDbContext _context;
        public GenreRepo(ApplicationDbContext context)
        {
          _context = context;
        }

        public List<Genre> GetAllOrderdByName()
        {
            return _context.Genres.OrderByDescending(g=>g.Name).ToList();
        }

        public Genre GetBy(int id)
        {
           return _context.Genres.FirstOrDefault(g => g.Id == id); 
        }
 
    }
}
