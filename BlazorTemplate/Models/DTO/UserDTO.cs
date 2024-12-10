using Entities.Models;

namespace BlazorTemplateAPI.Models.DTO
{
    public class UserDTO
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdateAt { get; set; }
        public bool isLocked { get; set; }
        public DateTime BlockedUntil { get; set; }
        public List<Role> Roles { get; set; } = new List<Role>();
    }
}
