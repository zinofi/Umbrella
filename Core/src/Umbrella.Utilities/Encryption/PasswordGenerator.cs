using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Umbrella.Utilities.Encryption.Interfaces;
using Umbrella.Utilities.Extensions;

namespace Umbrella.Utilities.Encryption
{
    public class PasswordGenerator : IPasswordGenerator
    {
        #region Private Static Members
        private static readonly char[] m_LettersArray = new char[26]
        {
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'
        };
        #endregion

        #region Private Members
        private readonly ILogger Log;
        #endregion

        #region Constructors
        public PasswordGenerator(ILogger<PasswordGenerator> logger)
        {
            Log = logger;
        }
        #endregion

        #region IPasswordGenerator Members
        public string GeneratePassword(int length = 8, int minNumbers = 1)
        {
            try
            {
                if (length < 1)
                    throw new ArgumentOutOfRangeException(nameof(length), "Must be greater than or equal to 1.");

                if (minNumbers < 0)
                    throw new ArgumentOutOfRangeException(nameof(minNumbers), "Must be greater than or equal to 0.");

                if (minNumbers > length)
                    throw new ArgumentOutOfRangeException(nameof(minNumbers), "Must be less than or equal to length.");

                StringBuilder builder = new StringBuilder(length);

                int lettersLength = length - minNumbers;

                using (RNGCryptoServiceProvider rngProvider = new RNGCryptoServiceProvider())
                {
                    while (builder.Length < lettersLength)
                    {
                        int index = GenerateRandomInteger(rngProvider, 0, 26);
                        char letter = m_LettersArray[index];

                        builder.Append(letter);
                    }

                    while (builder.Length < length)
                    {
                        int number = GenerateRandomInteger(rngProvider, 0, 10);
                        int insertIndex = GenerateRandomInteger(rngProvider, 1, builder.Length);

                        builder.Insert(insertIndex, number);
                    }
                }

                return builder.ToString();
            }
            catch (Exception exc) when (Log.WriteError(exc, new { length, minNumbers }))
            {
                throw;
            }
        }
        #endregion

        #region Private Members
        private int GenerateRandomInteger(RNGCryptoServiceProvider provider, int min, int max)
        {
            uint scale = uint.MaxValue;
            while (scale == uint.MaxValue)
            {
                // Get four random bytes.
                byte[] four_bytes = new byte[4];
                provider.GetBytes(four_bytes);

                // Convert that into an uint.
                scale = BitConverter.ToUInt32(four_bytes, 0);
            }

            // Add min to the scaled difference between max and min.
            return (int)(min + (max - min) *
                (scale / (double)uint.MaxValue));
        }
        #endregion
    }
}