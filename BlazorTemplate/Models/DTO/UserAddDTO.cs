using System.ComponentModel.DataAnnotations;

namespace BlazorTemplateAPI.Models.DTO
{
    public class UserAddDTO
    {
        public string UserName { get; set; }
        public string FirstName { get; set; }
        [MinLength(10,ErrorMessage = "asdasdasdasda")]
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
