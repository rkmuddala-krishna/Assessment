using System.Linq;
using System.Threading.Tasks;
using DSO.OTP.API.Services.Interface;
using DSO.OTP.API.Infrastructure;
using DSO.OTP.API.Services;
using DSO.OTP.API.Models;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using DSO.OTP.API.Models.Enum;
using DSO.ServiceLibrary;
namespace DSO.OTP.API.Services
{
    public class OTPDBService : BaseService, IOTPDBService
    {
        private readonly IRepository<OTP.API.Models.OTP> _otpRepo;
        private readonly ILogger<OTPDBService> _logger;
        private IConfiguration _iConfiguration;
        private IEmailService _emailService;
        private IOTPService _otpService;
        public OTPDBService(ILogger<OTPDBService> logger, IUnitOfWork unitOfWork, IRepository<OTP.API.Models.OTP> otpRepo, IConfiguration configuration, IEmailService emailService, IOTPService otpService) : base(unitOfWork)
        {
            _logger = logger;
            _otpRepo = otpRepo;
            _iConfiguration = configuration;
            _emailService = emailService;
            _otpService = otpService;
            emailService.Subscribe(_otpService);
        }


        public async Task<OTP.API.Models.OTP> Get(int OTPCode)
        {
            return await _otpRepo.GetAsync(OTPCode);
        }

        public async Task<OTP.API.Models.OTP> Get(params object[] filters)
        {
            return await _otpRepo.GetAsync(filters);
        }
        public async Task<List<OTP.API.Models.OTP>> All()
        {
            return await _otpRepo.GetAllAsync();
        }
        public async Task<OTP.API.Models.OTP> Create(OTPRequestInput otpinput)
        {
            var newotpobj = _otpService.CreateOTP(otpinput.Email); // otp generation via TOTP function
            var oTPexists = _otpRepo.GetAsync(newotpobj.Item1); // get OTP from DB and validate the inputs
            Models.OTP otpmodel = new Models.OTP();
            if (oTPexists.IsCompletedSuccessfully && oTPexists.Result != null)
            {
                if (oTPexists.Result.OTPCode == newotpobj.Item1 && oTPexists.Result.Email == otpinput.Email)
                {
                    _logger.LogInformation($"Duplicate OTP request for the user:{otpinput.Email} initiated at time :{oTPexists.Result.CreatedDate}");
                    return oTPexists.Result;
                }
            }
            else
            {
                otpmodel = new Models.OTP { OTPCode = newotpobj.Item1, Email = otpinput.Email, CreatedDate = newotpobj.Item2 };
                _logger.LogInformation($"New Otp:{newotpobj.Item1} for the user:{otpinput.Email} initiated at:{newotpobj.Item2} saved to DB successfully");
                _otpRepo.Add(otpmodel);
                await UnitOfWork.SaveChangeAsync();
            }
            return otpmodel;
        }

        public async Task Update(OTP.API.Models.OTP otpinput)
        {
            _otpRepo.Update(otpinput);
            await UnitOfWork.SaveChangeAsync();
        }
        public async Task DeleteAll()
        {
            _otpRepo.DeleteAll();
            await UnitOfWork.SaveChangeAsync();
        }


        public async Task<(OTP.API.Models.OTP, string)> Validate(int OTPCode, string email_address)
        {
            var RetrycountConfig = _iConfiguration.GetValue<int>("OTPSettings:RetryCount");
            var OTPWaittimeconfig = _iConfiguration.GetValue<int>("OTPSettings:OTPWaittime");
            var OTPExpiryInMinConfig = _iConfiguration.GetValue<int>("OTPSettings:OTPExpiryInMin");
            var otpvalidatestatus = _otpService.ValidateOTP(email_address, OTPCode);
            _logger.LogInformation($"Otp: {OTPCode} requested for validation, expiration status is:{otpvalidatestatus.Item1} for the user:{email_address}");
            var otpresult = await _otpRepo.GetAsync(OTPCode);
            string otpstatus = "";
            var otpmodel = otpresult;
            int retrycount = otpmodel.OTPCheckCount;
            otpstatus = otpvalidatestatus.Item1 ? ((OTPStatusType)0).ToString() : ((OTPStatusType)2).ToString();
            if (!otpvalidatestatus.Item1)
            {
                _logger.LogInformation($"Otp: {OTPCode} requested for validation expiry?:{!otpvalidatestatus.Item1} for the user:{email_address}");
                //await Task.Delay(OTPWaittimeconfig);
            }

            if (retrycount > RetrycountConfig)
            {
                _logger.LogInformation($"Otp: {OTPCode} requested for validation exceeded max try count: {retrycount} for the user:{email_address}");
                otpstatus = ((OTPStatusType)1).ToString();
            }

            return (otpresult, otpstatus);
        }
    }
}