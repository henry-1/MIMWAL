
namespace MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Common
{

    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Security.Cryptography;

    /// <summary>
    /// PasswordGenerator class
    /// </summary>
    /// <remarks>
    /// 
    /// Minimum password length: 4.
    /// Contains at least one uppercase character,
    /// one lowercase character, one number,
    /// and one special character.
    /// Remaining characters are randomly selected
    /// from all allowed characters.
    /// Character selection and position shuffling
    /// use a cryptographic random number generator.
    /// 
    /// </remarks>    

    [SuppressMessage("SonarAnalyzer.CSharp", "S2068", Justification = "validated, not a password")]
    public static class PasswordGenerator
    {

        private static readonly RNGCryptoServiceProvider rng =
            new RNGCryptoServiceProvider();

        private const int MinimumPasswordLength = 4;

        private const string PwdCharsUpperCase =
            "ABCDEFGHJKLMNPQRSTUVWXYZ";

        private const string PwdCharsLowerCase =
            "abcdefghjkmnpqrstuvwxyz";

        private const string PwdChars =
            PwdCharsUpperCase + PwdCharsLowerCase;

        private const string PwdSpecialChars =
            "!@#$&";

        private const string PwdNumbers =
            "23456789";

        private const string PwdAllChars =
            PwdChars +
            PwdSpecialChars +
            PwdNumbers;

        public static string GenerateRandomPassword(
            int length)
        {
            Logger.Instance.WriteMethodEntry(
                EventIdentifier.PasswordGeneratorGenerateRandomPassword,
                "Length: {0}.",
                length);

            try
            {
                if (length < MinimumPasswordLength)
                {
                    throw Logger.Instance.ReportError(
                        new ArgumentOutOfRangeException(
                            "length",
                            length,
                            string.Format(
                                CultureInfo.InvariantCulture,
                                "Password length must be at least {0}.",
                                MinimumPasswordLength)));
                }

                char[] password =
                    new char[length];

                password[0] = GetRandomCharacter(PwdCharsUpperCase);

                password[1] = GetRandomCharacter(PwdCharsLowerCase);

                password[2] = GetRandomCharacter(PwdNumbers);

                password[3] = GetRandomCharacter(PwdSpecialChars);

                for (int i = 4; i < password.Length; i++)
                {
                    password[i] = GetRandomCharacter(PwdAllChars);
                }

                Shuffle(password);            

                return new string(password);
            }
            finally
            {
                Logger.Instance.WriteMethodExit(
                    EventIdentifier.PasswordGeneratorGenerateRandomPassword,
                    "Length: {0}.",
                    length);
            }
        }

        private static char GetRandomCharacter(
            string characters)
        {
            int index = GetNextRandomNumber(
                0,
                characters.Length);

            return characters[index];
        }

        private static void Shuffle(char[] password)
        {
            for (int i = password.Length - 1; i > 0; i--)
            {
                int index = GetNextRandomNumber(
                    0,
                    i + 1);

                char temporary = password[i];
                password[i] = password[index];
                password[index] = temporary;
            }
        }

        private static int GetNextRandomNumber(
            int minValue,
            int maxValue)
        {
            if (minValue < 0)
            {
                throw Logger.Instance.ReportError(
                    new ArgumentOutOfRangeException(
                        "minValue"));
            }

            if (minValue >= maxValue)
            {
                throw Logger.Instance.ReportError(
                    new ArgumentException(
                        Messages.PasswordGenerator_MaxValueLessThanMinValueError));
            }

            uint range =
                (uint)(maxValue - minValue);

            ulong availableValues =
                (ulong)uint.MaxValue + 1UL;

            ulong limit =
                availableValues -
                availableValues % range;

            byte[] buffer =
                new byte[4];

            uint randomValue;

            do
            {
                rng.GetBytes(buffer);

                randomValue =
                    BitConverter.ToUInt32(
                        buffer,
                        0);
            }
            while ((ulong)randomValue >= limit);

            return minValue +
                (int)(randomValue % range);
        }
    }

    
}



