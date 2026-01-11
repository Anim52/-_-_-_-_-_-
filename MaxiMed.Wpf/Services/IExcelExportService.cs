using ClosedXML.Excel;
using MaxiMed.Wpf.ViewModels.Schedule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Wpf.Services
{
    public interface IExcelExportService
    {
        void ExportWeekSchedule(string filePath, string doctorName, IReadOnlyList<DateTime> days, IReadOnlyList<WeekRowVm> rows);
    }

    public sealed class ExcelExportService : IExcelExportService
    {
        public void ExportWeekSchedule(string filePath, string doctorName, IReadOnlyList<DateTime> days, IReadOnlyList<WeekRowVm> rows)
        {
            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Week");

            ws.Cell(1, 1).Value = "Врач:";
            ws.Cell(1, 2).Value = doctorName;

            // Заголовки
            ws.Cell(3, 1).Value = "Время";
            for (int d = 0; d < days.Count; d++)
            {
                ws.Cell(3, 2 + d).Value = days[d].ToString("ddd dd.MM");
            }

            ws.Range(3, 1, 3, 1 + days.Count).Style.Font.Bold = true;
            ws.Range(3, 1, 3, 1 + days.Count).Style.Fill.BackgroundColor = XLColor.LightGray;

            // Данные
            int row = 4;
            foreach (var r in rows)
            {
                ws.Cell(row, 1).Value = r.Time;

                for (int d = 0; d < r.Slots.Count; d++)
                {
                    var slot = r.Slots[d];
                    var cell = ws.Cell(row, 2 + d);

                    if (slot.IsFree)
                    {
                        cell.Value = "Свободно";
                        cell.Style.Fill.BackgroundColor = XLColor.FromArgb(220, 255, 220);
                    }
                    else
                    {
                        cell.Value = slot.PatientName ?? "Занято";
                        cell.Style.Fill.BackgroundColor = XLColor.FromArgb(255, 220, 220);
                    }

                    cell.Style.Alignment.WrapText = true;
                }

                row++;
            }

            ws.Columns().AdjustToContents();
            ws.SheetView.FreezeRows(3);
            wb.SaveAs(filePath);
        }
    }

}
