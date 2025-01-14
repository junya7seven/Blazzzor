using LibaryModalDialogPages.ModalPages;
using MudBlazor;
using System.Net.Http.Json;
using Shared;
using System.Linq.Expressions;
using static MudBlazor.CategoryTypes;


namespace BlazorAppClient.Pages
{
    public partial class Users
    {
        private IEnumerable<string> userProp { get; set; }
        private List<UserDTO> users = new List<UserDTO>();

        Dictionary<string, string> propertyValues { get; set; } = new Dictionary<string, string>();
        private bool isError { get; set; } = false;
        public string ErrorMessage { get; set; }

        private bool isDesc = false;

        private string searchString1 = "";

        private async Task ReloadPage()
        {
            await OnInitializedAsync();
        }

        private async Task GetUserData()
        {
            try
            {
                var response = await httpClient.GetAsync("user");
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
            await ReloadPage();
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

        protected override async Task OnInitializedAsync()
        {
            GetUserProp();

            foreach (var prop in userProp)
            {
                var value = users.GetType().GetProperty(prop)?.GetValue(users)?.ToString();
                propertyValues[prop] = value;
            }
            await GetUserData();
        }

        private void GetUserProp()
           {
               var properties = typeof(UserDTO).GetProperties();
       
               userProp = properties.Where(p => p.Name != nameof(UserDTO.Roles)).Select(p => p.Name);

               foreach (var prop in properties)
               {
                   if (!propertyValues.ContainsKey(prop.Name))
                       propertyValues[prop.Name] = null;
               }
           }


    }
}
