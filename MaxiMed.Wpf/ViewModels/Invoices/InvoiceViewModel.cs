using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaxiMed.Application.Invoices;
using MaxiMed.Domain.Lookups;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace MaxiMed.Wpf.ViewModels.Invoices
{
    public partial class InvoiceViewModel : ObservableObject
    {
        private readonly IInvoiceService _service;

        [ObservableProperty] private long appointmentId;
        [ObservableProperty] private long invoiceId;

        [ObservableProperty] private decimal totalAmount;
        [ObservableProperty] private decimal discountAmount;
        [ObservableProperty] private decimal paidAmount;

        public decimal DueAmount => TotalAmount - DiscountAmount - PaidAmount;

        public ObservableCollection<InvoiceItemDto> Items { get; } = new();
        public ObservableCollection<PaymentDto> Payments { get; } = new();

        // ----- добавление платежа
        [ObservableProperty] private PaymentMethod paymentMethod = PaymentMethod.Cash;
        [ObservableProperty] private decimal paymentAmount;
        [ObservableProperty] private string? paymentNote;
        // дата платежа выбирается пользователем (по умолчанию - сегодня)
        [ObservableProperty] private DateTime paymentDate = DateTime.Today;

        // ----- редактирование выбранного платежа
        [ObservableProperty] private PaymentDto? selectedPayment;
        [ObservableProperty] private DateTime selectedPaymentDate;

        [ObservableProperty] private string? errorText;
        [ObservableProperty] private bool isBusy;

        public InvoiceViewModel(IInvoiceService service) => _service = service;

        partial void OnSelectedPaymentChanged(PaymentDto? value)
        {
            if (value != null)
                SelectedPaymentDate = value.PaidAt;
        }

        // единый метод загрузки
        public async Task LoadByAppointmentAsync(long apptId)
        {
            AppointmentId = apptId;
            await ReloadAsync();
        }

        private async Task ReloadAsync()
        {
            ErrorText = null;

            var dto = await _service.GetOrCreateByAppointmentAsync(AppointmentId);

            InvoiceId = dto.Id;

            TotalAmount = dto.TotalAmount;
            DiscountAmount = dto.DiscountAmount;
            PaidAmount = dto.PaidAmount;
            OnPropertyChanged(nameof(DueAmount));

            Items.Clear();
            foreach (var x in dto.Items) Items.Add(x);

            Payments.Clear();
            foreach (var p in dto.Payments) Payments.Add(p);
        }

        [RelayCommand]
        private async Task AddPaymentAsync()
        {
            if (InvoiceId <= 0) return;

            try
            {
                ErrorText = null;
                if (PaymentAmount <= 0) throw new ArgumentException("Сумма должна быть больше 0");

                // сохраняем локальную дату (с текущим временем, чтобы не терять сортировку)
                var paidAt = PaymentDate.Date + DateTime.Now.TimeOfDay;

                await _service.AddPaymentAsync(InvoiceId, PaymentMethod, PaymentAmount, paidAt, PaymentNote);

                PaymentAmount = 0;
                PaymentNote = null;
                PaymentDate = DateTime.Today;

                await ReloadAsync();
            }
            catch (Exception ex)
            {
                ErrorText = ex.Message;
            }
        }

        [RelayCommand]
        private async Task UpdateSelectedPaymentDateAsync()
        {
            if (SelectedPayment is null) return;

            try
            {
                ErrorText = null;

                // аналогично - сохраняем выбранную дату + текущее время
                var paidAt = SelectedPaymentDate.Date + DateTime.Now.TimeOfDay;

                await _service.UpdatePaymentDateAsync(SelectedPayment.Id, paidAt);
                await ReloadAsync();
            }
            catch (Exception ex)
            {
                ErrorText = ex.Message;
            }
        }

        [RelayCommand]
        private async Task DeletePaymentAsync()
        {
            if (SelectedPayment is null) return;

            try
            {
                ErrorText = null;
                await _service.DeletePaymentAsync(SelectedPayment.Id);
                await ReloadAsync();
            }
            catch (Exception ex)
            {
                ErrorText = ex.Message;
            }
        }
    }
}
