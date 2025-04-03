using System;

namespace SkillShareHub.Models
{
    public class Friend
    {
        public int Id { get; set; }
        public User User { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsAccepted { get; set; }
    }
}
