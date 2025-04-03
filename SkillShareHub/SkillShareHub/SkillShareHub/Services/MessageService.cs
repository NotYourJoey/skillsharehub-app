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
    public class MessageService
    {
        private readonly IApiService _apiService;

        public MessageService()
        {
            _apiService = DependencyService.Get<IApiService>();
        }

        public async Task<List<object>> GetConversationsAsync()
        {
            try
            {
                var response = await _apiService.GetAsync(Constants.Endpoints.Messages);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResult = await response.Content.ReadAsStringAsync();
                    return System.Text.Json.JsonSerializer.Deserialize<List<object>>(jsonResult, new System.Text.Json.JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }

                return new List<object>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching conversations: {ex.Message}");
                return new List<object>();
            }
        }

        public async Task<List<Message>> GetMessagesWithUserAsync(int userId)
        {
            try
            {
                var response = await _apiService.GetAsync(string.Format(Constants.Endpoints.MessagesWithUser, userId));

                if (response.IsSuccessStatusCode)
                {
                    var jsonResult = await response.Content.ReadAsStringAsync();
                    return System.Text.Json.JsonSerializer.Deserialize<List<Message>>(jsonResult, new System.Text.Json.JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }

                return new List<Message>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching messages: {ex.Message}");
                return new List<Message>();
            }
        }

        public async Task<Message> SendMessageAsync(int receiverId, string content)
        {
            try
            {
                var messageData = new
                {
                    ReceiverId = receiverId,
                    Content = content
                };

                var json = System.Text.Json.JsonSerializer.Serialize(messageData);
                var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _apiService.PostAsync(Constants.Endpoints.Messages, httpContent);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResult = await response.Content.ReadAsStringAsync();
                    return System.Text.Json.JsonSerializer.Deserialize<Message>(jsonResult, new System.Text.Json.JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending message: {ex.Message}");
                return null;
            }
        }
    }
}
