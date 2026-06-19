using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using TeacherPortfolio.API.DTOs;

namespace TeacherPortfolio.API.Services;

public class WordExportService
{
    public byte[] GeneratePassportWord(ModelPassportDto passport)
    {
        using var stream = new MemoryStream();
        using var wordDoc = WordprocessingDocument.Create(stream, WordprocessingDocumentType.Document);

        var mainPart = wordDoc.AddMainDocumentPart();
        mainPart.Document = new Document();
        var body = mainPart.Document.AppendChild(new Body());

        // Заголовок
        var title = body.AppendChild(new Paragraph());
        var titleRun = title.AppendChild(new Run());
        titleRun.AppendChild(new Text("МОДЕЛЬНЫЙ ПАСПОРТ"));
        titleRun.RunProperties = new RunProperties { Bold = new Bold(), FontSize = new FontSize { Val = "48" } };
        title.ParagraphProperties = new ParagraphProperties { Justification = new Justification { Val = JustificationValues.Center } };

        // ФИО
        var fio = body.AppendChild(new Paragraph());
        fio.AppendChild(new Run()).AppendChild(new Text($"ФИО: {passport.TeacherInfo.FullName}"));

        // Должность
        var position = body.AppendChild(new Paragraph());
        position.AppendChild(new Run()).AppendChild(new Text($"Должность: {passport.TeacherInfo.Position}"));

        // Итоговые баллы
        var scores = body.AppendChild(new Paragraph());
        scores.AppendChild(new Run()).AppendChild(new Text($"Итоговый балл: {passport.TotalScores.GrandTotal}"));
        scores.AppendChild(new Run()).AppendChild(new Text($" | Категория: {passport.TotalScores.RecommendedCategory}"));

        // Таблица успеваемости
        var table = new Table();
        var props = new TableProperties();
        props.Append(new TableBorders(
            new TopBorder { Val = BorderValues.Single, Size = 4 },
            new BottomBorder { Val = BorderValues.Single, Size = 4 },
            new LeftBorder { Val = BorderValues.Single, Size = 4 },
            new RightBorder { Val = BorderValues.Single, Size = 4 },
            new InsideHorizontalBorder { Val = BorderValues.Single, Size = 4 },
            new InsideVerticalBorder { Val = BorderValues.Single, Size = 4 }
        ));
        table.AppendChild(props);

        // Заголовки таблицы
        var headerRow = new TableRow();
        headerRow.AppendChild(CreateCell("Дисциплина"));
        headerRow.AppendChild(CreateCell("Качество %"));
        headerRow.AppendChild(CreateCell("Успеваемость %"));
        table.AppendChild(headerRow);

        foreach (var item in passport.Parameter1.AcademicPerformances)
        {
            var row = new TableRow();
            row.AppendChild(CreateCell(item.Discipline));
            row.AppendChild(CreateCell(item.QualityPercent.ToString()));
            row.AppendChild(CreateCell(item.SuccessPercent.ToString()));
            table.AppendChild(row);
        }

        body.AppendChild(table);
        wordDoc.Save();
        stream.Position = 0;
        return stream.ToArray();
    }

    private TableCell CreateCell(string text)
    {
        var cell = new TableCell();
        var paragraph = new Paragraph();
        var run = new Run();
        run.AppendChild(new Text(text));
        paragraph.AppendChild(run);
        cell.AppendChild(paragraph);
        cell.Append(new TableCellProperties(new TableCellWidth { Type = TableWidthUnitValues.Auto }));
        return cell;
    }
}