using Application.Models;
using Application.Models.AuthModels;
using Application.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IUserService
    {
        /// <summary>
        /// Получить список всех пользователей с пагинацией.
        /// </summary>
        /// <param name="page">Текущая страница.</param>
        /// <param name="offset">Количество пользователей на странице.</param>
        /// <returns>Список пользователей и общее количество страниц.</returns>
        Task<(IEnumerable<UserDTO>, int)> GetAllUsersAsync(int page, int offset);

        /// <summary>
        /// Получить список всех пользователь по указанным ролям с пагинацией.
        /// </summary>
        /// <param name="page">Текущая страница.</param>
        /// <param name="offset">Количество пользоваталей на странице.</param>
        /// <param name="roles">Список ролей</param>
        /// <returns>Список пользователей по указанным ролям и общее количество страниц.</returns>
        Task<(IEnumerable<UserDTO>, int)> GetUsersByAllRolesAsync(int page, int offset, string[] roles);
        /// <summary>
        /// Получить пользователя по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор пользователя.</param>
        /// <returns>Пользователь в виде DTO или null, если пользователь не найден.</returns>
        Task<UserDTO?> GetUserByIdAsync(Guid id);

        /// <summary>
        /// Получить пользователя по почте.
        /// </summary>
        /// <param name="email">Почта пользователя.</param>
        /// <returns>Пользователь в виде DTO или null, если пользователь не найден.</returns>
        Task<ApplicationUser?> GetUserByEmailAsync(string email);

        /// <summary>
        /// Создать нового пользователя.
        /// </summary>
        /// <param name="userDto">Данные пользователя для создания.</param>
        /// <returns>Созданный пользователь в виде DTO.</returns>
        Task<UserDTO> CreateUserAsync(RegistrationModel userDto);

        /// <summary>
        /// Обновить пользователя.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <param name="user">Данные пользователя для создания.</param>
        /// <returns>Обновленный пользователь в виде DTO.</returns>
        Task<UserDTO> UpdateUserAsync(Guid userId, UserDTO user);

        /// <summary>
        /// Создать несколько пользователей.
        /// </summary>
        /// <param name="user">Массив пользователей.</param>
        /// <returns></returns>
        Task CreateUserRangeAsync(params UserDTO[] user);

        /// <summary>
        /// Заблокировать пользователя.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <param name="duration">Длительность блокировки default = 100 лет</param>
        /// <returns></returns>
        Task BlockUserAsync(Guid userId, DateTime blockUntil);

        /// <summary>
        /// Разблокировать пользователя.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <returns></returns>
        Task UnblockUserAsync(Guid userId);
    }
}
