using Application.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IUserRoleService
    {
        /// <summary>
        /// Получить список всех ролей.
        /// </summary>
        /// <returns>Список ролей.</returns>
        Task<IEnumerable<RoleDTO>> GetAllRolesAsync();
        /// <summary>
        /// Получить список ролей пользователя по индефикатору.
        /// </summary>
        /// <param name="userId">Индефикатор пользователя</param>
        /// <returns>Список ролей.</returns>
        Task<List<string>> GetUserRolesByIdAsync(Guid userId);
        /// <summary>
        /// Создать роль.
        /// </summary>
        /// <param name="roleName">Название роли.</param>
        /// <returns>1 или 0</returns>
        Task<int> CreateRoleAsync(string roleName);
        /// <summary>
        /// Назначить или отозвать роль пользователя по идентификатору.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <param name="roles">Список ролей.</param>
        /// <returns>true или false</returns>
        Task<bool> AssingRangeRolesAsync(Guid userId, Dictionary<string, bool> roles);
    }
}
