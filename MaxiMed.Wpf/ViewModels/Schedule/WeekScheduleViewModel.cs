using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaxiMed.Application.Appointments;
using MaxiMed.Application.Common;
using MaxiMed.Domain.Lookups;
using MaxiMed.Wpf.Services;
using MaxiMed.Wpf.ViewModels.Appointments;
using MaxiMed.Wpf.Views.Appointments;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Wpf.ViewModels.Schedule
{
    public partial class WeekScheduleViewModel : ObservableObject
    {
        private readonly IAppointmentService _service;
        private readonly IServiceProvider _sp;

        public List<LookupItemDto> Doctors { get; private set; } = new();

        [ObservableProperty] private int selectedDoctorId;
        [ObservableProperty] private DateTime weekStart = DateTime.Today;

        public ObservableCollection<WeekRowVm> Rows { get; } = new();
        public List<DateTime> Days { get; } = new();

        private readonly IExcelExportService _excel;
        public WeekScheduleViewModel(IAppointmentService service, IServiceProvider sp, IExcelExportService excel)
        {
            _service = service;
            _sp = sp;
            _excel = excel;
        }

        [RelayCommand]
        private void ExportExcel()
        {
            var doctorName = Doctors.FirstOrDefault(x => x.Id == SelectedDoctorId)?.Name ?? "Doctor";

            var dlg = new SaveFileDialog
            {
                Filter = "Excel (*.xlsx)|*.xlsx",
                FileName = $"Week_{doctorName}_{WeekStart:yyyyMMdd}.xlsx"
            };

            if (dlg.ShowDialog() != true) return;

            _excel.ExportWeekSchedule(dlg.FileName, doctorName, Days, Rows);
        }
        public async Task InitAsync()
        {
            Doctors = (await _service.GetActiveDoctorsAsync()).ToList();
            OnPropertyChanged(nameof(Doctors));

            if (Doctors.Count > 0)
                SelectedDoctorId = Doctors[0].Id;

            await LoadAsync();
        }

        [RelayCommand]
        private async Task LoadAsync()
        {
            Rows.Clear();
            Days.Clear();

            var start = weekStart.Date;
            for (int i = 0; i < 7; i++)
                Days.Add(start.AddDays(i));

            OnPropertyChanged(nameof(Days));

            // грузим все приёмы врача за неделю
            var all = new List<AppointmentDto>();
            foreach (var d in Days)
                all.AddRange(await _service.GetDayAsync(d, SelectedDoctorId));

            var workStart = start.AddHours(9);
            var workEnd = start.AddHours(18);

            for (var t = workStart; t < workEnd; t = t.AddMinutes(30))
            {
                var row = new WeekRowVm { Time = t.ToString("HH:mm") };

                foreach (var day in Days)
                {
                    var slotStart = day.Date.Add(t.TimeOfDay);
                    var slotEnd = slotStart.AddMinutes(30);

                    var hit = all.FirstOrDefault(a =>
                        a.StartAt < slotEnd &&
                        a.EndAt > slotStart &&
                        a.Status != AppointmentStatus.Canceled);

                    if (hit == null)
                    {
                        row.Slots.Add(new WeekSlotVm
                        {
                            StartAt = slotStart,
                            EndAt = slotEnd,
                            IsFree = true
                        });
                    }
                    else
                    {
                        row.Slots.Add(new WeekSlotVm
                        {
                            StartAt = slotStart,
                            EndAt = slotEnd,
                            IsFree = false,
                            AppointmentId = hit.Id,
                            PatientName = hit.PatientName
                        });
                    }
                }

                Rows.Add(row);
            }
        }

        [RelayCommand]
        private async Task AddFromSlotAsync(WeekSlotVm slot)
        {
            if (slot is null || !slot.IsFree) return;

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
                await LoadAsync();
        }
    }
}
