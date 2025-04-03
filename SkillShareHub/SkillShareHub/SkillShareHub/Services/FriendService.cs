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
    public class FriendService
    {
        private readonly IApiService _apiService;

        public FriendService()
        {
            _apiService = DependencyService.Get<IApiService>();
        }

        public async Task<FriendshipStatus> GetFriendshipStatusAsync(int userId)
        {
            try
            {
                // Get all friends to check if the user is already a friend
                var friends = await GetFriendsAsync();
                bool isFriend = friends.Any(f => f.User != null && f.User.Id == userId);

                // Get all pending received requests to check if there's a pending request from this user
                var receivedRequests = await GetFriendRequestsAsync();
                bool hasPendingReceived = receivedRequests.Any(r => r.User != null && r.User.Id == userId);

                // Get all pending sent requests to check if there's a pending request to this user
                var sentRequests = await GetSentRequestsAsync();
                bool hasPendingSent = sentRequests.Any(s => s.User != null && s.User.Id == userId);

                return new FriendshipStatus
                {
                    IsFriend = isFriend,
                    HasPendingReceived = hasPendingReceived,
                    HasPendingSent = hasPendingSent
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting friendship status: {ex.Message}");
                return new FriendshipStatus();
            }
        }


        public async Task<List<Friend>> GetFriendsAsync()
        {
            try
            {
                var response = await _apiService.GetAsync(Constants.Endpoints.Friends);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResult = await response.Content.ReadAsStringAsync();
                    return System.Text.Json.JsonSerializer.Deserialize<List<Friend>>(jsonResult, new System.Text.Json.JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }

                return new List<Friend>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching friends: {ex.Message}");
                return new List<Friend>();
            }
        }

        public async Task<List<Friend>> GetFriendRequestsAsync()
        {
            try
            {
                var response = await _apiService.GetAsync(Constants.Endpoints.FriendRequests);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResult = await response.Content.ReadAsStringAsync();
                    return System.Text.Json.JsonSerializer.Deserialize<List<Friend>>(jsonResult, new System.Text.Json.JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }

                return new List<Friend>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching friend requests: {ex.Message}");
                return new List<Friend>();
            }
        }



        public async Task<List<Friend>> GetSentRequestsAsync()
        {
            try
            {
                var response = await _apiService.GetAsync(Constants.Endpoints.SentRequests);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResult = await response.Content.ReadAsStringAsync();
                    return System.Text.Json.JsonSerializer.Deserialize<List<Friend>>(jsonResult, new System.Text.Json.JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }

                return new List<Friend>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching sent requests: {ex.Message}");
                return new List<Friend>();
            }
        }

        public async Task<bool> SendFriendRequestAsync(int userId)
        {
            try
            {
                var response = await _apiService.PostAsync(string.Format(Constants.Endpoints.SendRequest, userId), null);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending friend request: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> AcceptFriendRequestAsync(int requestId)
        {
            try
            {
                var response = await _apiService.PostAsync(string.Format(Constants.Endpoints.AcceptRequest, requestId), null);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error accepting friend request: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteFriendRequestAsync(int requestId)
        {
            try
            {
                var response = await _apiService.DeleteAsync(string.Format(Constants.Endpoints.FriendById, requestId));
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting friend request: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> RemoveFriendAsync(int friendshipId)
        {
            try
            {
                var response = await _apiService.DeleteAsync(string.Format(Constants.Endpoints.FriendById, friendshipId));
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error removing friend: {ex.Message}");
                return false;
            }
        }
    }
}
