using System;

namespace DSO.ServiceLibrary.Events
{
    public class OTPProcessedEventArgs : EventArgs
    {
        public int OTPCode { get; set; }

        public string Email { get; set; }

        public  OTPProcessedEventArgs(int otpcode, string email)
        {
            OTPCode = otpcode;
            Email = email;
        }


    }


}
