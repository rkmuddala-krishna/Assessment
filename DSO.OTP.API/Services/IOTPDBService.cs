using System.Threading.Tasks;
using DSO.OTP.API.Models;
using System.Collections.Generic;
using DSO.ServiceLibrary;

namespace DSO.OTP.API.Services.Interface
{
    public interface IOTPDBService
    {
        public Task<OTP.API.Models.OTP> Get(int id);

        public Task<OTP.API.Models.OTP> Get(params object[] filters);
        public Task<List<OTP.API.Models.OTP>> All();
        public Task<OTP.API.Models.OTP> Create(OTPRequestInput userId);
        public Task Update(OTP.API.Models.OTP otpinput);
        public Task<(OTP.API.Models.OTP, string)> Validate(int otpinput, string email_address);
        public Task DeleteAll();

        
    }
}