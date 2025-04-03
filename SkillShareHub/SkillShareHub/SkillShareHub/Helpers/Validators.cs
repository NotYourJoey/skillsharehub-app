using System;
using System.Text.RegularExpressions;

namespace SkillShareHub.Helpers
{
    public static class Validators
    {
        // Email validation
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Use regex pattern for email validation
                return Regex.IsMatch(email,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    RegexOptions.IgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        // Password validation (min 6 chars)
        public static bool IsValidPassword(string password)
        {
            return !string.IsNullOrWhiteSpace(password) && password.Length >= 6;
        }

        // Username validation (alphanumeric, underscore, 3-20 chars)
        public static bool IsValidUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return false;

            return Regex.IsMatch(username, @"^[a-zA-Z0-9_]{3,20}$");
        }

        // Check if all required fields are filled
        public static bool AreRequiredFieldsFilled(params string[] fields)
        {
            foreach (var field in fields)
            {
                if (string.IsNullOrWhiteSpace(field))
                    return false;
            }

            return true;
        }
    }
}
