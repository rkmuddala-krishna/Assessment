using System;
using System.Diagnostics;
using System.Net;
using System.Security.Cryptography;
using System.Text;
namespace DSO.ServiceLibrary
{
    public static class TOTPGenerator
    {
        private static readonly DateTime _unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private static TimeSpan _timestep = TimeSpan.FromSeconds(60);
        private static readonly Encoding _encoding = new UTF8Encoding(false, true);

        private static int ComputeTotp(HashAlgorithm hashAlgorithm, ulong timestepNumber, string modifier)
        {
            // # of 0's = length of pin
            const int Mod = 1000000;

            // See https://tools.ietf.org/html/rfc4226
            // We can add an optional modifier
            var timestepAsBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((long)timestepNumber));
            var hash = hashAlgorithm.ComputeHash(ApplyModifier(timestepAsBytes, modifier));

            // Generate DT string
            var offset = hash[hash.Length - 1] & 0xf;
            var binaryCode = (hash[offset] & 0x7f) << 24
                             | (hash[offset + 1] & 0xff) << 16
                             | (hash[offset + 2] & 0xff) << 8
                             | (hash[offset + 3] & 0xff);

            int otpcode = binaryCode % Mod;
            if (Math.Floor(Math.Log10(otpcode) + 1) < 6)
            {
                Console.WriteLine($"You OTP Code is {otpcode} is less than 6");
            }
            return otpcode;
        }

        private static byte[] ApplyModifier(byte[] input, string modifier)
        {
            if (string.IsNullOrEmpty(modifier))
            {
                return input;
            }

            var modifierBytes = _encoding.GetBytes(modifier);
            var combined = new byte[checked(input.Length + modifierBytes.Length)];
            Buffer.BlockCopy(input, 0, combined, 0, input.Length);
            Buffer.BlockCopy(modifierBytes, 0, combined, input.Length, modifierBytes.Length);
            return combined;
        }

        // More info: https://tools.ietf.org/html/rfc6238#section-4
        private static ulong GetCurrentTimeStepNumber(DateTime utcnow)
        {
            var delta = utcnow - _unixEpoch;
            return (ulong)(delta.Ticks / _timestep.Ticks);
        }


        /// <summary>
        /// Generates TOTP for the specified <paramref name="securityToken"/>.
        /// </summary>
        /// <param name="securityToken">The security token to generate code.</param>
        /// <param name="modifier">The modifier.</param>
        /// <returns>The generated code.</returns>
        public static (int, DateTime) GenerateTotp(string securityToken, string modifier = null) => GenerateTotp(Encoding.Unicode.GetBytes(securityToken), modifier);

        /// <summary>
        /// Validates the TOTP for the specified <paramref name="securityToken"/>.
        /// </summary>
        /// <param name="securityToken">The security token for verifying.</param>
        /// <param name="code">The code to validate.</param>
        /// <param name="modifier">The modifier</param>
        /// <returns><c>True</c> if validate succeed, otherwise, <c>false</c>.</returns>
        public static (bool, DateTime) ValidateTotp(string securityToken, int code, string modifier = null) => ValidateTotp(Encoding.Unicode.GetBytes(securityToken), code, modifier);

        /// <summary>
        /// Generates TOTP for the specified <paramref name="securityToken"/>.
        /// </summary>
        /// <param name="securityToken">The security token to generate TOTP.</param>
        /// <param name="modifier">The modifier.</param>
        /// <returns>The generated code.</returns>
        public static (int, DateTime) GenerateTotp(byte[] securityToken, string modifier = null)
        {
            if (securityToken == null)
            {
                throw new ArgumentNullException(nameof(securityToken));
            }

            // Allow a variance of no greater than 90 seconds in either direction
            var utcnow = DateTime.UtcNow;
            var currentTimeStep = GetCurrentTimeStepNumber(utcnow);

            using (var hashAlgorithm = new HMACSHA1(securityToken))
            {

                int otpcode = ComputeTotp(hashAlgorithm, currentTimeStep, modifier);
                //Console.WriteLine($"You OTP Code is {otpcode} generated at {currentTimeStep}");
                return (otpcode, utcnow);
            }
        }

        /// <summary>
        /// Validates the TOTP for the specified <paramref name="securityToken"/>.
        /// </summary>
        /// <param name="securityToken">The security token for verifying.</param>
        /// <param name="code">The TOTP to validate.</param>
        /// <param name="modifier">The modifier</param>
        /// <returns><c>True</c> if validate succeed, otherwise, <c>false</c>.</returns>
        public static (bool, DateTime) ValidateTotp(byte[] securityToken, int code, string modifier = null)
        {
            if (securityToken == null)
            {
                throw new ArgumentNullException(nameof(securityToken));
            }
            var utcnow = DateTime.UtcNow;
            // Allow a variance of no greater than 90 seconds in either direction
            var currentTimeStep = GetCurrentTimeStepNumber(utcnow);
            using (var hashAlgorithm = new HMACSHA1(securityToken))
            {
                for (var i = -2; i <= 2; i++)
                {
                    var computedTotp = ComputeTotp(hashAlgorithm, (ulong)((long)currentTimeStep + i), modifier);
                   
                    if (computedTotp == code)
                    {
                        Console.WriteLine($"OTP:Generator -> OTP Code:{computedTotp} found at time: {currentTimeStep}");
                        return (true, utcnow);
                    }
                }
            }

            // No match
            return (false, utcnow);
        }

    }
}