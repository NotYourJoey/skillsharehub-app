using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using SkillShareHub.Helpers;
using SkillShareHub.Models;
using Xamarin.Forms;

namespace SkillShareHub.Services
{
    public class NotificationService
    {
        private readonly IApiService _apiService;

        public NotificationService()
        {
            _apiService = DependencyService.Get<IApiService>();
        }

        public async Task<List<Notification>> GetNotificationsAsync()
        {
            try
            {
                var response = await _apiService.GetAsync(Constants.Endpoints.Notifications);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResult = await response.Content.ReadAsStringAsync();
                    return System.Text.Json.JsonSerializer.Deserialize<List<Notification>>(jsonResult, new System.Text.Json.JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }

                return new List<Notification>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching notifications: {ex.Message}");
                return new List<Notification>();
            }
        }

        public async Task<int> GetUnreadCountAsync()
        {
            try
            {
                var response = await _apiService.GetAsync(Constants.Endpoints.UnreadCount);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResult = await response.Content.ReadAsStringAsync();
                    var result = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(jsonResult);
                    return result.GetProperty("count").GetInt32();
                }

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching unread count: {ex.Message}");
                return 0;
            }
        }

        public async Task<bool> MarkAsReadAsync(int notificationId)
        {
            try
            {
                var response = await _apiService.PutAsync(string.Format(Constants.Endpoints.MarkAsRead, notificationId), null);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error marking notification as read: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> MarkAllAsReadAsync()
        {
            try
            {
                var response = await _apiService.PutAsync(Constants.Endpoints.MarkAllAsRead, null);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error marking all notifications as read: {ex.Message}");
                return false;
            }
        }
    }
}
