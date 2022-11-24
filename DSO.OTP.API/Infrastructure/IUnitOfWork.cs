using System;
using System.Threading.Tasks;

namespace DSO.OTP.API.Infrastructure
{
    public interface IUnitOfWork    
    {   
        Task<int> SaveChangeAsync();   
		Task<TResult> ExecuteTransactionAsync<TResult>(Func<Task<TResult>> func);
    }  
}