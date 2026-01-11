using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaxiMed.Application.Services;
using MaxiMed.Wpf.Views.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Wpf.ViewModels.Services
{
    public partial class ServicesViewModel : ObservableObject
    {
        private readonly IServiceService _service;
        private readonly IServiceProvider _sp;

        public ObservableCollection<ServiceDto> Items { get; } = new();

        [ObservableProperty] private string? searchText;
        [ObservableProperty] private ServiceDto? selected;

        public ServicesViewModel(IServiceService service, IServiceProvider sp)
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
            var win = _sp.GetRequiredService<ServiceEditWindow>();
            win.Owner = System.Windows.Application.Current.MainWindow;

            var vm = (ServiceEditViewModel)win.DataContext;
            vm.LoadFrom(null);

            if (win.ShowDialog() == true)
                await LoadAsync();
        }

        [RelayCommand]
        private async Task EditAsync()
        {
            if (Selected is null) return;

            var win = _sp.GetRequiredService<ServiceEditWindow>();
            win.Owner = System.Windows.Application.Current.MainWindow;

            var vm = (ServiceEditViewModel)win.DataContext;
            vm.LoadFrom(Selected);

            if (win.ShowDialog() == true)
                await LoadAsync();
        }

        [RelayCommand]
        private async Task DeleteAsync()
        {
            if (Selected is null) return;

            await _service.DeleteAsync(Selected.Id);
            await LoadAsync();
        }
    }
}
