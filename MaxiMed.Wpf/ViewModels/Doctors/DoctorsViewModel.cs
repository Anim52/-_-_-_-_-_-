using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaxiMed.Application.Doctors;
using MaxiMed.Wpf.Views.Doctors;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;

namespace MaxiMed.Wpf.ViewModels.Doctors
{
    public partial class DoctorsViewModel : ObservableObject
    {
        private readonly IDoctorService _service;
        private readonly IServiceProvider _sp;

        public ObservableCollection<DoctorDto> Items { get; } = new();

        [ObservableProperty] private string? searchText;
        [ObservableProperty] private DoctorDto? selected;
        [ObservableProperty] private bool isBusy;

        public DoctorsViewModel(IDoctorService service, IServiceProvider sp)
        {
            _service = service;
            _sp = sp;
        }

        partial void OnIsBusyChanged(bool value)
        {
            LoadCommand.NotifyCanExecuteChanged();
            AddCommand.NotifyCanExecuteChanged();
            EditCommand.NotifyCanExecuteChanged();
            DeleteCommand.NotifyCanExecuteChanged();
        }

        partial void OnSelectedChanged(DoctorDto? value)
        {
            EditCommand.NotifyCanExecuteChanged();
            DeleteCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand(CanExecute = nameof(CanWork))]
        public async Task LoadAsync()
        {
            try
            {
                IsBusy = true;

                Items.Clear();
                var list = await _service.SearchAsync(SearchText);
                foreach (var d in list) Items.Add(d);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand(CanExecute = nameof(CanWork))]
        public async Task AddAsync()
        {
            var vm = _sp.GetRequiredService<DoctorEditViewModel>();
            await vm.InitLookupsAsync();

            // дефолтные значения
            vm.LoadFrom(new DoctorDto
            {
                IsActive = true
            }, "Новый врач");

            // если есть справочники — выберем первый, чтобы не было BranchId=0
            if (vm.Branches.Count > 0) vm.BranchId = vm.Branches[0].Id;
            if (vm.Specialties.Count > 0) vm.SpecialtyId = vm.Specialties[0].Id;

            var win = _sp.GetRequiredService<DoctorEditWindow>();
            win.Owner = System.Windows.Application.Current.MainWindow;
            win.DataContext = vm;

            vm.RequestClose += ok => { win.DialogResult = ok; win.Close(); };

            if (win.ShowDialog() == true)
            {
                await LoadAsync();
                Selected = Items.FirstOrDefault(x => x.Id == vm.Id);
            }
        }

        [RelayCommand(CanExecute = nameof(CanEditOrDelete))]
        public async Task EditAsync()
        {
            if (Selected is null) return;

            var vm = _sp.GetRequiredService<DoctorEditViewModel>();
            await vm.InitLookupsAsync();

            vm.LoadFrom(Selected, $"Редактирование: {Selected.FullName}");

            var win = _sp.GetRequiredService<DoctorEditWindow>();
            win.Owner = System.Windows.Application.Current.MainWindow;
            win.DataContext = vm;

            vm.RequestClose += ok => { win.DialogResult = ok; win.Close(); };

            if (win.ShowDialog() == true)
            {
                await LoadAsync();
                Selected = Items.FirstOrDefault(x => x.Id == vm.Id);
            }
        }

        [RelayCommand(CanExecute = nameof(CanEditOrDelete))]
        public async Task DeleteAsync()
        {
            if (Selected is null) return;

            // тут мы "архивируем" (IsActive=false), как в DoctorService.DeleteAsync
            await _service.DeleteAsync(Selected.Id);
            await LoadAsync();
            Selected = null;
        }

        private bool CanWork() => !IsBusy;
        private bool CanEditOrDelete() => !IsBusy && Selected is not null;
    }
}
