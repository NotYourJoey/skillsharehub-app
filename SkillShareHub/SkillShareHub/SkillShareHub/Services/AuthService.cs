using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using SkillShareHub.Helpers;
using SkillShareHub.Models;
using Xamarin.Forms;

namespace SkillShareHub.Services
{
    public class AuthService
    {
        private readonly IApiService _apiService;

        public AuthService()
        {
            _apiService = DependencyService.Get<IApiService>();
        }

        public async Task<(bool Success, string ErrorMessage)> RegisterAsync(string firstName, string lastName, string username,
                                             string email, string password, string confirmPassword,
                                             string location, string skills, byte[] profileImage = null)
        {
            try
            {
                // Create multipart form content for file upload
                var content = new MultipartFormDataContent();

                // Add text fields
                content.Add(new StringContent(firstName), "FirstName");
                content.Add(new StringContent(lastName), "LastName");
                content.Add(new StringContent(username), "Username");
                content.Add(new StringContent(email), "Email");
                content.Add(new StringContent(password), "Password");
                content.Add(new StringContent(confirmPassword), "ConfirmPassword");
                content.Add(new StringContent(location), "Location");
                content.Add(new StringContent(skills), "Skills");

                // Add profile image if provided
                if (profileImage != null)
                {
                    var imageContent = new ByteArrayContent(profileImage);
                    imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
                    content.Add(imageContent, "ProfilePhoto", "profile.jpg");
                }

                // Log for debugging
                Debug.WriteLine($"Sending registration for user: {username}, email: {email}");

                var response = await _apiService.PostAsync(Constants.Endpoints.Register, content);

                // Read response content regardless of status code
                var responseContent = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"Registration response: {response.StatusCode}, Content: {responseContent}");

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        var result = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(responseContent);

                        // Store token and user info
                        var token = result.GetProperty("token").GetString();
                        var user = result.GetProperty("user");

                        Settings.AuthToken = token;
                        Settings.UserId = user.GetProperty("id").GetInt32();
                        Settings.Username = user.GetProperty("username").GetString();
                        Settings.ProfilePhotoUrl = user.GetProperty("profilePhotoUrl").GetString();

                        return (true, null);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error parsing registration response: {ex.Message}");
                        return (false, "Could not process the server response. Please try again.");
                    }
                }
                else
                {
                    // Try to extract the error message from the response
                    string errorMessage = "Registration failed. Please try again.";

                    try
                    {
                        // Attempt to parse JSON error response
                        var error = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(responseContent);
                        if (error.TryGetProperty("message", out var message))
                        {
                            errorMessage = message.GetString();
                        }
                        else if (error.TryGetProperty("error", out var errorProp))
                        {
                            errorMessage = errorProp.GetString();
                        }
                        else if (responseContent.Length < 100) // If it's a short string, might be a simple error
                        {
                            errorMessage = responseContent;
                        }
                    }
                    catch
                    {
                        // If we can't parse the error as JSON, just use the status code
                        errorMessage = $"Registration failed with status code: {response.StatusCode}";
                    }

                    return (false, errorMessage);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Registration error: {ex.Message}");
                return (false, $"An error occurred: {ex.Message}");
            }
        }

        public async Task<(bool Success, string ErrorMessage)> LoginAsync(string email, string password)
        {
            try
            {
                var loginData = new
                {
                    Email = email,
                    Password = password
                };

                var json = System.Text.Json.JsonSerializer.Serialize(loginData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Log for debugging
                Debug.WriteLine($"Sending login request for user: {email}");

                var response = await _apiService.PostAsync(Constants.Endpoints.Login, content);

                // Read response content regardless of status code
                var responseContent = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"Login response: {response.StatusCode}, Content: {responseContent}");

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        var result = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(responseContent);

                        // Store token and user info
                        var token = result.GetProperty("token").GetString();
                        var user = result.GetProperty("user");

                        Settings.AuthToken = token;
                        Settings.UserId = user.GetProperty("id").GetInt32();
                        Settings.Username = user.GetProperty("username").GetString();
                        Settings.ProfilePhotoUrl = user.GetProperty("profilePhotoUrl").GetString();

                        return (true, null);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error parsing login response: {ex.Message}");
                        return (false, "Could not process the server response. Please try again.");
                    }
                }
                else
                {
                    // Try to extract the error message from the response
                    string errorMessage = "Login failed. Please check your credentials and try again.";

                    try
                    {
                        // Attempt to parse JSON error response
                        var error = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(responseContent);
                        if (error.TryGetProperty("message", out var message))
                        {
                            errorMessage = message.GetString();
                        }
                        else if (error.TryGetProperty("error", out var errorProp))
                        {
                            errorMessage = errorProp.GetString();
                        }
                        else if (responseContent.Length < 100) // If it's a short string, might be a simple error
                        {
                            errorMessage = responseContent;
                        }
                    }
                    catch
                    {
                        // If we can't parse the error as JSON, just use the status code
                        errorMessage = $"Login failed with status code: {response.StatusCode}";
                    }

                    return (false, errorMessage);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Login error: {ex.Message}");
                return (false, $"An error occurred: {ex.Message}");
            }
        }

        // Test method to validate API connectivity
        public async Task<bool> TestApiConnection()
        {
            try
            {
                var response = await _apiService.GetAsync("");  // Just hit the base URL
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"API connection test failed: {ex.Message}");
                return false;
            }
        }

        public void Logout()
        {
            Settings.ClearAll();
        }
    }
}
