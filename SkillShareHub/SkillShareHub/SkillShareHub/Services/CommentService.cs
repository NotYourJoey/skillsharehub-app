using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using SkillShareHub.Helpers;
using SkillShareHub.Models;
using Xamarin.Forms;

namespace SkillShareHub.Services
{
    public class CommentService
    {
        private readonly IApiService _apiService;

        public CommentService()
        {
            _apiService = DependencyService.Get<IApiService>();
        }

        public async Task<List<Comment>> GetCommentsAsync(int postId)
        {
            try
            {
                var response = await _apiService.GetAsync(string.Format(Constants.Endpoints.PostComments, postId));

                if (response.IsSuccessStatusCode)
                {
                    var jsonResult = await response.Content.ReadAsStringAsync();
                    return System.Text.Json.JsonSerializer.Deserialize<List<Comment>>(jsonResult, new System.Text.Json.JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }

                return new List<Comment>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching comments: {ex.Message}");
                return new List<Comment>();
            }
        }

        public async Task<Comment> AddCommentAsync(int postId, string content)
        {
            try
            {
                var commentData = new
                {
                    Content = content
                };

                var json = System.Text.Json.JsonSerializer.Serialize(commentData);
                var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _apiService.PostAsync(string.Format(Constants.Endpoints.PostComments, postId), httpContent);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResult = await response.Content.ReadAsStringAsync();
                    return System.Text.Json.JsonSerializer.Deserialize<Comment>(jsonResult, new System.Text.Json.JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding comment: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> DeleteCommentAsync(int postId, int commentId)
        {
            try
            {
                var response = await _apiService.DeleteAsync($"{string.Format(Constants.Endpoints.PostComments, postId)}/{commentId}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting comment: {ex.Message}");
                return false;
            }
        }
    }
}
