using System;
using Xamarin.Essentials;
using System.Diagnostics;

namespace SkillShareHub.Helpers
{
    public static class Settings
    {
        private const string AuthTokenKey = "auth_token";
        public static string AuthToken
        {
            get
            {
                var token = Preferences.Get(AuthTokenKey, string.Empty);
                DebugCheckValue(nameof(AuthToken), token);
                return token;
            }
            set => Preferences.Set(AuthTokenKey, value ?? string.Empty);
        }

        private const string UserIdKey = "user_id";
        public static int UserId
        {
            get
            {
                var userId = Preferences.Get(UserIdKey, 0);
                DebugCheckValue(nameof(UserId), userId);
                return userId;
            }
            set => Preferences.Set(UserIdKey, value);
        }

        private const string UsernameKey = "username";
        public static string Username
        {
            get
            {
                var username = Preferences.Get(UsernameKey, string.Empty);
                DebugCheckValue(nameof(Username), username);
                return username;
            }
            set => Preferences.Set(UsernameKey, value ?? string.Empty);
        }

        private const string ProfilePhotoKey = "profile_photo";
        public static string ProfilePhotoUrl
        {
            get
            {
                var url = Preferences.Get(ProfilePhotoKey, string.Empty);
                DebugCheckValue(nameof(ProfilePhotoUrl), url);
                return url;
            }
            set => Preferences.Set(ProfilePhotoKey, value ?? string.Empty);
        }

        public static bool IsAuthenticated => !string.IsNullOrEmpty(AuthToken);

        public static void ClearAll()
        {
            Preferences.Clear();
        }

        private static void DebugCheckValue(string propertyName, object value)
        {
            if (Debugger.IsAttached && (value == null || (value is string s && string.IsNullOrEmpty(s))))
            {
                Debug.WriteLine($"Warning: {propertyName} setting is null or empty.");
            }
        }
    }
}

