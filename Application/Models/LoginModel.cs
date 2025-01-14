using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    public class LoginModel
    {
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Неверный формат почты")]
        [DataType(DataType.EmailAddress)]
        [DefaultValue("Arkady2007@gmail.com")]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [DefaultValue("password123!@")]
        public string Password { get; set; }
    }
}
