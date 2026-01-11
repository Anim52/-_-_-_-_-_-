using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaxiMed.Application.Patients;
using MaxiMed.Wpf.Services;
using MaxiMed.Wpf.ViewModels.Patients;
using MaxiMed.Wpf.Views.Patients;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;



namespace MaxiMed.Wpf.ViewModels
{
    public partial class PatientsViewModel : ObservableObject
    {
        private readonly IPatientService _service;
        private readonly IServiceProvider _sp;
        private readonly INavigationService _nav;
        public ObservableCollection<PatientDto> Items { get; } = new();

        [ObservableProperty] private string? searchText;
        [ObservableProperty] private PatientDto? selected;
        [ObservableProperty] private bool isBusy;

        public PatientsViewModel(IPatientService service, IServiceProvider sp, INavigationService nav)
        {
            _service = service;
            _sp = sp;
            _nav = nav;
        }

        partial void OnIsBusyChanged(bool value)
        {
            LoadCommand.NotifyCanExecuteChanged();
            RefreshCommand.NotifyCanExecuteChanged();
            AddCommand.NotifyCanExecuteChanged();
            EditCommand.NotifyCanExecuteChanged();
            DeleteCommand.NotifyCanExecuteChanged();
        }

        partial void OnSelectedChanged(PatientDto? value)
        {
            EditCommand.NotifyCanExecuteChanged();
            DeleteCommand.NotifyCanExecuteChanged();
        }

        private bool CanWork() => !IsBusy;
        private bool CanEditOrDelete() => !IsBusy && Selected is not null;

        
        [RelayCommand(CanExecute = nameof(CanWork))]
        public async Task LoadAsync()
        {
            try
            {
                IsBusy = true;

                Items.Clear();
                var list = await _service.SearchAsync(SearchText);
                foreach (var p in list) Items.Add(p);
            }
            finally
            {
                IsBusy = false;
            }
        }
        [RelayCommand]
        private async Task OpenHistoryAsync()
        {
            if (Selected is null) return;

            var page = _sp.GetRequiredService<PatientHistoryPage>();
            var vm = (PatientHistoryViewModel)page.DataContext;

            await vm.LoadAsync(Selected.Id, Selected.FullName);

            _nav.Navigate(page);
        }

        [RelayCommand]
        private async Task OpensHistoryAsync()
        {
            if (Selected is null) return;

            var win = _sp.GetRequiredService<PatientHistoryWindow>();
            win.Owner = System.Windows.Application.Current.MainWindow;

            var vm = (PatientsHistoryViewModel)win.DataContext;
            await vm.LoadAsync(Selected.Id);

            win.ShowDialog();
        }


        // ✅ Команда "Обновить" (перезагрузить без изменения SearchText)
        [RelayCommand(CanExecute = nameof(CanWork))]
        public async Task RefreshAsync()
        {
            await LoadAsync();
        }

        // ✅ Добавление через диалог
        [RelayCommand(CanExecute = nameof(CanWork))]
        public async Task AddAsync()
        {
            var vm = _sp.GetRequiredService<PatientEditViewModel>();
            vm.LoadFrom(new PatientDto(), "Новый пациент");

            var win = _sp.GetRequiredService<PatientEditWindow>();
            win.Owner = System.Windows.Application.Current.MainWindow;
            win.DataContext = vm;

            // если у тебя уже закрытие сделано в code-behind окна — можно не подписываться.
            vm.RequestClose += ok => { win.DialogResult = ok; win.Close(); };

            if (win.ShowDialog() == true)
            {
                await LoadAsync();
                Selected = Items.FirstOrDefault(x => x.Id == vm.Id);
            }
        }

        // ✅ Редактирование через диалог
        [RelayCommand(CanExecute = nameof(CanEditOrDelete))]
        public async Task EditAsync()
        {
            if (Selected is null) return;

            // подгрузим свежие данные из БД (на случай если list не всё содержит)
            var fresh = await _service.GetAsync(Selected.Id) ?? Selected;

            var vm = _sp.GetRequiredService<PatientEditViewModel>();
            vm.LoadFrom(fresh, $"Редактирование: {fresh.FullName}");

            var win = _sp.GetRequiredService<PatientEditWindow>();
            win.Owner = System.Windows.Application.Current.MainWindow;
            win.DataContext = vm;

            vm.RequestClose += ok => { win.DialogResult = ok; win.Close(); };

            if (win.ShowDialog() == true)
            {
                await LoadAsync();
                Selected = Items.FirstOrDefault(x => x.Id == vm.Id);
            }
        }

        // ✅ Удаление (пока физическое, потом можно сделать архив)
        [RelayCommand(CanExecute = nameof(CanEditOrDelete))]
        public async Task DeleteAsync()
        {
            if (Selected is null) return;

            var id = Selected.Id;

            if (System.Windows.MessageBox.Show(
                    $"Удалить пациента ID={id}?",
                    "Подтверждение",
                    System.Windows.MessageBoxButton.YesNo,
                    System.Windows.MessageBoxImage.Warning) != System.Windows.MessageBoxResult.Yes)
                return;

            await _service.DeleteAsync(id);
            await LoadAsync();
            Selected = null;
        }

    }
}
