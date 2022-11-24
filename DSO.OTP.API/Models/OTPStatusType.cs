
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DSO.OTP.API.Models.Enum {
public enum OTPStatusType
    {
        STATUS_OTP_OK = 0, //OTP is valid
        STATUS_OTP_FAIL = 1,// OTP failed after 10 validation attempts.
        STATUS_OTP_TIMEOUT = 2, //OTP timeout after 1 minute.

    }

}

