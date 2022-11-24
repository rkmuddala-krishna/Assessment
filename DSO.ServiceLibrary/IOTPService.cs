using System;
using DSO.ServiceLibrary.Events;
namespace DSO.ServiceLibrary
{
    public interface IOTPService
    {
        event EventHandler<OTPProcessedEventArgs> OnOTPProcessed;
        //  delegate bool OnOTPProcessReturnDeletegate(OTPProcessedEventArgs e);
        //  event OnOTPProcessReturnDeletegate OnOTPProcessedreturn;
        (int, DateTime) CreateOTP(string email_address);
        (bool, DateTime) ValidateOTP(string email_address,int otpcode);

    }
}
