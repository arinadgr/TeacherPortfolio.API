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
        var workplace = body.AppendChild(new Paragraph());
        workplace.AppendChild(new Run())
            .AppendChild(new Text($"Место работы: {passport.TeacherInfo.Workplace}"));

        var email = body.AppendChild(new Paragraph());
        email.AppendChild(new Run())
            .AppendChild(new Text($"Email: {passport.TeacherInfo.Email}"));

        // Итоговые баллы
        var scores = body.AppendChild(new Paragraph());
        scores.AppendChild(new Run()).AppendChild(new Text($"Итоговый балл: {passport.TotalScores.GrandTotal}"));
        var summary = body.AppendChild(new Paragraph());
        summary.AppendChild(new Run())
            .AppendChild(new Text(
                $"Параметр I: {passport.TotalScores.Parameter1Total} | " +
                $"Параметр II: {passport.TotalScores.Parameter2Total}"
            ));
        scores.AppendChild(new Run()).AppendChild(new Text($" | Категория: {passport.TotalScores.RecommendedCategory}"));

        AddHeading(body, "ПАРАМЕТР I");

        AddHeading(body, "1.1 Успеваемость и качество знаний");

        var table = CreateTable(
            "Учебный год",
            "Дисциплина",
            "Группа",
            "Качество %",
            "Успеваемость %"
        );

        foreach (var item in passport.Parameter1.AcademicPerformances)
        {
            var row = new TableRow();

            row.Append(CreateCell(item.AcademicYear));
            row.Append(CreateCell(item.Discipline));
            row.Append(CreateCell(item.GroupName));
            row.Append(CreateCell(item.QualityPercent.ToString()));
            row.Append(CreateCell(item.SuccessPercent.ToString()));

            table.Append(row);
        }

        body.Append(table);

        AddHeading(body, "1.2 Результаты ГИА");

        var graduationTable = CreateTable(
            "Учебный год",
            "Студент",
            "Группа",
            "Тема ВКР",
            "Оценка"
        );

        foreach (var item in passport.Parameter1.GraduationResults)
        {
            var row = new TableRow();

            row.Append(CreateCell(item.AcademicYear));
            row.Append(CreateCell(item.StudentName));
            row.Append(CreateCell(item.GroupName));
            row.Append(CreateCell(item.ThesisTopic));
            row.Append(CreateCell(item.Grade));

            graduationTable.Append(row);
        }

        body.Append(graduationTable);
        AddHeading(body, "1.3 Достижения студентов");

        var achievementsTable = CreateTable(
            "Студент",
            "Мероприятие",
            "Уровень",
            "Дата",
            "Результат"
        );

        foreach (var item in passport.Parameter1.StudentAchievements)
        {
            var row = new TableRow();

            row.Append(CreateCell(item.StudentName));
            row.Append(CreateCell(item.EventName));
            row.Append(CreateCell(item.Level));
            row.Append(CreateCell(item.EventDate.ToString("dd.MM.yyyy")));
            row.Append(CreateCell(item.Result));

            achievementsTable.Append(row);
        }

        body.Append(achievementsTable);
        AddHeading(body, "ПАРАМЕТР II");

        // 2.1 Методические материалы

        AddHeading(body, "2.1 Методические материалы");

        var methodicalTable = CreateTable(
            "Учебный год",
            "Тип материала",
            "Тема"
        );

        foreach (var item in passport.Parameter2.MethodicalMaterials)
        {
            var row = new TableRow();

            row.Append(CreateCell(item.AcademicYear));
            row.Append(CreateCell(item.MaterialType));
            row.Append(CreateCell(item.Topic));

            methodicalTable.Append(row);
        }

        body.Append(methodicalTable);


        // 2.2 Электронные образовательные ресурсы

        AddHeading(body, "2.2 Электронные образовательные ресурсы");

        var eorTable = CreateTable(
            "Учебный год",
            "Название",
            "Тема",
            "Форма взаимодействия"
        );

        foreach (var item in passport.Parameter2.ElectronicResources)
        {
            var row = new TableRow();

            row.Append(CreateCell(item.AcademicYear));
            row.Append(CreateCell(item.Name));
            row.Append(CreateCell(item.Topic));
            row.Append(CreateCell(item.InteractionForm));

            eorTable.Append(row);
        }

        body.Append(eorTable);


        // 2.3 Транслирование опыта

        AddHeading(body, "2.3 Транслирование опыта");

        var experienceTable = CreateTable(
            "Мероприятие",
            "Тема",
            "Уровень",
            "Дата"
        );

        foreach (var item in passport.Parameter2.ExperienceSharings)
        {
            var row = new TableRow();

            row.Append(CreateCell(item.EventName));
            row.Append(CreateCell(item.Topic));
            row.Append(CreateCell(item.Level));
            row.Append(CreateCell(item.EventDate.ToString("dd.MM.yyyy")));

            experienceTable.Append(row);
        }

        body.Append(experienceTable);


        // 2.4 Конкурсы профессионального мастерства

        AddHeading(body, "2.4 Конкурсы профессионального мастерства");

        var contestsTable = CreateTable(
            "Учебный год",
            "Конкурс",
            "Организатор",
            "Результат"
        );

        foreach (var item in passport.Parameter2.TeacherContests)
        {
            var row = new TableRow();

            row.Append(CreateCell(item.AcademicYear));
            row.Append(CreateCell(item.ContestName));
            row.Append(CreateCell(item.Organizer));
            row.Append(CreateCell(item.Result));

            contestsTable.Append(row);
        }

        body.Append(contestsTable);


        // 2.5 Экспертная деятельность

        AddHeading(body, "2.5 Экспертная деятельность");

        var expertTable = CreateTable(
            "Мероприятие",
            "Уровень",
            "Тип деятельности",
            "Дата"
        );

        foreach (var item in passport.Parameter2.ExpertActivities)
        {
            var row = new TableRow();

            row.Append(CreateCell(item.EventName));
            row.Append(CreateCell(item.Level));
            row.Append(CreateCell(item.ActivityType));
            row.Append(CreateCell(item.EventDate?.ToString("dd.MM.yyyy") ?? ""));

            expertTable.Append(row);
        }

        body.Append(expertTable);


        // 2.6 Образовательные технологии

        AddHeading(body, "2.6 Образовательные технологии");

        var technologiesTable = CreateTable(
            "Технология",
            "Цель",
            "Результат"
        );

        foreach (var item in passport.Parameter2.EducationalTechnologies)
        {
            var row = new TableRow();

            row.Append(CreateCell(item.TechnologyName));
            row.Append(CreateCell(item.Purpose));
            row.Append(CreateCell(item.Result));

            technologiesTable.Append(row);
        }

        body.Append(technologiesTable);

        body.Append(new Paragraph(new Run(new Text(""))));

        body.Append(new Paragraph(
            new Run(new Text(DateTime.Now.ToString("dd.MM.yyyy")))
        ));

        body.Append(new Paragraph(
            new Run(new Text(
                "Работодатель _________________ (Ф.И.О. работодателя)"
            ))
        ));

        body.Append(new Paragraph(
            new Run(new Text(
                "Руководитель структурного подразделения _________________ (Ф.И.О. руководителя структурного подразделения)"
            ))
        ));

        body.Append(new Paragraph(
            new Run(new Text(
                "подтверждают достоверность представленной информации"
            ))
        ));

        body.Append(new Paragraph(
            new Run(new Text(
                $"{passport.TeacherInfo.FullName} (Ф.И.О. педагогического работника)"
            ))
        ));

        body.Append(new Paragraph(
            new Run(new Text(
                $"аттестуемого(ой) с целью установления {passport.TotalScores.RecommendedCategory} по должности «{passport.TeacherInfo.Position}»"
            ))
        ));

        body.Append(new Paragraph(
            new Run(new Text(
                $"{DateTime.Now:dd.MM.yyyy} ____________________ (подпись руководителя структурного подразделения)"
            ))
        ));

        body.Append(new Paragraph(
            new Run(new Text(
                $"{DateTime.Now:dd.MM.yyyy} ____________________ (подпись работодателя)"
            ))
        ));

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
        cell.Append(new TableCellProperties(new TableCellWidth
        {
            Type = TableWidthUnitValues.Dxa,
            Width = "2500"
        }));
        return cell;
    }
    private void AddHeading(Body body, string text)
    {
        var p = body.AppendChild(new Paragraph());

        p.ParagraphProperties = new ParagraphProperties(
            new SpacingBetweenLines { Before = "200", After = "100" });

        var run = p.AppendChild(new Run());

        run.RunProperties = new RunProperties(
            new Bold(),
            new FontSize { Val = "28" });

        run.AppendChild(new Text(text));
    }

    private Table CreateTable(params string[] headers)
    {
        var table = new Table();

        table.AppendChild(
            new TableProperties(
                new TableBorders(
                    new TopBorder { Val = BorderValues.Single, Size = 4 },
                    new BottomBorder { Val = BorderValues.Single, Size = 4 },
                    new LeftBorder { Val = BorderValues.Single, Size = 4 },
                    new RightBorder { Val = BorderValues.Single, Size = 4 },
                    new InsideHorizontalBorder { Val = BorderValues.Single, Size = 4 },
                    new InsideVerticalBorder { Val = BorderValues.Single, Size = 4 }
                )
            )
        );

        var headerRow = new TableRow();

        foreach (var header in headers)
            headerRow.Append(CreateCell(header));

        table.Append(headerRow);

        return table;
    }
}