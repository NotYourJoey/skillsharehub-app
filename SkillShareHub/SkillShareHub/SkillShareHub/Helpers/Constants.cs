namespace SkillShareHub.Helpers
{
    public static class Constants
    {
        // API Base URL 
        public const string ApiBaseUrl = "https://10.0.2.2:7025/api/";
        
        // API Endpoints
        public static class Endpoints
        {
            // Auth endpoints
            public const string Register = "Auth/register";
            public const string Login = "Auth/login";
            
            // User endpoints
            public const string Users = "users";
            public const string UserById = "users/{0}";
            public const string UserProfile = "users/profile";
            public const string SuggestedFriends = "users/suggested-friends";
            public const string Feed = "users/feed";
            public const string UserPosts = "users/{0}/posts"; // For getting posts by user ID

            // Posts endpoints
            public const string Posts = "posts";
            public const string PostById = "posts/{0}";
            public const string PostComments = "posts/{0}/comments";
            public const string PostLike = "posts/{0}/like";
            
            // Comments endpoints
            public const string Comments = "comments";
            public const string CommentById = "comments/{0}";
            
            // Friends endpoints
            public const string Friends = "friends";
            public const string FriendRequests = "friends/requests";
            public const string SentRequests = "friends/sent";
            public const string SendRequest = "friends/request/{0}";
            public const string AcceptRequest = "friends/accept/{0}";
            public const string FriendById = "friends/{0}";
            public const string RequestById = "friends/request/{0}";
            
            // Messages endpoints
            public const string Messages = "messages";
            public const string MessagesWithUser = "messages/{0}";
            
            // Notifications endpoints
            public const string Notifications = "notifications";
            public const string UnreadCount = "notifications/unread-count";
            public const string MarkAsRead = "notifications/{0}/read";
            public const string MarkAllAsRead = "notifications/read-all";
        }
    }
}
