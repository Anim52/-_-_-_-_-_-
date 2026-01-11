using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaxiMed.Application.Users;
using MaxiMed.Wpf.Views.Admin;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Wpf.ViewModels.Admin
{
    public partial class UserListViewModel : ObservableObject
    {
        private readonly IUserAdminService _service;
        private readonly IServiceProvider _sp;

        public ObservableCollection<UserListItemDto> Items { get; } = new();

        [ObservableProperty] private string? searchText;
        [ObservableProperty] private UserListItemDto? selected;

        public UserListViewModel(IUserAdminService service, IServiceProvider sp)
        {
            _service = service;
            _sp = sp;
        }

        [RelayCommand]
        public async Task LoadAsync()
        {
            Items.Clear();
            var list = await _service.SearchAsync(SearchText);
            foreach (var x in list) Items.Add(x);
        }

        [RelayCommand]
        private async Task AddAsync()
        {
            var win = _sp.GetRequiredService<UserEditWindow>();
            win.Owner = System.Windows.Application.Current.MainWindow;

            var vm = (UserEditViewModel)win.DataContext;
            await vm.LoadAsync(0);

            if (win.ShowDialog() == true)
                await LoadAsync();
        }

        [RelayCommand]
        private async Task EditAsync()
        {
            if (Selected is null) return;

            var win = _sp.GetRequiredService<UserEditWindow>();
            win.Owner = System.Windows.Application.Current.MainWindow;

            var vm = (UserEditViewModel)win.DataContext;
            await vm.LoadAsync(Selected.Id);

            if (win.ShowDialog() == true)
                await LoadAsync();
        }
    }
}
