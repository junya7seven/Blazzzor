using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    public class UpdateUser
    {
        [Length(6, 64, ErrorMessage = "Мин. 6 символов, макс 64")]
        [RegularExpression(@"^[a-zA-Zа-яА-Я0-9]+$", ErrorMessage = "Поле должно содержать только буквы и цифры.")]
        [DefaultValue("Bobster777")]
        public string UserName { get; set; }
        [Length(6, 64, ErrorMessage = "Мин. 2 символов, макс 64")]
        [RegularExpression(@"^[a-zA-Zа-яА-Я]+$", ErrorMessage = "Поле должно содержать только буквы.")]
        [DefaultValue("Аркадий")]
        public string FirstName { get; set; }
        [Length(6, 64, ErrorMessage = "Мин. 2 символов, макс 64")]
        [RegularExpression(@"^[a-zA-Zа-яА-Я]+$", ErrorMessage = "Поле должно содержать только буквы.")]
        [DefaultValue("Паровозов")]
        public string LastName { get; set; }
        [DataType(DataType.EmailAddress)]
        [DefaultValue("Arkady2007@gmail.com")]
        public string Email { get; set; }
    }
}
