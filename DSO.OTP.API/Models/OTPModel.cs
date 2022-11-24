using System;
namespace DSO.OTP.API.Models
{
    public class OTP : EntityBase
    {

        public int ID { get; set; }
        public  override int OTPCode { get; set; }

        public string Email { get; set; }
        public override DateTime CreatedDate { get; set; }
        public int OTPCheckCount { get; set; }
    }
}