using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaxiMed.Application.Appointments;
using MaxiMed.Application.Attachments;
using MaxiMed.Application.Invoices;
using MaxiMed.Application.Visits;
using MaxiMed.Wpf.Helpers;
using MaxiMed.Wpf.Api;
using MaxiMed.Wpf.ViewModels.Invoices;
using MaxiMed.Wpf.Views.Diagnoses;
using MaxiMed.Wpf.Views.Invoices;
using MaxiMed.Wpf.Views.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MaxiMed.Wpf.ViewModels.Visits
{
    public partial class VisitEditViewModel : ObservableObject
    {
        private readonly IVisitService _visitService;
        private readonly IVisitDiagnosisService _visitDx;
        private readonly IPrescriptionService _rx;
        private readonly IAppointmentServiceItemService _apptServices;
        private readonly IAppointmentService _appointmentService;
        private readonly IAttachmentService _attachments;
        private readonly IServiceProvider _sp;
        private readonly ApiClient _api;

        public event Action<bool>? RequestClose;

        [ObservableProperty] private long visitId;
        [ObservableProperty] private long appointmentId;
        [ObservableProperty] private int doctorId;
        [ObservableProperty] private int patientId;

        [ObservableProperty] private DateTime openedAt;
        [ObservableProperty] private DateTime? closedAt;
        [ObservableProperty] private string patientName = "Пациент";
        [ObservableProperty] private string doctorName = "Врач";

        [ObservableProperty] private string? complaints;
        [ObservableProperty] private string? anamnesis;
        [ObservableProperty] private string? examination;
        [ObservableProperty] private string? diagnosisText;
        [ObservableProperty] private string? recommendations;
        [ObservableProperty] private decimal servicesTotal;

        public ObservableCollection<VisitDiagnosisItemDto> Diagnoses { get; } = new();
        [ObservableProperty] private VisitDiagnosisItemDto? selectedDiagnosis;

        public ObservableCollection<PrescriptionDto> Prescriptions { get; } = new();
        [ObservableProperty] private PrescriptionDto? selectedPrescription;
        [ObservableProperty] private string? newPrescriptionText;

        public ObservableCollection<AppointmentServiceItemDto> Services { get; } = new();
        [ObservableProperty] private AppointmentServiceItemDto? selectedServiceItem;

        public ObservableCollection<AttachmentDto> Attachments { get; } = new();
        [ObservableProperty] private AttachmentDto? selectedAttachment;

        [ObservableProperty] private bool isBusy;
        [ObservableProperty] private string? errorText;

        public VisitEditViewModel(
            IVisitService visitService,
            IVisitDiagnosisService visitDx,
            IPrescriptionService rx,
            IAppointmentServiceItemService apptServices,
            IAppointmentService appointmentService,
            IAttachmentService attachments,
            IServiceProvider sp,
            ApiClient api)
        {
            _visitService = visitService;
            _visitDx = visitDx;
            _rx = rx;
            _apptServices = apptServices;
            _appointmentService = appointmentService;
            _attachments = attachments;
            _sp = sp;
            _api = api;
        }

        public async Task LoadOrCreateAsync(long apptId, int docId, int patId)
        {
            AppointmentId = apptId;
            DoctorId = docId;
            PatientId = patId;

            var dto = await _visitService.GetOrCreateByAppointmentAsync(apptId, docId);

            VisitId = dto.Id;
            OpenedAt = dto.OpenedAt;
            ClosedAt = dto.ClosedAt;

            Complaints = dto.Complaints;
            Anamnesis = dto.Anamnesis;
            Examination = dto.Examination;
            DiagnosisText = dto.DiagnosisText;
            Recommendations = dto.Recommendations;

            var patient = await _appointmentService.GetPatientLookupAsync(patId);
            PatientName = patient?.Name ?? $"Пациент №{patId}";

            DoctorName = (await _appointmentService.GetActiveDoctorsAsync())
                .FirstOrDefault(x => x.Id == docId)?.Name ?? $"Врач №{docId}";

            await ReloadDiagnosesAsync();
            await ReloadPrescriptionsAsync();
            await ReloadServicesAsync();
            await ReloadAttachmentsAsync();
        }

        private async Task ReloadServicesAsync()
        {
            Services.Clear();

            var list = await _apptServices.GetAsync(AppointmentId);

            foreach (var x in list)
                Services.Add(x);

            ServicesTotal = Services.Sum(s => s.Price);
        }

        private async Task ReloadDiagnosesAsync()
        {
            Diagnoses.Clear();

            if (VisitId <= 0)
                return;

            var list = await _visitDx.GetAsync(VisitId);

            foreach (var x in list)
                Diagnoses.Add(x);
        }

        private async Task ReloadPrescriptionsAsync()
        {
            Prescriptions.Clear();

            if (VisitId <= 0)
                return;

            var list = await _rx.GetAsync(VisitId);

            foreach (var x in list)
                Prescriptions.Add(x);
        }

        [RelayCommand]
        private async Task AddServiceAsync()
        {
            var win = _sp.GetRequiredService<ServicePickerWindow>();
            win.Owner = App.Current.MainWindow;

            if (win.ShowDialog() != true || win.Result is null)
                return;

            await _apptServices.AddAsync(AppointmentId, win.Result.Id, qty: 1);
            await ReloadServicesAsync();
        }

        [RelayCommand]
        private async Task SaveServiceRowAsync()
        {
            if (SelectedServiceItem is null)
                return;

            await _apptServices.UpdateAsync(
                SelectedServiceItem.Id,
                SelectedServiceItem.Qty,
                SelectedServiceItem.Price);

            await ReloadServicesAsync();
        }

        [RelayCommand]
        private async Task RemoveServiceAsync()
        {
            if (SelectedServiceItem is null)
                return;

            await _apptServices.DeleteAsync(SelectedServiceItem.Id);
            await ReloadServicesAsync();
        }

        [RelayCommand]
        private async Task OpenInvoiceAsync()
        {
            var win = _sp.GetRequiredService<InvoiceWindow>();
            win.Owner = System.Windows.Application.Current.MainWindow;

            var vm = (InvoiceViewModel)win.DataContext;
            await vm.LoadByAppointmentAsync(AppointmentId);

            win.ShowDialog();
        }

        [RelayCommand]
        private async Task AddPrescriptionAsync()
        {
            if (string.IsNullOrWhiteSpace(NewPrescriptionText))
                return;

            await _rx.AddAsync(VisitId, NewPrescriptionText);
            NewPrescriptionText = "";

            await ReloadPrescriptionsAsync();
        }

        [RelayCommand]
        private async Task RemovePrescriptionAsync()
        {
            if (SelectedPrescription is null)
                return;

            await _rx.DeleteAsync(SelectedPrescription.Id);
            await ReloadPrescriptionsAsync();
        }

        private async Task ReloadAttachmentsAsync()
        {
            Attachments.Clear();

            var list = await _attachments.GetByVisitAsync(VisitId);

            foreach (var x in list)
                Attachments.Add(x);
        }

        [RelayCommand]
        private async Task UploadAttachmentAsync()
        {
            var dlg = new OpenFileDialog();

            if (dlg.ShowDialog() != true)
                return;

            var data = await File.ReadAllBytesAsync(dlg.FileName);

            await _attachments.UploadAsync(
                patientId: PatientId,
                visitId: VisitId,
                fileName: Path.GetFileName(dlg.FileName),
                contentType: null,
                data: data);

            await ReloadAttachmentsAsync();
        }

        [RelayCommand]
        private async Task DownloadAttachmentAsync()
        {
            if (SelectedAttachment is null)
                return;

            var (name, _, data) = await _attachments.DownloadAsync(SelectedAttachment.Id);

            var dlg = new SaveFileDialog { FileName = name };

            if (dlg.ShowDialog() != true)
                return;

            await File.WriteAllBytesAsync(dlg.FileName, data);
        }

        [RelayCommand]
        private async Task DeleteAttachmentAsync()
        {
            if (SelectedAttachment is null)
                return;

            await _attachments.DeleteAsync(SelectedAttachment.Id);
            await ReloadAttachmentsAsync();
        }

        [RelayCommand]
        private async Task PrintPrescriptionAsync()
        {
            await SaveVisitInternalAsync();

            if (!string.IsNullOrWhiteSpace(NewPrescriptionText))
            {
                await _rx.AddAsync(VisitId, NewPrescriptionText);
                NewPrescriptionText = "";
            }

            // Берём свежие данные именно из БД через API, а не из текущего окна.
            var data = await _api.GetAsync<VisitPrintDto>($"api/visits/{VisitId}/print");

            if (data is null)
                return;

            var prescriptionText = data.Prescriptions.Count > 0
                ? string.Join(Environment.NewLine, data.Prescriptions.Select(x => $"- {x.Text}"))
                : data.Recommendations;

            PrintHelper.PrintPrescription(
                string.IsNullOrWhiteSpace(data.PatientName) ? PatientName : data.PatientName,
                string.IsNullOrWhiteSpace(data.DoctorName) ? DoctorName : data.DoctorName,
                data.Date == default ? DateTime.Now : data.Date,
                data.Complaints,
                data.DiagnosisText,
                prescriptionText);
        }

        private sealed class VisitPrintDto
        {
            public long Id { get; set; }
            public string PatientName { get; set; } = "";
            public string DoctorName { get; set; } = "";
            public DateTime Date { get; set; }
            public string? Complaints { get; set; }
            public string? DiagnosisText { get; set; }
            public string? Recommendations { get; set; }
            public List<PrescriptionPrintDto> Prescriptions { get; set; } = new();
        }

        private sealed class PrescriptionPrintDto
        {
            public string Text { get; set; } = "";
        }

        private async Task SaveVisitInternalAsync()
        {
            await _visitService.SaveAsync(new VisitDto
            {
                Id = VisitId,
                AppointmentId = AppointmentId,
                DoctorId = DoctorId,
                OpenedAt = OpenedAt,
                ClosedAt = ClosedAt,
                Complaints = Complaints,
                Anamnesis = Anamnesis,
                Examination = Examination,
                DiagnosisText = DiagnosisText,
                Recommendations = Recommendations
            });
        }

        [RelayCommand]
        private async Task SaveAsync()
        {
            await SaveVisitInternalAsync();
            RequestClose?.Invoke(true);
        }
    }
}