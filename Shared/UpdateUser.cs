using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class UpdateUser
    {
        [Length(6, 64, ErrorMessage = "Мин. 6 символов, макс 64")]
        [RegularExpression(@"^[a-zA-Zа-яА-Я0-9]+$", ErrorMessage = "Поле должно содержать только буквы и цифры.")]
        public string UserName { get; set; }
        [Length(6, 64, ErrorMessage = "Мин. 6 символов, макс 64")]
        [RegularExpression(@"^[a-zA-Zа-яА-Я0-9]+$", ErrorMessage = "Поле должно содержать только буквы и цифры.")]
        public string FirstName { get; set; }
        [Length(6, 64, ErrorMessage = "Мин. 6 символов, макс 64")]
        [RegularExpression(@"^[a-zA-Zа-яА-Я0-9]+$", ErrorMessage = "Поле должно содержать только буквы и цифры.")]
        public string LastName { get; set; }
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}
