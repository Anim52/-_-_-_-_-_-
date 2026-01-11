using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaxiMed.Application.Common;
using MaxiMed.Wpf.Views.Specialties;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;

namespace MaxiMed.Wpf.ViewModels.Specialties
{
    public partial class SpecialtiesViewModel : ObservableObject
    {
        private readonly ISpecialtyService _service;
        private readonly IServiceProvider _sp;

        public ObservableCollection<SpecialtyDto> Items { get; } = new();

        [ObservableProperty] private string? searchText;
        [ObservableProperty] private SpecialtyDto? selected;
        [ObservableProperty] private bool isBusy;

        public SpecialtiesViewModel(ISpecialtyService service, IServiceProvider sp)
        {
            _service = service;
            _sp = sp;
        }

        private bool CanWork() => !IsBusy;
        private bool CanEdit() => !IsBusy && Selected is not null;

        partial void OnIsBusyChanged(bool value)
        {
            LoadCommand.NotifyCanExecuteChanged();
            AddCommand.NotifyCanExecuteChanged();
            EditCommand.NotifyCanExecuteChanged();
            ArchiveCommand.NotifyCanExecuteChanged();
        }

        partial void OnSelectedChanged(SpecialtyDto? value)
        {
            EditCommand.NotifyCanExecuteChanged();
            ArchiveCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand(CanExecute = nameof(CanWork))]
        public async Task LoadAsync()
        {
            try
            {
                IsBusy = true;
                Items.Clear();
                var list = await _service.SearchAsync(SearchText);
                foreach (var x in list) Items.Add(x);
            }
            finally { IsBusy = false; }
        }

        [RelayCommand(CanExecute = nameof(CanWork))]
        public async Task AddAsync()
        {
            var vm = _sp.GetRequiredService<SpecialtyEditViewModel>();
            vm.LoadFrom(new SpecialtyDto { IsActive = true }, "Новая специализация");

            var win = _sp.GetRequiredService<SpecialtyEditWindow>();
            win.Owner = System.Windows.Application.Current.MainWindow;
            win.DataContext = vm;

            vm.RequestClose += ok => { win.DialogResult = ok; win.Close(); };

            if (win.ShowDialog() == true)
            {
                await LoadAsync();
                Selected = Items.FirstOrDefault(x => x.Id == vm.Id);
            }
        }

        [RelayCommand(CanExecute = nameof(CanEdit))]
        public async Task EditAsync()
        {
            if (Selected is null) return;

            var vm = _sp.GetRequiredService<SpecialtyEditViewModel>();
            vm.LoadFrom(Selected, $"Редактирование: {Selected.Name}");

            var win = _sp.GetRequiredService<SpecialtyEditWindow>();
            win.Owner = System.Windows.Application.Current.MainWindow;
            win.DataContext = vm;

            vm.RequestClose += ok => { win.DialogResult = ok; win.Close(); };

            if (win.ShowDialog() == true)
            {
                await LoadAsync();
                Selected = Items.FirstOrDefault(x => x.Id == vm.Id);
            }
        }

        [RelayCommand(CanExecute = nameof(CanEdit))]
        public async Task ArchiveAsync()
        {
            if (Selected is null) return;
            await _service.ArchiveAsync(Selected.Id);
            await LoadAsync();
            Selected = null;
        }
    }
}
