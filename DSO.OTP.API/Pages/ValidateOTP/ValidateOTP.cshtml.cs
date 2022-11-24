using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace DSO.OTP.API.Pages
{
    public class ValidateOTPModel : PageModel
    {
        private readonly ILogger<ValidateOTPModel> _logger;

        public ValidateOTPModel(ILogger<ValidateOTPModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}
