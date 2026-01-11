using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaxiMed.Application.Appointments;
using MaxiMed.Application.Common;
using MaxiMed.Wpf.Views.Appointments;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Wpf.ViewModels.Appointments
{
    public partial class FreeSlotsViewModel : ObservableObject
    {
        private readonly IAppointmentService _service;
        private readonly IServiceProvider _sp;

        public List<LookupItemDto> Specialties { get; private set; } = new();

        [ObservableProperty] private int selectedSpecialtyId;

        public List<LookupItemDto> Doctors { get; private set; } = new();

        public ObservableCollection<FreeSlotDto> Slots { get; } = new();

        [ObservableProperty] private int selectedDoctorId;
        [ObservableProperty] private DateTime fromDate = DateTime.Today;
        [ObservableProperty] private DateTime toDate = DateTime.Today.AddDays(7);

        public FreeSlotsViewModel(IAppointmentService service, IServiceProvider sp)
        {
            _service = service;
            _sp = sp;
        }

        public async Task InitAsync()
        {
            Specialties = (await _service.GetActiveSpecialtiesAsync()).ToList();
            OnPropertyChanged(nameof(Specialties));

            if (Specialties.Count > 0)
                SelectedSpecialtyId = Specialties[0].Id;

            await ReloadDoctorsAsync();
        }
        private async Task ReloadDoctorsAsync()
        {
            Doctors = SelectedSpecialtyId > 0
                ? (await _service.GetDoctorsBySpecialtyAsync(SelectedSpecialtyId)).ToList()
                : (await _service.GetActiveDoctorsAsync()).ToList();

            OnPropertyChanged(nameof(Doctors));

            SelectedDoctorId = Doctors.Count > 0 ? Doctors[0].Id : 0;
        }
        partial void OnSelectedSpecialtyIdChanged(int value)
        {
            _ = ReloadDoctorsAsync();
        }


        [RelayCommand]
        private async Task SearchAsync()
        {
            Slots.Clear();

            var list = await _service.FindFreeSlotsAsync(
                SelectedDoctorId,
                FromDate,
                ToDate,
                maxResults: 30);

            foreach (var s in list)
                Slots.Add(s);
        }

        [RelayCommand]
        private async Task CreateAppointmentAsync(FreeSlotDto slot)
        {
            if (slot is null) return;

            var vm = _sp.GetRequiredService<AppointmentEditViewModel>();
            await vm.InitAsync();

            vm.LoadFrom(new AppointmentDto
            {
                DoctorId = SelectedDoctorId,
                StartAt = slot.StartAt,
                EndAt = slot.EndAt
            }, "Новая запись");

            var win = _sp.GetRequiredService<AppointmentEditWindow>();
            win.Owner = System.Windows.Application.Current.MainWindow;
            win.DataContext = vm;

            if (win.ShowDialog() == true)
                await SearchAsync();
        }
    }
}
