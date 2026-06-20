using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaxiMed.Application.Invoices;
using MaxiMed.Domain.Lookups;
using MaxiMed.Wpf.Helpers;
using MaxiMed.Wpf.Api;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace MaxiMed.Wpf.ViewModels.Invoices
{
    public partial class InvoiceViewModel : ObservableObject
    {
        private readonly IInvoiceService _service;
        private readonly ApiClient _api;

        [ObservableProperty] private long appointmentId;
        [ObservableProperty] private long invoiceId;

        [ObservableProperty] private decimal totalAmount;
        [ObservableProperty] private decimal discountAmount;
        [ObservableProperty] private decimal paidAmount;

        public decimal DueAmount => TotalAmount - DiscountAmount - PaidAmount;

        public ObservableCollection<InvoiceItemDto> Items { get; } = new();
        public ObservableCollection<PaymentDto> Payments { get; } = new();

        [ObservableProperty] private PaymentMethod paymentMethod = PaymentMethod.Cash;
        [ObservableProperty] private decimal paymentAmount;
        [ObservableProperty] private string? paymentNote;
        [ObservableProperty] private DateTime paymentDate = DateTime.Today;

        [ObservableProperty] private PaymentDto? selectedPayment;
        [ObservableProperty] private DateTime selectedPaymentDate = DateTime.Today;

        [ObservableProperty] private string? errorText;
        [ObservableProperty] private bool isBusy;

        public InvoiceViewModel(IInvoiceService service, ApiClient api)
        {
            _service = service;
            _api = api;
        }

        partial void OnSelectedPaymentChanged(PaymentDto? value)
        {
            if (value != null)
                SelectedPaymentDate = value.PaidAt;
        }

        public async Task LoadByAppointmentAsync(long apptId)
        {
            AppointmentId = apptId;
            await ReloadAsync();
        }

        private async Task ReloadAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                ErrorText = null;

                var dto = await _service.GetOrCreateByAppointmentAsync(AppointmentId);

                InvoiceId = dto.Id;

                TotalAmount = dto.TotalAmount;
                DiscountAmount = dto.DiscountAmount;
                PaidAmount = dto.PaidAmount;
                OnPropertyChanged(nameof(DueAmount));

                Items.Clear();
                foreach (var x in dto.Items)
                    Items.Add(x);

                Payments.Clear();
                foreach (var p in dto.Payments)
                    Payments.Add(p);
            }
            catch (Exception ex)
            {
                ErrorText = ex.Message;
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task AddPaymentAsync()
        {
            if (InvoiceId <= 0)
                return;

            try
            {
                ErrorText = null;

                if (PaymentAmount <= 0)
                    throw new ArgumentException("Сумма должна быть больше 0");

                var paidAt = PaymentDate.Date + DateTime.Now.TimeOfDay;

                await _service.AddPaymentAsync(
                    InvoiceId,
                    PaymentMethod,
                    PaymentAmount,
                    paidAt,
                    PaymentNote);

                PaymentAmount = 0;
                PaymentNote = null;
                PaymentDate = DateTime.Today;

                await ReloadAsync();

                PrintHelper.PrintReceipt(
                    InvoiceId,
                    AppointmentId,
                    TotalAmount,
                    DiscountAmount,
                    PaidAmount,
                    DueAmount,
                    paidAt,
                    BuildItemsText(),
                    BuildPaymentsText());
            }
            catch (Exception ex)
            {
                ErrorText = ex.Message;
            }
        }

        [RelayCommand]
        private async Task UpdateSelectedPaymentDateAsync()
        {
            if (SelectedPayment is null)
                return;

            try
            {
                ErrorText = null;

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
        private async Task PrintReceiptAsync()
        {
            await ReloadAsync();

            // Берём свежие данные именно из БД через API, а не только из текущего окна.
            var data = await _api.GetAsync<InvoicePrintDto>($"api/invoices/{InvoiceId}/print");

            if (data is null)
                return;

            var itemsText = data.Items.Count > 0
                ? string.Join(Environment.NewLine, data.Items.Select(x =>
                    $"- {x.ServiceName}: {x.Qty} x {x.Price:N2} = {x.LineTotal:N2} руб."))
                : BuildItemsText();

            var paymentsText = data.Payments.Count > 0
                ? string.Join(Environment.NewLine, data.Payments.OrderBy(x => x.PaidAt).Select(x =>
                    $"- {x.PaidAt:dd.MM.yyyy HH:mm}: {x.Amount:N2} руб. ({x.Method})"))
                : BuildPaymentsText();

            PrintHelper.PrintReceipt(
                data.Id,
                AppointmentId,
                data.TotalAmount,
                data.DiscountAmount,
                data.PaidAmount,
                data.DueAmount,
                DateTime.Now,
                itemsText,
                paymentsText,
                data.PatientName);
        }

        private sealed class InvoicePrintDto
        {
            public long Id { get; set; }
            public string PatientName { get; set; } = "";
            public decimal TotalAmount { get; set; }
            public decimal DiscountAmount { get; set; }
            public decimal PaidAmount { get; set; }
            public decimal DueAmount { get; set; }
            public List<InvoiceItemPrintDto> Items { get; set; } = new();
            public List<PaymentPrintDto> Payments { get; set; } = new();
        }

        private sealed class InvoiceItemPrintDto
        {
            public string ServiceName { get; set; } = "";
            public int Qty { get; set; }
            public decimal Price { get; set; }
            public decimal LineTotal { get; set; }
        }

        private sealed class PaymentPrintDto
        {
            public decimal Amount { get; set; }
            public DateTime PaidAt { get; set; }
            public string Method { get; set; } = "";
            public string? Note { get; set; }
        }

        private string BuildItemsText()
        {
            return Items.Count > 0
                ? string.Join(Environment.NewLine,
                    Items.Select(x =>
                        $"- {x.ServiceName}: {x.Qty} x {x.Price:N2} = {x.LineTotal:N2} руб."))
                : "Услуги по счёту не добавлены.";
        }

        private string BuildPaymentsText()
        {
            return Payments.Count > 0
                ? string.Join(Environment.NewLine,
                    Payments
                        .OrderBy(x => x.PaidAt)
                        .Select(x =>
                            $"- {x.PaidAt:dd.MM.yyyy HH:mm}: {x.Amount:N2} руб. ({x.Method})"))
                : "Оплаты по счёту не добавлены.";
        }

        [RelayCommand]
        private async Task DeletePaymentAsync()
        {
            if (SelectedPayment is null)
                return;

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