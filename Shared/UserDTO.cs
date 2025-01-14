using System.ComponentModel.DataAnnotations;

namespace Shared
{
    public class UserDTO
    {
        [Display(Name = "Id пользователя")]
        public Guid UserId { get; set; }
        [Display(Name = "Пользовательское имя")]
        public string UserName { get; set; }
        [Display(Name = "Имя")]
        public string FirstName { get; set; }
        [Display(Name = "Фамилия")]
        public string LastName { get; set; }
        [Display(Name = "Почта")]
        public string Email { get; set; }
        [Display(Name = "Дата регистрации")]
        public DateTime CreatedAt { get; set; }
        [Display(Name = "Последнее обновление")]
        public DateTime LastUpdateAt { get; set; }
        [Display(Name = "Статус блокировки")]
        public bool isLocked { get; set; }
        [Display(Name = "Дата окончания блокировки")]
        public DateTime BlockedUntil { get; set; }
        public List<Role> Roles { get; set; } = new List<Role>();
    }
    public class Role
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string NormalName { get; set; }
    }
}
