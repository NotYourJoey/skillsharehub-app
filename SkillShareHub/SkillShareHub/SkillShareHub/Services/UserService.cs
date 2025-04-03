using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using SkillShareHub.Helpers;
using SkillShareHub.Models;
using Xamarin.Forms;

namespace SkillShareHub.Services
{
    public class UserService
    {
        private readonly IApiService _apiService;

        public UserService()
        {
            _apiService = DependencyService.Get<IApiService>();
        }

        public async Task<User> GetUserAsync(int userId)
        {
            try
            {
                var response = await _apiService.GetAsync($"{Constants.Endpoints.Users}/{userId}");

                if (response.IsSuccessStatusCode)
                {
                    var jsonResult = await response.Content.ReadAsStringAsync();
                    return System.Text.Json.JsonSerializer.Deserialize<User>(jsonResult, new System.Text.Json.JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching user: {ex.Message}");
                return null;
            }
        }

        public async Task<User> GetProfileAsync()
        {
            try
            {
                var response = await _apiService.GetAsync(Constants.Endpoints.UserProfile);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResult = await response.Content.ReadAsStringAsync();
                    return System.Text.Json.JsonSerializer.Deserialize<User>(jsonResult, new System.Text.Json.JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching profile: {ex.Message}");
                return null;
            }
        }

        public async Task<List<User>> SearchUsersAsync(string query, string skill = "")
        {
            try
            {
                string endpoint = Constants.Endpoints.Users;
                if (!string.IsNullOrEmpty(query) || !string.IsNullOrEmpty(skill))
                {
                    endpoint += "?";
                    if (!string.IsNullOrEmpty(query))
                    {
                        endpoint += $"search={query}";
                    }

                    if (!string.IsNullOrEmpty(skill))
                    {
                        endpoint += !string.IsNullOrEmpty(query) ? $"&skill={skill}" : $"skill={skill}";
                    }
                }

                var response = await _apiService.GetAsync(endpoint);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResult = await response.Content.ReadAsStringAsync();
                    return System.Text.Json.JsonSerializer.Deserialize<List<User>>(jsonResult, new System.Text.Json.JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }

                return new List<User>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error searching users: {ex.Message}");
                return new List<User>();
            }
        }

        public async Task<List<User>> GetSuggestedFriendsAsync()
        {
            try
            {
                var response = await _apiService.GetAsync(Constants.Endpoints.SuggestedFriends);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResult = await response.Content.ReadAsStringAsync();
                    return System.Text.Json.JsonSerializer.Deserialize<List<User>>(jsonResult, new System.Text.Json.JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }

                return new List<User>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching suggested friends: {ex.Message}");
                return new List<User>();
            }
        }

        public async Task<bool> UpdateProfileAsync(string firstName, string lastName, string location, string skills, byte[] profileImage = null)
        {
            try
            {
                var content = new MultipartFormDataContent();

                // Add text fields if provided
                if (!string.IsNullOrEmpty(firstName))
                    content.Add(new StringContent(firstName), "FirstName");

                if (!string.IsNullOrEmpty(lastName))
                    content.Add(new StringContent(lastName), "LastName");

                if (!string.IsNullOrEmpty(location))
                    content.Add(new StringContent(location), "Location");

                if (!string.IsNullOrEmpty(skills))
                    content.Add(new StringContent(skills), "Skills");

                // Add profile image if provided
                if (profileImage != null)
                {
                    var imageContent = new ByteArrayContent(profileImage);
                    imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
                    content.Add(imageContent, "ProfilePhoto", "profile.jpg");
                }

                var response = await _apiService.PutAsync(Constants.Endpoints.UserProfile, content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating profile: {ex.Message}");
                return false;
            }
        }
    }
}
