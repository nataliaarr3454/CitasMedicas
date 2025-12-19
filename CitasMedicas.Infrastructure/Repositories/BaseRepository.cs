using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CitasMedicas.Core.Interfaces;
using CitasMedicas.Core.Entities;
using CitasMedicas.Infrastructure.Data;
using System.Linq;

namespace CitasMedicas.Infrastructure.Repositories
{
    public class BaseRepository<T>
       : IBaseRepository<T> where T : class
    {
        protected readonly CitasMedicasContext _context;
        protected readonly DbSet<T> _entities;

        public BaseRepository(CitasMedicasContext context)
        {
            _context = context;
            _entities = context.Set<T>();
        }
        public virtual async Task<IEnumerable<T>> GetAll()
        {
            return await _entities.ToListAsync();
        }

        public virtual async Task<T?> GetById(int id)
        {
            return await _entities.FindAsync(id);
        }

        public virtual async Task Add(T entity)
        {
            await _entities.AddAsync(entity);
        }

        public virtual async Task Update(T entity)
        {
            _entities.Update(entity);
            await _context.SaveChangesAsync();
        }

        public virtual async Task Delete(int id)
        {
            var entity = await GetById(id);
            if (entity != null)
            {
                _entities.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public virtual async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
