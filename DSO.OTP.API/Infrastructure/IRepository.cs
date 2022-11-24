using System;
using System.Threading.Tasks;
using System.Collections.Generic;
namespace DSO.OTP.API.Infrastructure
{
    public interface IRepository<T> where T : class
    {
        void Add(T entity);
        void Delete(T entity);
        void Update(T entity);
        Task<T> GetAsync(int id);
        Task<T> GetAsync(params object[] filter);

        Task<List<T>> GetAllAsync();

        void DeleteAll();
    }
}