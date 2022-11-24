using System;
using DSO.ServiceLibrary.Events;
using OtpNet;
using System.Text;
namespace DSO.ServiceLibrary
{
    public class OTPService : IOTPService
    {
        public event EventHandler<OTPProcessedEventArgs> OnOTPProcessed;

        public (int, DateTime) CreateOTP(string email_address)
        {
            var otp = GenerateOTP_AddtoDB(email_address);
            if (OnOTPProcessed != null)
            {
                OnOTPProcessed(this, new OTPProcessedEventArgs(otp.Item1, email_address));
            }
            return otp;
        }

        private (int, DateTime) GenerateOTP_AddtoDB(string email_address)
        {
            return GenerateOTP(email_address);
        }

        private (int, DateTime) GenerateOTP(string email_address)
        {
            var CurrentDateTime = DateTime.UtcNow; // UTC time format
            var totp = new Totp(Encoding.UTF8.GetBytes(email_address), step: 60, totpSize: 6); // Secret + 60 Second (valid) + returns 6 digit otp
            var totpCode = totp.ComputeTotp(CurrentDateTime); // function to generate 6 digit otp
            return (Convert.ToInt32(totpCode), CurrentDateTime);
            //return TOTPGenerator.GenerateTotp(email_address);
        }
        public (bool, DateTime) ValidateOTP(string email_address, int inputotpcode)
        {
            var totp = new Totp(Encoding.UTF8.GetBytes(email_address), step: 60, totpSize: 6);
            var CurrentDateTime = DateTime.UtcNow;
            var window = new VerificationWindow(previous: 1, future: 1);
            // function check for previously generated in the same window of 60 seconds.
            var isValidtotpCode = totp.VerifyTotp(CurrentDateTime, inputotpcode.ToString(), out long timeWindowUsed, VerificationWindow.RfcSpecifiedNetworkDelay);
            return (isValidtotpCode, CurrentDateTime);
        }


    }
}
