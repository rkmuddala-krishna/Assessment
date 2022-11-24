using System;
using DSO.OTP.API.Models;
namespace DSO.OTP.API.Models
{
    public class OTPRequestInput
    {

        public string Email { get; set; }

        public  int OTPCode { get;set;} 

        public int OTPCheckCount { get; set; }
    }
    
}
