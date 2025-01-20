using LibaryModalDialogPages.ModalPages;
using MudBlazor;
using System.Net.Http.Json;
using Shared;
using System.Linq.Expressions;
using static MudBlazor.CategoryTypes;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Shareds;
using Microsoft.AspNetCore.Components.Web;


namespace BlazorAppClient.Pages
{
    public partial class Users
    {
        private bool isError { get; set; } = false;
        public string ErrorMessage { get; set; }

        private bool isDesc = false;

        private bool isLoading;

        private IEnumerable<string> userProp { get; set; }
        private List<UserDTO> users = new List<UserDTO>();

        private Dictionary<string, string> columnHeaders = new();
        private Dictionary<string, bool> sortDirections = new();
        private int totalItems;

        private string searchQuery { get; set; } = string.Empty;

        private List<Role> Roles { get; set; } = new();
        private string _stringValue;
        private string stringValue
        {
            get => _stringValue;
            set
            {
                Console.WriteLine($"Role changed to: {value}");
                _stringValue = value == "Убрать роли" ? null : value;
                _ = table?.ReloadServerData();
            }
        }
        private MudTable<UserDTO> table;
        private async Task SearchUsers(KeyboardEventArgs args)
        {
            if (args.Key == "Enter") 
            {
                await table.ReloadServerData();
            }
            else
            {
                await table.ReloadServerData();
            }
        }
        private async Task GetRoles()
        {
            try
            {
                var response = await httpClient.GetAsync("userrole");
                if (response.IsSuccessStatusCode)
                {
                    Roles = await response.Content.ReadFromJsonAsync<List<Role>>() ?? new List<Role>();
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error loading roles: {ex.Message}");
            }
        }

        private async Task OnRoleChanged(string selectedRole)
        {
            stringValue = selectedRole == "Убрать роли" ? null : selectedRole;
            await table.ReloadServerData();
        }

        private async Task<TableData<UserDTO>> LoadServerData(TableState state, CancellationToken cancellationToken)
        {
            isLoading = true;

            try
            {
                int currentPage = state.Page + 1;
                int pageSize = state.PageSize;

                string url = stringValue == null
                    ? $"user?page={currentPage}&offset={pageSize}&searchQuery={searchQuery}"
                    : $"user/byroles?page={currentPage}&offset={pageSize}&roles={stringValue}&searchQuery={searchQuery}";

                var response = await httpClient.GetFromJsonAsync<PagginationModel<UserDTO>>(url, cancellationToken);
                if (response != null)
                {
                    users = response.Items.ToList();
                    totalItems = response.LatPage; 
                }

                return new TableData<UserDTO>
                {
                    Items = users,
                    TotalItems = totalItems
                };
            }
            finally
            {
                isLoading = false;
            }
        }

        protected override async Task OnInitializedAsync()
        {
            isLoading = true;
            GetUserProperties();
            await GetRoles();
            isLoading = false;
        }

        private void GetUserProperties()
        {
            var properties = typeof(UserDTO).GetProperties();

            userProp = properties
                .Where(p => p.Name != nameof(UserDTO.Roles))
                .Select(p => p.Name)
                .ToList();

            columnHeaders = properties.ToDictionary(
                prop => prop.Name,
                prop => prop.GetCustomAttribute<DisplayAttribute>()?.Name ?? prop.Name
            );
        }
        private async Task ReloadPage()
        {
            await OnInitializedAsync();
        }

        private async Task GetUserData()
        {
            try
            {
                var response = await httpClient.GetAsync($"user?page={1}&offset={10}");
                if (!response.IsSuccessStatusCode)
                {
                    isError = true;
                    ErrorMessage = response.StatusCode.ToString();
                }
                users = await response.Content.ReadFromJsonAsync<List<UserDTO>>();
            }
            catch (Exception ex)
            {
                isError = true;
                ErrorMessage = ex.Message;
            }
        }

        private async Task ViewUserRolesAsync(UserDTO user)
        {
            var parameters = new DialogParameters<ViewRolesDialog> { { x => x.User, user } };

            var dialog = await DialogService.ShowAsync<ViewRolesDialog>("Роли", parameters);
            var result = await dialog.Result;

            if (!result.Canceled)
            {
                Guid.TryParse(result.Data.ToString(), out Guid deletedServer);
            }
            await table.ReloadServerData();
        }

        private async Task BlockUserAsync(UserDTO user)
        {
            var parameters = new DialogParameters<BlockUserDialog> { { x => x.User, user } };

            var dialog = await DialogService.ShowAsync<BlockUserDialog>("Блокировка", parameters);
            var result = await dialog.Result;

            if (!result.Canceled)
            {
                Guid.TryParse(result.Data.ToString(), out Guid deletedServer);
            }
            await ReloadPage();
        }

        private async Task AddNewRoleAsync()
        {

            var dialog = await DialogService.ShowAsync<AddNewRoleDialog>("Добавить роль");
            var result = await dialog.Result;

            if (!result.Canceled)
            {
                Guid.TryParse(result.Data.ToString(), out Guid deletedServer);
            }
        }

        private async Task AddNewUserAsync()
        {
            try
            {
                var dialog = await DialogService.ShowAsync<AddNewUserDialog>("Добавить пользователя");
                var result = await dialog.Result;

                if (!result.Canceled)
                {
                    Guid.TryParse(result.Data.ToString(), out Guid deletedServer);
                }
                await ReloadPage();
            }
            catch (Exception ex)
            {
                isError = true;
                ErrorMessage = ex.Message;
                Snackbar.Add($"Ошибка: {ErrorMessage}", Severity.Error);
            }
        }

        private async Task UpdateUserAsync(UserDTO user)
        {
            var updateUser = Mapper.Map<UpdateUser>(user);
            var parameters = new DialogParameters();
            parameters.Add(nameof(UpdateUser), updateUser);
            parameters.Add("UserId", user.UserId);

            var dialog = await DialogService.ShowAsync<UpdateUserDialog>("Пользователь", parameters);
            var result = await dialog.Result;

            if (!result.Canceled)
            {
                Guid.TryParse(result.Data.ToString(), out Guid deletedServer);
            }
            await ReloadPage();
        }

        private async Task SortTableByColumn(string column)
        {

            var propertyInfo = typeof(UserDTO).GetProperty(column);

            if (propertyInfo == null)
                return;

            var parameter = Expression.Parameter(typeof(UserDTO), "x");
            var property = Expression.Property(parameter, propertyInfo);
            var lambda = Expression.Lambda<Func<UserDTO, object>>(Expression.Convert(property, typeof(object)), parameter);

            users = isDesc ?
                users.AsQueryable().OrderByDescending(lambda).ToList() :
                users.AsQueryable().OrderBy(lambda).ToList();

            isDesc = !isDesc;

        }



    }
}
