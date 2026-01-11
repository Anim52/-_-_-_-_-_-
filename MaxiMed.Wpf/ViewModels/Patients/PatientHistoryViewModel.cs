using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaxiMed.Application.Appointments;
using MaxiMed.Application.Common;
using MaxiMed.Domain.Entities;
using MaxiMed.Wpf.ViewModels.Appointments;
using MaxiMed.Wpf.Views.Appointments;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Wpf.ViewModels.Patients
{
    public partial class PatientHistoryViewModel : ObservableObject
    {
        private readonly IAppointmentService _service;
        private readonly IServiceProvider _sp;

        public ObservableCollection<AppointmentDto> Items { get; } = new();

        [ObservableProperty] private LookupItemDto? patient;

        public PatientHistoryViewModel(IAppointmentService service, IServiceProvider sp)
        {
            _service = service;
            _sp = sp;
        }

        public async Task LoadAsync(int patientId, string patientName)
        {
            Patient = new LookupItemDto { Id = patientId, Name = patientName };

            Items.Clear();
            var list = await _service.GetByPatientAsync(patientId);
            foreach (var x in list)
                Items.Add(x);
        }

        [RelayCommand]
        private async Task OpenAppointmentAsync(AppointmentDto dto)
        {
            if (dto is null) return;

            var vm = _sp.GetRequiredService<AppointmentEditViewModel>();
            await vm.InitAsync();
            vm.LoadFrom(dto, "Приём пациента");

            var win = _sp.GetRequiredService<AppointmentEditWindow>();
            win.Owner = System.Windows.Application.Current.MainWindow;
            win.DataContext = vm;
            win.ShowDialog();
        }
    }
}
