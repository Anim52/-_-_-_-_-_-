using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaxiMed.Application.Patients;
using MaxiMed.Wpf.Services;
using MaxiMed.Wpf.ViewModels.Visits;
using MaxiMed.Wpf.Views.Visits;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Wpf.ViewModels.Patients
{
    public partial class PatientsHistoryViewModel : ObservableObject
    {
        private readonly IPatientHistoryService _service;
        private readonly ISessionService _session;
        private readonly IServiceProvider _sp;

        public ObservableCollection<PatientHistoryItemDto> Items { get; } = new();

        [ObservableProperty] private PatientHistoryItemDto? selected;
        private int _patientId;

        public PatientsHistoryViewModel(IPatientHistoryService service, ISessionService session, IServiceProvider sp)
        {
            _service = service;
            _session = session;
            _sp = sp;
        }

        public async Task LoadAsync(int patientId)
        {
            _patientId = patientId;

            Items.Clear();

            var list = await _service.GetAsync(patientId);
            if (_session.IsInRole("Doctor") && _session.DoctorId.HasValue)
                list = list.Where(x => x.DoctorId == _session.DoctorId.Value).ToList();

            foreach (var x in list) Items.Add(x);
        }

        [RelayCommand]
        private async Task OpenVisitAsync()
        {
            if (Selected?.VisitId is null) return;

            var win = _sp.GetRequiredService<VisitEditWindow>();
            win.Owner = System.Windows.Application.Current.MainWindow;

            var vm = (VisitEditViewModel)win.DataContext;
            await vm.LoadOrCreateAsync(Selected.AppointmentId, docId: 0, patId: _patientId);


            win.ShowDialog();
        }
    }
}
