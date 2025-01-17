using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    public class RegistrationModel
    {
        [Required]
        [Length(6, 64, ErrorMessage = "Мин. 6 символов, макс 64")]
        [RegularExpression(@"^[a-zA-Zа-яА-Я0-9]+$", ErrorMessage = "Поле должно содержать только буквы и цифры.")]
        [DefaultValue("Bobster777")]
        public string UserName { get; set; }
        [Required]
        [Length(2, 64, ErrorMessage = "Мин. 2 символов, макс 64")]
        [RegularExpression(@"^[a-zA-Zа-яА-Я]+$", ErrorMessage = "Поле должно содержать только буквы.")]
        [DefaultValue("Аркадий")]
        public string FirstName { get; set; }
        [Required]
        [Length(2, 64, ErrorMessage = "Мин. 2 символов, макс 64")]
        [RegularExpression(@"^[a-zA-Zа-яА-Я]+$", ErrorMessage = "Поле должно содержать только буквы.")]
        [DefaultValue("Паровозов")]
        public string LastName { get; set; }
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Неверный формат почты")]
        [DataType(DataType.EmailAddress)]
        [DefaultValue("Arkady2007@gmail.com")]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [DefaultValue("password123!@")]
        public string PasswordHash { get; set; }
    }
}
