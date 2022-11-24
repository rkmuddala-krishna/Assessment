using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace DSO.OTP.API.Pages
{
    public class GenerateOTPModel : PageModel
    {
        private readonly ILogger<GenerateOTPModel> _logger;

        public GenerateOTPModel(ILogger<GenerateOTPModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}
