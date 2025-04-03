using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using SkillShareHub.Helpers;
using SkillShareHub.Models;
using Xamarin.Forms;

namespace SkillShareHub.Services
{
    public class PostService
    {
        private readonly IApiService _apiService;

        public PostService(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<List<Post>> GetPostsAsync()
        {
            try
            {
                var response = await _apiService.GetAsync(Constants.Endpoints.Posts).ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    Logger.LogError($"Failed to fetch posts: API returned {response.StatusCode}");
                    return new List<Post>();
                }

                var jsonResult = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return System.Text.Json.JsonSerializer.Deserialize<List<Post>>(jsonResult, new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error fetching posts: {ex.Message}");
                return new List<Post>();
            }
        }

        public async Task<Post> GetPostAsync(int postId)
        {
            try
            {
                var response = await _apiService.GetAsync($"{Constants.Endpoints.Posts}/{postId}").ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    Logger.LogError($"Failed to fetch post: API returned {response.StatusCode}");
                    return null;
                }

                var jsonResult = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return System.Text.Json.JsonSerializer.Deserialize<Post>(jsonResult, new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error fetching post: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> LikePostAsync(int postId)
        {
            try
            {
                var response = await _apiService.PostAsync(string.Format(Constants.Endpoints.PostLike, postId), null).ConfigureAwait(false);
                if (!response.IsSuccessStatusCode)
                {
                    Logger.LogError($"Failed to like post: API returned {response.StatusCode}");
                }
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error liking post: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UnlikePostAsync(int postId)
        {
            try
            {
                var response = await _apiService.DeleteAsync(string.Format(Constants.Endpoints.PostLike, postId)).ConfigureAwait(false);
                if (!response.IsSuccessStatusCode)
                {
                    Logger.LogError($"Failed to unlike post: API returned {response.StatusCode}");
                }
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error unliking post: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> CreatePostAsync(string content, byte[] mediaBytes, string mediaType)
        {
            try
            {
                var multipartContent = new MultipartFormDataContent
                {
                    { new StringContent(content ?? string.Empty), "Content" }
                };

                if (mediaBytes != null && mediaBytes.Length > 0)
                {
                    var mediaContent = new ByteArrayContent(mediaBytes)
                    {
                        Headers = { ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(mediaType == "image" ? "image/jpeg" : "video/mp4") }
                    };
                    string fileName = $"media_{DateTime.UtcNow.Ticks}.jpg";
                    multipartContent.Add(mediaContent, "Media", fileName);
                }

                var response = await _apiService.PostAsync(Constants.Endpoints.Posts, multipartContent).ConfigureAwait(false);
                if (!response.IsSuccessStatusCode)
                {
                    Logger.LogError($"Failed to create post: API returned {response.StatusCode}");
                }
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error creating post: {ex.Message}");
                return false;
            }
        }

        public async Task<List<Post>> GetUserPostsAsync(int userId)
        {
            try
            {
                var response = await _apiService.GetAsync($"{Constants.Endpoints.Users}/{userId}/posts").ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResult = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    return System.Text.Json.JsonSerializer.Deserialize<List<Post>>(jsonResult, new System.Text.Json.JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }

                return new List<Post>();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error fetching user posts: {ex.Message}");
                return new List<Post>();
            }
        }


        public static class Logger
        {
            public static void LogError(string message)
            {
                // In a real-world app, replace with a call to a logging library/service
                Console.WriteLine("[Error] " + message);
            }

            public static void LogInfo(string message)
            {
                // In a real-world app, replace with a call to a logging library/service
                Console.WriteLine("[Info] " + message);
            }
        }

    }
}
