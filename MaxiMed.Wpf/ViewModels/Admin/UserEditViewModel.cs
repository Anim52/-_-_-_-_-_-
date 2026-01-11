using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using MaxiMed.Application.Users;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Wpf.ViewModels.Admin
{
    public partial class UserEditViewModel : ObservableObject
    {
        private readonly IUserAdminService _service;

        public event Action<bool>? RequestClose;

        [ObservableProperty] private int id;
        [ObservableProperty] private string login = "";
        [ObservableProperty] private string? fullName;
        [ObservableProperty] private bool isActive = true;

        // роли как чекбоксы
        public ObservableCollection<RoleItemVm> Roles { get; } = new();

        // пароль: обязателен при создании
        [ObservableProperty] private string? newPassword;

        [ObservableProperty] private string? errorText;

        public bool IsCreateMode => Id == 0;

        public UserEditViewModel(IUserAdminService service) => _service = service;

        public async Task LoadAsync(int id)
        {
            ErrorText = null;
            NewPassword = null;

            Id = id;
            Login = "";
            FullName = "";
            IsActive = true;

            // подгружаем список ролей всегда
            Roles.Clear();
            var allRoles = await _service.GetAllRoleNamesAsync();
            foreach (var r in allRoles)
                Roles.Add(new RoleItemVm(r));

            if (id == 0)
            {
                // дефолт: Registrar (если есть)
                var def = Roles.FirstOrDefault(x => x.Name.Equals("Registrar", StringComparison.OrdinalIgnoreCase));
                if (def != null) def.IsSelected = true;

                OnPropertyChanged(nameof(IsCreateMode));
                return;
            }

            var dto = await _service.GetAsync(id) ?? throw new InvalidOperationException("Пользователь не найден");

            Login = dto.Login;
            FullName = dto.FullName;
            IsActive = dto.IsActive;

            foreach (var r in Roles)
                r.IsSelected = dto.Roles.Any(x => x.Equals(r.Name, StringComparison.OrdinalIgnoreCase));

            OnPropertyChanged(nameof(IsCreateMode));
        }

        [RelayCommand]
        private async Task SaveAsync()
        {
            try
            {
                ErrorText = null;

                var selectedRoles = Roles
                    .Where(x => x.IsSelected)
                    .Select(x => x.Name)
                    .ToList();

                if (selectedRoles.Count == 0)
                    throw new ArgumentException("Выберите хотя бы одну роль");

                if (Id == 0 && string.IsNullOrWhiteSpace(NewPassword))
                    throw new ArgumentException("Пароль обязателен при создании пользователя");

                var dto = new UserEditDto
                {
                    Id = Id,
                    Login = Login.Trim(),
                    FullName = string.IsNullOrWhiteSpace(FullName) ? null : FullName.Trim(),
                    IsActive = IsActive,
                    Roles = selectedRoles
                };

                if (Id == 0)
                    Id = await _service.CreateAsync(dto, NewPassword);
                else
                    await _service.UpdateAsync(dto, string.IsNullOrWhiteSpace(NewPassword) ? null : NewPassword);

                RequestClose?.Invoke(true);
            }
            catch (Exception ex)
            {
                ErrorText = ex.Message;
            }
        }
    }
}
