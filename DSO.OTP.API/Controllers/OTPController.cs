using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DSO.ServiceLibrary;
using Microsoft.Extensions.Configuration;
using DSO.OTP.API.Models;
using DSO.OTP.API.Models.Enum;
using DSO.OTP.API.Services.Interface;
namespace DSO.OTP.API.Controllers
{
    [ApiController]
    [Route("api/OTP")]
    public class OTPController : ControllerBase
    {

        private IEmailService _emailService;

        private IOTPDBService _OTPDBService;
        private readonly ILogger<OTPController> _logger;
        private IConfiguration _iConfiguration;
        public OTPController(ILogger<OTPController> logger, IEmailService emailService, IConfiguration configuration, IOTPDBService oTPDBService)
        {
            _logger = logger;
            _iConfiguration = configuration;
            _emailService = emailService;
            _OTPDBService = oTPDBService;
        }

        [HttpGet("Listall")]
        public async Task<IActionResult> get()
        {
            var otpdbresults = _OTPDBService.All().Result;
            var currentdate = DateTime.UtcNow;
            var otpresultsexpired = otpdbresults.Select(x => new { OTPCode = CheckDigitLength(x.OTPCode), x.Email, x.CreatedDate, currentdate = currentdate, expired = (currentdate - x.CreatedDate).TotalSeconds > 60 });
            return await Task.FromResult(Ok(otpresultsexpired));

        }
        [HttpPost("ValidateOTP")]
        public async Task<IActionResult> ValidateOTP([FromBody] OTPRequestInput otpinput)
        {
            if (otpinput.OTPCode.Equals(string.Empty))
                return BadRequest(new { status = StatusCode(400), StatusText = "OTP is required" });//BadRequest(string.Format("OTP is required"));

            var otpresult = _OTPDBService.Validate(otpinput.OTPCode, otpinput.Email);

            // if (!otpresult.IsCompletedSuccessfully)
            //     return BadRequest(new { status = StatusCode(400), StatusText = "Invalid OTP" });//BadRequest(string.Format("OTP is required"));

            var otpmodel = otpresult.Result.Item1;
            if (otpresult.Result.Item2 == ((OTPStatusType)0).ToString())
            {
                try
                {
                    otpmodel.OTPCheckCount++;
                    await _OTPDBService.Update(otpmodel);
                }
                catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
                {

                    _logger.LogError($"Error updating record to DB : {ex.Data}");
                }
            }

            var result = new { status = StatusCode(200), data = new { OTP_Status = otpresult.Result.Item2, otp_createddate=otpresult.Result.Item1.CreatedDate, emailaddress = otpmodel.Email } };
            return await Task.FromResult(Ok(result));
        }


        [HttpPost("GenerateOTP")]
        public async Task<IActionResult> GenerateOTP([FromBody] OTPRequestInput emailinput)
        {
            if (emailinput.Email.Equals(string.Empty))
                //return BadRequest(string.Format("Email address is required"));
                return BadRequest(new { status = StatusCode(400), StatusText = "Email address is required" });

            if (!_emailService.IsValidEmail(emailinput.Email))
            {
                //return BadRequest(string.Format("Invalid Email address"));
                return BadRequest(new { status = StatusCode(400), StatusText = "Invalid Email address" });

            }
            var otp = _OTPDBService.Create(emailinput).Result;
            var otpresult = _OTPDBService.Get(otp.OTPCode).Result;

            var result = new { status = StatusCode(200), data = new { otp = CheckDigitLength(otpresult.OTPCode), emailsentstatus = _emailService.EmailSentStatus, createddate = otpresult.CreatedDate } };
            return await Task.FromResult(Ok(result));
        }

        [HttpPost("PurgeOTP")]
        public async Task<IActionResult> PurgeOTP()
        {
            var otp = _OTPDBService.DeleteAll();
            var result = new { status = StatusCode(200), data = new { status = "Deleted successfully" } };
            return await Task.FromResult(Ok(result));
        }

        private string CheckDigitLength(int otpcode)
        {
            return Math.Floor(Math.Log10(otpcode) + 1) < 6 ? otpcode.ToString().PadLeft(6, '0') : otpcode.ToString();
        }


    }

    public enum OTPStatus
    {
        STATUS_OTP_OK = 0, //email containing OTP has been sent successfully.
        STATUS_OTP_FAIL = 1,//email address does not exist or sending to the email has failed.
        STATUS_OTP_TIMEOUT = 2, //email address is invalid.

    }
}
