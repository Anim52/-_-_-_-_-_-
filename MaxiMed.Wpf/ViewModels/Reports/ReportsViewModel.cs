using ClosedXML.Excel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveCharts;
using LiveCharts.Wpf;
using MaxiMed.Application.Reports;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MaxiMed.Wpf.ViewModels.Reports
{
    public partial class ReportsViewModel : ObservableObject
    {
        private readonly IReportService _service;

        // --- Табличные данные ---
        public ObservableCollection<VisitsReportRowDto> Visits { get; } = new();
        public ObservableCollection<RevenueReportRowDto> Revenue { get; } = new();
        public ObservableCollection<DoctorReportRowDto> Doctors { get; } = new();

        public ObservableCollection<GenderStatRowDto> GenderStats { get; } = new();
        public ObservableCollection<AgeStatRowDto> AgeStats { get; } = new();

        [ObservableProperty] private DateTime from = DateTime.Today.AddDays(-30);
        [ObservableProperty] private DateTime to = DateTime.Today;

        public Func<double, string> AgeFormatter => v => v.ToString("0");
        public Func<double, string> FinanceFormatter => v => v.ToString("0.00");
        public Func<double, string> VisitsFormatter => v => v.ToString("0");

        // --- Графики LiveCharts.Wpf ---
        // Пирог по полу
        public SeriesCollection GenderSeries { get; private set; } = new SeriesCollection();

        // Столбики по возрастным группам
        public SeriesCollection AgeSeries { get; private set; } = new SeriesCollection();
        public string[] AgeLabels { get; private set; } = Array.Empty<string>();

        // Линии: доход / расход
        public SeriesCollection FinanceSeries { get; private set; } = new SeriesCollection();
        public string[] FinanceLabels { get; private set; } = Array.Empty<string>();

        // Столбики: количество визитов по дням
        public SeriesCollection VisitsCountSeries { get; private set; } = new SeriesCollection();
        public string[] VisitsLabels { get; private set; } = Array.Empty<string>();

        public ReportsViewModel(IReportService service) => _service = service;

        // ================== Загрузка данных ==================

        [RelayCommand]
        private async Task LoadAsync()
        {
            Visits.Clear();
            Revenue.Clear();
            Doctors.Clear();
            GenderStats.Clear();
            AgeStats.Clear();

            // Основные отчёты
            foreach (var x in await _service.GetVisitsAsync(From, To))
                Visits.Add(x);

            foreach (var x in await _service.GetRevenueAsync(From, To))
                Revenue.Add(x);

            foreach (var x in await _service.GetDoctorsAsync(From, To))
                Doctors.Add(x);

            // Статистика по пациентам
            foreach (var x in await _service.GetGenderStatsAsync())
                GenderStats.Add(x);

            foreach (var x in await _service.GetAgeStatsAsync())
                AgeStats.Add(x);
            Summary = await _service.GetSummaryAsync(From, To);
            OnPropertyChanged(nameof(Summary));

            BuildCharts();
        }
        public SummaryReportDto Summary { get; set; }


        // ================== Построение графиков ==================

        private void BuildCharts()
        {
            BuildGenderChart();
            BuildAgeChart();
            BuildFinanceChart();
            BuildVisitsCountChart();
        }

        private void BuildGenderChart()
        {
            var series = new SeriesCollection();

            foreach (var g in GenderStats)
            {
                series.Add(new PieSeries
                {
                    Title = g.GenderName, // DTO: GenderName, Count, Percent
                    Values = new ChartValues<double> { (double)g.Count },
                    DataLabels = true,
                    LabelPoint = cp => $"{cp.Y} ({g.Percent:0.#}%)"
                });
            }

            GenderSeries = series;
            OnPropertyChanged(nameof(GenderSeries));
        }

        private void BuildAgeChart()
        {
            AgeLabels = AgeStats.Select(a => a.AgeGroup).ToArray(); // DTO: AgeGroup, Count, Percent

            AgeSeries = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Пациенты",
                    Values = new ChartValues<int>(AgeStats.Select(a => a.Count))
                }
            };

            OnPropertyChanged(nameof(AgeSeries));
            OnPropertyChanged(nameof(AgeLabels));
        }

        private void BuildFinanceChart()
        {
            if (Revenue.Count == 0)
            {
                FinanceSeries = new SeriesCollection();
                FinanceLabels = Array.Empty<string>();
                OnPropertyChanged(nameof(FinanceSeries));
                OnPropertyChanged(nameof(FinanceLabels));
                return;
            }

            var ordered = Revenue.OrderBy(r => r.Date).ToList();

            FinanceLabels = ordered.Select(r => r.Date.ToString("dd.MM")).ToArray();

            // ✅ ОКРУГЛЯЕМ ДО 2 ЗНАКОВ
            var incomeValues = new ChartValues<double>(
                ordered.Select(r => Math.Round((double)r.Paid, 2))
            );

            // условные расходы = 30% от дохода, тоже округляем
            var expenseValues = new ChartValues<double>(
                ordered.Select(r => Math.Round((double)r.Paid * 0.3, 2))
            );

            FinanceSeries = new SeriesCollection
    {
        new LineSeries
        {
            Title = "Доход (оплачено)",
            Values = incomeValues,
            PointGeometry = DefaultGeometries.Circle,
            PointGeometrySize = 6
        },
        new LineSeries
        {
            Title = "Расход (30%)",
            Values = expenseValues,
            PointGeometry = DefaultGeometries.Square,
            PointGeometrySize = 6
        }
    };

            OnPropertyChanged(nameof(FinanceSeries));
            OnPropertyChanged(nameof(FinanceLabels));
        }


        private void BuildVisitsCountChart()
        {
            if (Visits.Count == 0)
            {
                VisitsCountSeries = new SeriesCollection();
                VisitsLabels = Array.Empty<string>();
                OnPropertyChanged(nameof(VisitsCountSeries));
                OnPropertyChanged(nameof(VisitsLabels));
                return;
            }

            var groups = Visits
                .GroupBy(v => v.Date.Date)
                .OrderBy(g => g.Key)
                .ToList();

            VisitsLabels = groups.Select(g => g.Key.ToString("dd.MM")).ToArray();
            var counts = new ChartValues<int>(groups.Select(g => g.Count()));

            VisitsCountSeries = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Визитов",
                    Values = counts
                }
            };

            OnPropertyChanged(nameof(VisitsCountSeries));
            OnPropertyChanged(nameof(VisitsLabels));
        }

        private bool HasAnyRows()
            => Visits.Count > 0 || Revenue.Count > 0 || Doctors.Count > 0;

        private async Task EnsureLoadedAsync()
        {
            if (From == default) From = DateTime.Today.AddDays(-7);
            if (To == default) To = DateTime.Today;

            if (To < From)
            {
                var tmp = From;
                From = To;
                To = tmp;
            }

            if (!HasAnyRows())
                await LoadAsync();
        }

        // ================== Печать ==================

        [RelayCommand]
        private void Print(DataGrid grid)
        {
            var dlg = new PrintDialog();
            if (dlg.ShowDialog() != true) return;

            dlg.PrintVisual(grid, "Отчёт");
        }

        // ================== Экспорт в Excel ==================

        [RelayCommand]
        private async Task ExportRevenueAsync()
        {
            await EnsureLoadedAsync();

            if (Revenue.Count == 0)
            {
                MessageBox.Show(
                    "За выбранный период нет данных по выручке.",
                    "Нет данных",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }

            var dlg = new SaveFileDialog
            {
                Filter = "Excel (*.xlsx)|*.xlsx",
                FileName = $"Revenue_{From:yyyyMMdd}-{To:yyyyMMdd}.xlsx"
            };
            if (dlg.ShowDialog() != true) return;

            using var wb = new XLWorkbook();
            var ws = wb.AddWorksheet("Revenue");

            ws.Cell(1, 1).Value = "Дата";
            ws.Cell(1, 2).Value = "Всего";
            ws.Cell(1, 3).Value = "Оплачено";
            ws.Cell(1, 4).Value = "Долг";

            var r = 2;
            foreach (var x in Revenue.OrderBy(x => x.Date))
            {
                ws.Cell(r, 1).Value = x.Date;
                ws.Cell(r, 1).Style.DateFormat.Format = "dd.MM.yyyy";

                ws.Cell(r, 2).Value = x.Total;
                ws.Cell(r, 3).Value = x.Paid;
                ws.Cell(r, 4).Value = x.Debt;
                r++;
            }

            ws.Range(1, 1, 1, 4).Style.Font.Bold = true;
            ws.Columns().AdjustToContents();

            wb.SaveAs(dlg.FileName);
        }

        [RelayCommand]
        private async Task ExportVisitsAsync()
        {
            await EnsureLoadedAsync();

            if (Visits.Count == 0)
            {
                MessageBox.Show("За выбранный период нет данных по визитам..." );
                return;
            }


            var dlg = new SaveFileDialog
            {
                Filter = "Excel (*.xlsx)|*.xlsx",
                FileName = $"Visits_{From:yyyyMMdd}-{To:yyyyMMdd}.xlsx"
            };
            if (dlg.ShowDialog() != true) return;

            using var wb = new XLWorkbook();
            var ws = wb.AddWorksheet("Visits");

            ws.Cell(1, 1).Value = "Дата";
            ws.Cell(1, 2).Value = "Врач";
            ws.Cell(1, 3).Value = "Пациент";
            ws.Cell(1, 4).Value = "Статус";
            ws.Cell(1, 5).Value = "Сумма";
            ws.Cell(1, 6).Value = "Оплачено";

            var r = 2;
            foreach (var x in Visits)
            {
                ws.Cell(r, 1).Value = x.Date;
                ws.Cell(r, 1).Style.DateFormat.Format = "dd.MM.yyyy HH:mm";

                ws.Cell(r, 2).Value = x.Doctor;
                ws.Cell(r, 3).Value = x.Patient;
                ws.Cell(r, 4).Value = x.Status;
                ws.Cell(r, 5).Value = x.Total;
                ws.Cell(r, 6).Value = x.Paid;
                r++;
            }

            ws.Range(1, 1, 1, 6).Style.Font.Bold = true;
            ws.Columns().AdjustToContents();


            wb.SaveAs(dlg.FileName);
        }

        [RelayCommand]
        private async Task ExportDoctorsAsync()
        {
            await EnsureLoadedAsync();

            if (Doctors.Count == 0)
                throw new Exception("За выбранный период нет данных по врачам.");

            var dlg = new SaveFileDialog
            {
                Filter = "Excel (*.xlsx)|*.xlsx",
                FileName = $"Doctors_{From:yyyyMMdd}-{To:yyyyMMdd}.xlsx"
            };
            if (dlg.ShowDialog() != true) return;

            using var wb = new XLWorkbook();
            var ws = wb.AddWorksheet("Doctors");

            ws.Cell(1, 1).Value = "Врач";
            ws.Cell(1, 2).Value = "Количество визитов";
            ws.Cell(1, 3).Value = "Выручка";

            var r = 2;
            foreach (var x in Doctors.OrderByDescending(x => x.Revenue))
            {
                ws.Cell(r, 1).Value = x.Doctor;
                ws.Cell(r, 2).Value = x.VisitsCount;
                ws.Cell(r, 3).Value = x.Revenue;
                r++;
            }

            ws.Range(1, 1, 1, 3).Style.Font.Bold = true;
            ws.Columns().AdjustToContents();

            wb.SaveAs(dlg.FileName);
        }
    }
}
