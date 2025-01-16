using Entities.Models;

namespace BlazorTemplate.Models
{
    public class ApplicationUser : User
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
