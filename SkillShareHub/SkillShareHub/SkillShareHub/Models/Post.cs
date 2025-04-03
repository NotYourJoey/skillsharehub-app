using System;

namespace SkillShareHub.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public string MediaUrl { get; set; }
        public string MediaType { get; set; }
        public DateTime CreatedAt { get; set; }
        public User User { get; set; }
        public int LikesCount { get; set; }
        public int CommentsCount { get; set; }
        public bool IsLiked { get; set; }
    }
}
