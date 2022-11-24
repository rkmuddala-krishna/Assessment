using System;
using System.Threading.Tasks;
using DSO.OTP.API.DB;
using Microsoft.EntityFrameworkCore;
using DSO.OTP.API.Models;
using System.Collections.Generic;
namespace DSO.OTP.API.Infrastructure
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly OTPDBContext _dbContext;
        private DbSet<T> _dbSet;

        private DbSet<T> DbSet => _dbSet ??= _dbContext.Set<T>();

        public Repository(OTPDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Add(T entity)
        {
            DbSet.Add(entity);
        }

        public void Delete(T entity)
        {
            DbSet.Remove(entity);
        }
        public void DeleteAll()
        {
           DbSet.RemoveRange();
           
        }
        public async Task<T> GetAsync(int id)
        {
            return await DbSet.FindAsync(id);
        }

        public async Task<T> GetAsync(params object[] filters)
        {
            return await DbSet.FromSqlRaw<T>($"SELECT * FROM [OTP] WHERE OTPCode = {(int)filters[0]} and Email = {(string)filters[1]}").FirstOrDefaultAsync();
            //return await Task.Run(() => DbSet.ToListAsync().Result.Find(x => (x as DSO.OTP.API.Models.OTP).OTPCode == (int)filters[0]));
        }
        public async Task<List<T>> GetAllAsync()
        {
            return await Task.Run(() => DbSet.ToListAsync());
        }
        public void Update(T entity)
        {
            DbSet.Update(entity);
        }
    }
}