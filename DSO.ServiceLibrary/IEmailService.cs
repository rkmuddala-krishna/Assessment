using System;
using DSO.ServiceLibrary.Events;
namespace DSO.ServiceLibrary
{
    public interface IEmailService
    {
        void Subscribe(IOTPService otpService);

        bool IsValidEmail(string email);

        public int EmailSentStatus { get; set; }
    }
}
