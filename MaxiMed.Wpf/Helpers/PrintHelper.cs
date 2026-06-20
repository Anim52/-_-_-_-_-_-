using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace MaxiMed.Wpf.Helpers;

public static class PrintHelper
{
    public static void PrintText(string title, string text)
    {
        var doc = CreateBaseDocument(title);
        AddSection(doc, title, text);
        Print(title, doc);
    }

    public static void PrintTicket(
        string? patientName,
        string? doctorName,
        string? branchName,
        DateTime startAt,
        DateTime endAt,
        string? room = null)
    {
        var doc = CreateBaseDocument("ТАЛОН НА ПРИЁМ");

        doc.Blocks.Add(new Paragraph(new Run("Медицинская информационная система MaxiMed"))
        {
            FontSize = 12,
            Foreground = Brushes.Gray,
            TextAlignment = TextAlignment.Center,
            Margin = new Thickness(0, 0, 0, 18)
        });

        AddInfoRow(doc, "Пациент", patientName);
        AddInfoRow(doc, "Врач", doctorName);
        AddInfoRow(doc, "Филиал", branchName);
        AddInfoRow(doc, "Кабинет", room);
        AddInfoRow(doc, "Дата", startAt == default ? null : startAt.ToString("dd.MM.yyyy"));
        AddInfoRow(doc, "Время", startAt == default ? null : $"{startAt:HH:mm} – {endAt:HH:mm}");

        doc.Blocks.Add(new Paragraph(new Run("Пожалуйста, подойдите к регистратуре за 10 минут до начала приёма."))
        {
            FontSize = 12,
            Foreground = Brushes.DimGray,
            Margin = new Thickness(0, 18, 0, 0),
            TextAlignment = TextAlignment.Center
        });

        Print("Талон на приём", doc);
    }

    public static void PrintPrescription(
        string? patientName,
        string? doctorName,
        DateTime visitDate,
        string? complaints,
        string? diagnosis,
        string? recommendations)
    {
        var doc = CreateBaseDocument("НАЗНАЧЕНИЕ ВРАЧА");

        AddInfoRow(doc, "Пациент", patientName);
        AddInfoRow(doc, "Врач", doctorName);
        AddInfoRow(doc, "Дата приёма", visitDate == default ? null : visitDate.ToString("dd.MM.yyyy HH:mm"));

        AddSection(doc, "Жалобы", complaints);
        AddSection(doc, "Диагноз", diagnosis);
        AddSection(doc, "Рекомендации и назначения", recommendations);

        doc.Blocks.Add(new Paragraph(new Run("Подпись врача: ____________________________"))
        {
            FontSize = 13,
            Margin = new Thickness(0, 24, 0, 0),
            TextAlignment = TextAlignment.Right
        });

        Print("Назначение врача", doc);
    }

    public static void PrintReceipt(
        long invoiceId,
        long appointmentId,
        decimal totalAmount,
        decimal discountAmount,
        decimal paidAmount,
        decimal dueAmount,
        DateTime paidAt,
        string? itemsText = null,
        string? paymentsText = null,
        string? patientName = null)
    {
        var doc = CreateBaseDocument("ЧЕК ОБ ОПЛАТЕ");

        AddInfoRow(doc, "Номер счёта", invoiceId.ToString());
        AddInfoRow(doc, "Запись", appointmentId.ToString());
        AddInfoRow(doc, "Пациент", patientName);
        AddInfoRow(doc, "Дата", paidAt.ToString("dd.MM.yyyy HH:mm"));
        AddInfoRow(doc, "Стоимость", $"{totalAmount:N2} руб.");
        AddInfoRow(doc, "Скидка", $"{discountAmount:N2} руб.");
        AddInfoRow(doc, "Оплачено", $"{paidAmount:N2} руб.");
        AddInfoRow(doc, "Остаток", $"{dueAmount:N2} руб.");

        AddSection(doc, "Услуги", itemsText);
        AddSection(doc, "Оплаты", paymentsText);

        doc.Blocks.Add(new Paragraph(new Run("Спасибо за обращение!"))
        {
            FontSize = 13,
            Margin = new Thickness(0, 18, 0, 0),
            TextAlignment = TextAlignment.Center
        });

        Print("Чек об оплате", doc);
    }

    private static FlowDocument CreateBaseDocument(string title)
    {
        var doc = new FlowDocument
        {
            PagePadding = new Thickness(48),
            FontFamily = new FontFamily("Segoe UI"),
            FontSize = 14,
            ColumnWidth = double.PositiveInfinity
        };

        doc.Blocks.Add(new Paragraph(new Run(title))
        {
            FontSize = 24,
            FontWeight = FontWeights.Bold,
            TextAlignment = TextAlignment.Center,
            Margin = new Thickness(0, 0, 0, 18)
        });

        return doc;
    }

    private static void AddInfoRow(FlowDocument doc, string label, string? value)
    {
        var paragraph = new Paragraph
        {
            Margin = new Thickness(0, 0, 0, 8),
            BorderBrush = Brushes.LightGray,
            BorderThickness = new Thickness(0, 0, 0, 1),
            Padding = new Thickness(0, 0, 0, 6)
        };

        paragraph.Inlines.Add(new Run(label + ": ") { FontWeight = FontWeights.SemiBold });
        paragraph.Inlines.Add(new Run(string.IsNullOrWhiteSpace(value) ? "не указано" : value.Trim()));
        doc.Blocks.Add(paragraph);
    }

    private static void AddSection(FlowDocument doc, string title, string? value)
    {
        doc.Blocks.Add(new Paragraph(new Run(title))
        {
            FontSize = 15,
            FontWeight = FontWeights.Bold,
            Margin = new Thickness(0, 14, 0, 4)
        });
        doc.Blocks.Add(new Paragraph(new Run(string.IsNullOrWhiteSpace(value) ? "Данные не заполнены." : value))
        {
            LineHeight = 22,
            Margin = new Thickness(0, 0, 0, 2)
        });
    }

    private static void Print(string title, FlowDocument doc)
    {
        var dialog = new PrintDialog();
        if (dialog.ShowDialog() == true)
            dialog.PrintDocument(((IDocumentPaginatorSource)doc).DocumentPaginator, title);
    }
}
