using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Umbrella.Utilities.Encryption.Abstractions;
using Umbrella.Utilities.Extensions;

namespace Umbrella.Utilities.Encryption
{
    //TODO: Add support for special chars, e.g. !@#$!&
    public class SecureStringGenerator : ISecureStringGenerator
    {
        #region Private Static Members
        private static readonly char[] m_LowerCaseLettersArray = new char[26]
        {
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'
        };
        private static readonly char[] m_UpperCaseLettersArray = m_LowerCaseLettersArray.Select(x => char.ToUpperInvariant(x)).ToArray();
        #endregion

        #region Private Members
        private readonly ILogger Log;
        #endregion

        #region Constructors
        public SecureStringGenerator(ILogger<SecureStringGenerator> logger)
        {
            Log = logger;
        }
        #endregion

        #region IPasswordGenerator Members
        public string Generate(int length = 8, int numbers = 1, int upperCaseLetters = 1)
        {
            try
            {
				// TODO: Replace these checks with new Guard statements. Move outside try...catch.
                if (length < 1)
                    throw new ArgumentOutOfRangeException(nameof(length), "Must be greater than or equal to 1.");

                if (numbers < 0)
                    throw new ArgumentOutOfRangeException(nameof(numbers), "Must be greater than or equal to 0.");

                if (numbers > length)
                    throw new ArgumentOutOfRangeException(nameof(numbers), "Must be less than or equal to length.");

                if (upperCaseLetters < 0)
                    throw new ArgumentOutOfRangeException(nameof(upperCaseLetters), "Must be greater than or equal to 0.");

                if (upperCaseLetters > length)
                    throw new ArgumentOutOfRangeException(nameof(upperCaseLetters), "Must be less than or equal to length.");

                if (numbers + upperCaseLetters > length)
                    throw new ArgumentOutOfRangeException($"{nameof(numbers)}, {nameof(upperCaseLetters)}", $"The sum of the {nameof(numbers)} and the {nameof(upperCaseLetters)} arguments is greater than the length.");

                Span<char> password = stackalloc char[length];

                int lowerCaseLettersLength = length - numbers - upperCaseLetters;

				// TODO: Make the RNG provider a class member, implement IDisposable to clean it up, use RandomNumberGenerator class instead.
				// Call RandomNumberGenerator.Create()
                // We are building up a string here starting with lowercase letters, followed by uppercase and finally numbers.
                using (RNGCryptoServiceProvider rngProvider = new RNGCryptoServiceProvider())
                {
                    int idx = 0;

                    while (idx < lowerCaseLettersLength)
                    {
                        int index = GenerateRandomInteger(rngProvider, 0, 26);
                        char letter = m_LowerCaseLettersArray[index];

                        password[idx++] = letter;
                    }

                    while (idx < length - numbers)
                    {
                        int index = GenerateRandomInteger(rngProvider, 0, 26);
                        char letter = m_UpperCaseLettersArray[index];

                        password[idx++] = letter;
                    }

                    while (idx < length)
                    {
                        int number = GenerateRandomInteger(rngProvider, 0, 10);

                        password[idx++] = number.ToString()[0];
                    }

                    // Randomly shuffle the generated password
                    int n = password.Length;
                    while (n > 1)
                    {
                        int k = GenerateRandomInteger(rngProvider, 0, n--);
                        char temp = password[n];
                        password[n] = password[k];
                        password[k] = temp;
                    }
                }

                return password.ToString();
            }
            catch (Exception exc) when (Log.WriteError(exc, new { length, numbers }))
            {
                throw;
            }
        }

#if !AzureDevOps
        [Obsolete]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal string GenerateOld(int length = 8, int numbers = 1, int upperCaseLetters = 1)
        {
            try
            {
                if (length < 1)
                    throw new ArgumentOutOfRangeException(nameof(length), "Must be greater than or equal to 1.");

                if (numbers < 0)
                    throw new ArgumentOutOfRangeException(nameof(numbers), "Must be greater than or equal to 0.");

                if (numbers > length)
                    throw new ArgumentOutOfRangeException(nameof(numbers), "Must be less than or equal to length.");

                if (upperCaseLetters < 0)
                    throw new ArgumentOutOfRangeException(nameof(upperCaseLetters), "Must be greater than or equal to 0.");

                if (upperCaseLetters > length)
                    throw new ArgumentOutOfRangeException(nameof(upperCaseLetters), "Must be less than or equal to length.");

                if (numbers + upperCaseLetters > length)
                    throw new ArgumentOutOfRangeException($"{nameof(numbers)}, {nameof(upperCaseLetters)}", $"The sum of the {nameof(numbers)} and the {nameof(upperCaseLetters)} arguments is greater than the length.");

                char[] password = new char[length];

                int lowerCaseLettersLength = length - numbers - upperCaseLetters;

                // We are building up a string here starting with lowercase letters, followed by uppercase and finally numbers.
                using (RNGCryptoServiceProvider rngProvider = new RNGCryptoServiceProvider())
                {
                    int idx = 0;

                    while (idx < lowerCaseLettersLength)
                    {
                        int index = GenerateRandomInteger(rngProvider, 0, 26);
                        char letter = m_LowerCaseLettersArray[index];

                        password[idx++] = letter;
                    }

                    while (idx < length - numbers)
                    {
                        int index = GenerateRandomInteger(rngProvider, 0, 26);
                        char letter = m_UpperCaseLettersArray[index];

                        password[idx++] = letter;
                    }

                    while (idx < length)
                    {
                        int number = GenerateRandomInteger(rngProvider, 0, 10);

                        password[idx++] = number.ToString().ToCharArray()[0];
                    }

                    // Randomly shuffle the generated password
                    int n = password.Length;
                    while (n > 1)
                    {
                        int k = GenerateRandomInteger(rngProvider, 0, n--);
                        char temp = password[n];
                        password[n] = password[k];
                        password[k] = temp;
                    }
                }

                return new string(password);
            }
            catch (Exception exc) when (Log.WriteError(exc, new { length, numbers }))
            {
                throw;
            }
        }
#endif
        #endregion

        #region Private Members
        private int GenerateRandomInteger(RNGCryptoServiceProvider provider, int min, int max)
        {
            uint scale = uint.MaxValue;
            while (scale == uint.MaxValue)
            {
				// TODO: Could use ArrayPool here.
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