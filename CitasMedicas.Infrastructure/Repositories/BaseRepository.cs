using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CitasMedicas.Core.Interfaces;
using CitasMedicas.Core.Entities;
using CitasMedicas.Infrastructure.Data;
using System;

namespace CitasMedicas.Infrastructure.Repositories
{
    public class BaseRepository<T>
       : IBaseRepository<T> where T : BaseEntity
    {
        protected readonly CitasMedicasContext _context;
        protected readonly DbSet<T> _entities;

        public BaseRepository(CitasMedicasContext context)
        {
            _context = context;
            _entities = context.Set<T>();
        }
        public async Task<IEnumerable<T>> GetAll()
        {
            return await _entities.ToListAsync();
        }

        public async Task<T> GetById(int id)
        {
            return await _entities.FindAsync(id);
        }
        public async Task Add(T entity)
        {
            await _entities.AddAsync(entity);
        }


        public async Task Update(T entity)
        {
            _entities.Update(entity);
        }

        public async Task Delete(int id)
        {
            T entity = await GetById(id);
            _entities.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}

