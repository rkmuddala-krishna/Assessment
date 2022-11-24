using System;
using DSO.ServiceLibrary.Events;
using System.Text.RegularExpressions;
namespace DSO.ServiceLibrary
{
    public class EmailService : IEmailService
    {

        public int EmailSentStatus { get; set; }
        public void Subscribe(IOTPService otpService)
        {
            otpService.OnOTPProcessed += SendEmail;
            //otpService.OnOTPProcessedreturn += SendEmails;
        }

        public void SendEmail(object sender, OTPProcessedEventArgs e)
        {
            Console.WriteLine($"You OTP Code is {e.OTPCode}. The code is valid for 1 minute");
            EmailSentStatus = (int)EmailSentStatusType.STATUS_EMAIL_OK;

        }

       
        public bool IsValidEmail(string email)
        {
            var Regex_subdomainemail = new Regex(@"^\w+([-+.']\w+)*@\w+([-.]{0,0}\w+)\/*.dso.org.sg$"); //@*.dso.org.sg only one subdomain allowed
            var Regex_domainemail = new Regex(@"^\w+([-+.']\w+)*@dso.org.sg$"); //dso.org.sg
            return (Regex_subdomainemail.IsMatch(email) || Regex_domainemail.IsMatch(email));
        }
    }

    public enum EmailSentStatusType
    {
        STATUS_EMAIL_OK = 0, //email containing OTP has been sent successfully.
        STATUS_EMAIL_FAIL = 1,//email address does not exist or sending to the email has failed.
        STATUS_EMAIL_INVALID = 2, //email address is invalid.

    }
}
