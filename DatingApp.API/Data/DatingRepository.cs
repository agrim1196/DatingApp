using System.Collections.Generic;
using System.Threading.Tasks;
using DatingApp.API.Model;
using Microsoft.EntityFrameworkCore;
using System.Linq;
namespace DatingApp.API.Data
{
    public class DatingRepository : IDatingRepository
    {
 
          private readonly DataContext _context;
        public DatingRepository(DataContext context)
        {
          _context=context;
        }

       

        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);

        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public async Task<User> GetUser(int id)
        {
            var res = await _context.Users.Include(p => p.Photos).FirstOrDefaultAsync(u => u.Id == id);
            return res;
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
                 var res = await _context.Users.Include(p => p.Photos).ToListAsync();
                 return res;
        }

        public async Task<bool> SaveUsers()
        {
               return await _context.SaveChangesAsync() > 0 ;
        }
       
       
        public async Task<Photo> GetPhoto(int id)
        {
            var photo = await _context.Photos.FirstOrDefaultAsync(p => p.Id == id);

            return photo;
        }

        
        public async Task<Photo> GetMainPhotoForUser(int userId)
        {
           return await _context.Photos.Where(p=> p.UserId == userId).FirstOrDefaultAsync(p => p.IsMain);
        }

        
    }
}