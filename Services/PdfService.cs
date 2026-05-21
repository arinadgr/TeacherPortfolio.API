using System;
using System.Collections.Generic;
using System.Linq;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using TeacherPortfolio.API.DTOs;

namespace TeacherPortfolio.API.Services
{
    public class PdfService
    {
        public PdfService()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }


        // Уже существующий метод для достижений студентов
        public byte[] GenerateAchievementsReport(TeacherProfileResponse profile)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header()
                        .Text("Отчет о достижениях преподавателя")
                        .SemiBold().FontSize(20).FontColor(Colors.Blue.Darken2);

                    page.Content().PaddingVertical(20).Column(col =>
                    {
                        col.Item().Text($"ФИО: {profile.FullName}");
                        col.Item().Text($"Должность: {profile.Position}");
                        col.Item().Text($"Место работы: {profile.Workplace}");
                        col.Item().Text($"Email: {profile.Email}");
                        col.Item().PaddingTop(10).LineHorizontal(0.5f);

                        col.Item().PaddingTop(10).Text("Достижения студентов").SemiBold().FontSize(16);

                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(2);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Text("Студент").Bold();
                                header.Cell().Text("Мероприятие").Bold();
                                header.Cell().Text("Уровень").Bold();
                                header.Cell().Text("Дата").Bold();
                                header.Cell().Text("Результат").Bold();
                            });

                            foreach (var item in profile.Achievements)
                            {
                                table.Cell().Text(item.StudentName);
                                table.Cell().Text(item.AchievementType);
                                table.Cell().Text(item.Level);
                                table.Cell().Text(item.EventDate.ToString("dd.MM.yyyy"));
                                table.Cell().Text(item.Result ?? "-");
                            }
                        });
                    });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Сгенерировано: ");
                            x.Span(DateTime.Now.ToString("dd.MM.yyyy HH:mm"));
                        });
                });
            });

            return document.GeneratePdf();
        }

        // 1. Успеваемость (AcademicPerformance)
        public byte[] GenerateAcademicPerformancePdf(List<AcademicPerformanceDto> data, string teacherName, string teacherPosition)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    page.Header()
                        .Text("Отчёт по успеваемости и качеству знаний")
                        .SemiBold().FontSize(18).FontColor(Colors.Blue.Darken2);

                    page.Content().PaddingVertical(15).Column(col =>
                    {
                        col.Item().Text($"Преподаватель: {teacherName}");
                        col.Item().Text($"Должность: {teacherPosition}");
                        col.Item().PaddingTop(10).LineHorizontal(0.5f);

                        col.Item().PaddingTop(10).Text("Результаты успеваемости").SemiBold().FontSize(14);

                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Text("Учебный год").Bold();
                                header.Cell().Text("Дисциплина").Bold();
                                header.Cell().Text("Группа").Bold();
                                header.Cell().Text("Качество %").Bold();
                                header.Cell().Text("Успеваемость %").Bold();
                            });

                            foreach (var item in data)
                            {
                                table.Cell().Text(item.AcademicYearName);
                                table.Cell().Text(item.Discipline);
                                table.Cell().Text(item.GroupName);
                                table.Cell().Text(Convert.ToSingle(item.QualityPercent).ToString("F1"));
                                table.Cell().Text(Convert.ToSingle(item.SuccessPercent).ToString("F1"));
                            }
                        });
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Сформировано: ");
                        x.Span(DateTime.Now.ToString("dd.MM.yyyy HH:mm"));
                    });
                });
            });

            return document.GeneratePdf();
        }

        // 2. Результаты ГИА (GraduationResult)
        public byte[] GenerateGraduationResultPdf(List<GraduationResultDto> data, string teacherName, string teacherPosition)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    page.Header()
                        .Text("Отчёт по результатам ГИА (руководство ВКР)")
                        .SemiBold().FontSize(18).FontColor(Colors.Blue.Darken2);

                    page.Content().PaddingVertical(15).Column(col =>
                    {
                        col.Item().Text($"Преподаватель: {teacherName}");
                        col.Item().Text($"Должность: {teacherPosition}");
                        col.Item().PaddingTop(10).LineHorizontal(0.5f);

                        col.Item().PaddingTop(10).Text("Результаты итоговой аттестации").SemiBold().FontSize(14);

                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(1);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Text("Учебный год").Bold();
                                header.Cell().Text("Студент").Bold();
                                header.Cell().Text("Группа").Bold();
                                header.Cell().Text("Тема ВКР").Bold();
                                header.Cell().Text("Оценка").Bold();
                            });

                            foreach (var item in data)
                            {
                                table.Cell().Text(item.AcademicYearName);
                                table.Cell().Text(item.StudentName);
                                table.Cell().Text(item.GroupName);
                                table.Cell().Text(item.ThesisTopic);
                                table.Cell().Text(item.Grade);
                            }
                        });
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Сформировано: ");
                        x.Span(DateTime.Now.ToString("dd.MM.yyyy HH:mm"));
                    });
                });
            });

            return document.GeneratePdf();
        }

        // 3. Методические материалы
        public byte[] GenerateMethodicalMaterialPdf(List<MethodicalMaterialDto> data, string teacherName, string teacherPosition)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    page.Header()
                        .Text("Отчёт по программно-методическим материалам")
                        .SemiBold().FontSize(18).FontColor(Colors.Blue.Darken2);

                    page.Content().PaddingVertical(15).Column(col =>
                    {
                        col.Item().Text($"Преподаватель: {teacherName}");
                        col.Item().Text($"Должность: {teacherPosition}");
                        col.Item().PaddingTop(10).LineHorizontal(0.5f);

                        col.Item().PaddingTop(10).Text("Методические материалы").SemiBold().FontSize(14);

                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(3);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Text("Учебный год").Bold();
                                header.Cell().Text("Тип материала").Bold();
                                header.Cell().Text("Специальность").Bold();
                                header.Cell().Text("Тема / Название").Bold();
                            });

                            foreach (var item in data)
                            {
                                table.Cell().Text(item.AcademicYearName);
                                table.Cell().Text(item.MaterialTypeName);
                                table.Cell().Text(item.Specialty ?? "—");
                                table.Cell().Text(item.Topic);
                            }
                        });
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Сформировано: ");
                        x.Span(DateTime.Now.ToString("dd.MM.yyyy HH:mm"));
                    });
                });
            });

            return document.GeneratePdf();
        }

        // 4. Электронные ресурсы
        public byte[] GenerateElectronicResourcePdf(List<ElectronicResourceDto> data, string teacherName, string teacherPosition)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    page.Header()
                        .Text("Отчёт по электронным образовательным ресурсам")
                        .SemiBold().FontSize(18).FontColor(Colors.Blue.Darken2);

                    page.Content().PaddingVertical(15).Column(col =>
                    {
                        col.Item().Text($"Преподаватель: {teacherName}");
                        col.Item().Text($"Должность: {teacherPosition}");
                        col.Item().PaddingTop(10).LineHorizontal(0.5f);

                        col.Item().PaddingTop(10).Text("Электронные ресурсы").SemiBold().FontSize(14);

                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(3);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Text("Учебный год").Bold();
                                header.Cell().Text("Название").Bold();
                                header.Cell().Text("Тема").Bold();
                                header.Cell().Text("Форма взаимодействия").Bold();
                            });

                            foreach (var item in data)
                            {
                                table.Cell().Text(item.AcademicYearName);
                                table.Cell().Text(item.Name);
                                table.Cell().Text(item.Topic);
                                table.Cell().Text(item.InteractionForm);
                            }
                        });
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Сформировано: ");
                        x.Span(DateTime.Now.ToString("dd.MM.yyyy HH:mm"));
                    });
                });
            });

            return document.GeneratePdf();
        }

        // 5. Трансляция опыта
        public byte[] GenerateExperienceSharingPdf(List<ExperienceSharingDto> data, string teacherName, string teacherPosition)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    page.Header()
                        .Text("Отчёт по трансляции опыта")
                        .SemiBold().FontSize(18).FontColor(Colors.Blue.Darken2);

                    page.Content().PaddingVertical(15).Column(col =>
                    {
                        col.Item().Text($"Преподаватель: {teacherName}");
                        col.Item().Text($"Должность: {teacherPosition}");
                        col.Item().PaddingTop(10).LineHorizontal(0.5f);

                        col.Item().PaddingTop(10).Text("Мероприятия").SemiBold().FontSize(14);

                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Text("Дата").Bold();
                                header.Cell().Text("Мероприятие").Bold();
                                header.Cell().Text("Уровень").Bold();
                                header.Cell().Text("Формат").Bold();
                                header.Cell().Text("Тема").Bold();
                            });

                            foreach (var item in data)
                            {
                                table.Cell().Text(item.EventDate.ToString("dd.MM.yyyy"));
                                table.Cell().Text(item.EventName);
                                table.Cell().Text(item.LevelName ?? "—");
                                table.Cell().Text(item.FormatName ?? "—");
                                table.Cell().Text(item.Topic);
                            }
                        });
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Сформировано: ");
                        x.Span(DateTime.Now.ToString("dd.MM.yyyy HH:mm"));
                    });
                });
            });

            return document.GeneratePdf();
        }

        // 6. Конкурсы педмастерства
        public byte[] GenerateTeacherContestPdf(List<TeacherContestDto> data, string teacherName, string teacherPosition)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    page.Header()
                        .Text("Отчёт по конкурсам педагогического мастерства")
                        .SemiBold().FontSize(18).FontColor(Colors.Blue.Darken2);

                    page.Content().PaddingVertical(15).Column(col =>
                    {
                        col.Item().Text($"Преподаватель: {teacherName}");
                        col.Item().Text($"Должность: {teacherPosition}");
                        col.Item().PaddingTop(10).LineHorizontal(0.5f);

                        col.Item().PaddingTop(10).Text("Участие в конкурсах").SemiBold().FontSize(14);

                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Text("Учебный год").Bold();
                                header.Cell().Text("Конкурс").Bold();
                                header.Cell().Text("Уровень").Bold();
                                header.Cell().Text("Результат").Bold();
                            });

                            foreach (var item in data)
                            {
                                table.Cell().Text(item.AcademicYearName);
                                table.Cell().Text(item.ContestName);
                                table.Cell().Text(item.Level);
                                table.Cell().Text(item.Result);
                            }
                        });
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Сформировано: ");
                        x.Span(DateTime.Now.ToString("dd.MM.yyyy HH:mm"));
                    });
                });
            });

            return document.GeneratePdf();
        }

        // 7. Экспертная деятельность
        public byte[] GenerateExpertActivityPdf(List<ExpertActivityDto> data, string teacherName, string teacherPosition)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    page.Header()
                        .Text("Отчёт по экспертной деятельности")
                        .SemiBold().FontSize(18).FontColor(Colors.Blue.Darken2);

                    page.Content().PaddingVertical(15).Column(col =>
                    {
                        col.Item().Text($"Преподаватель: {teacherName}");
                        col.Item().Text($"Должность: {teacherPosition}");
                        col.Item().PaddingTop(10).LineHorizontal(0.5f);

                        col.Item().PaddingTop(10).Text("Экспертная активность").SemiBold().FontSize(14);

                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(3);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Text("Дата").Bold();
                                header.Cell().Text("Мероприятие").Bold();
                                header.Cell().Text("Уровень").Bold();
                                header.Cell().Text("Тип деятельности").Bold();
                            });

                            foreach (var item in data)
                            {
                                table.Cell().Text(item.EventDate?.ToString("dd.MM.yyyy") ?? "—");
                                table.Cell().Text(item.EventName);
                                table.Cell().Text(item.LevelName ?? "—");
                                table.Cell().Text(item.ActivityType ?? "—");
                            }
                        });
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Сформировано: ");
                        x.Span(DateTime.Now.ToString("dd.MM.yyyy HH:mm"));
                    });
                });
            });

            return document.GeneratePdf();
        }

        // 8. Образовательные технологии
        public byte[] GenerateEducationalTechnologyPdf(List<EducationalTechnologyDto> data, string teacherName, string teacherPosition)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    page.Header()
                        .Text("Отчёт по образовательным технологиям")
                        .SemiBold().FontSize(18).FontColor(Colors.Blue.Darken2);

                    page.Content().PaddingVertical(15).Column(col =>
                    {
                        col.Item().Text($"Преподаватель: {teacherName}");
                        col.Item().Text($"Должность: {teacherPosition}");
                        col.Item().PaddingTop(10).LineHorizontal(0.5f);

                        col.Item().PaddingTop(10).Text("Используемые технологии").SemiBold().FontSize(14);

                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(4);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Text("Технология").Bold();
                                header.Cell().Text("Цель использования").Bold();
                                header.Cell().Text("Результат").Bold();
                            });

                            foreach (var item in data)
                            {
                                table.Cell().Text(item.TechnologyName);
                                table.Cell().Text(item.Purpose ?? "—");
                                table.Cell().Text(item.Result ?? "—");
                            }
                        });
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Сформировано: ");
                        x.Span(DateTime.Now.ToString("dd.MM.yyyy HH:mm"));
                    });
                });
            });

            return document.GeneratePdf();
        }

            public byte[] GeneratePassportPdf(ModelPassportDto passport)
            {
                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(2f, Unit.Centimetre);
                        page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Times New Roman"));

                        page.Content().Column(col =>
                        {
                            // Шапка
                            col.Item().AlignCenter().Text("Результаты профессиональной деятельности педагогических работников,\nаттестуемых в целях установления квалификационной категории\nпо должностям «мастер производственного обучения», «преподаватель»,\n«руководитель физического воспитания»")
                                .FontSize(12).Bold();

                            col.Item().PaddingTop(10).LineHorizontal(0.5f);

                            // Информация о преподавателе
                            col.Item().PaddingTop(5).Text($"Фамилия, имя, отчество {passport.TeacherInfo.FullName}");
                            col.Item().Text($"Должность, место работы {passport.TeacherInfo.Position}, {passport.TeacherInfo.Workplace}");
                            col.Item().Text($"Наличие квалификационной категории по должности, срок её действия {passport.TeacherInfo.QualificationCategory ?? "—"}");
                            col.Item().Text($"Заявленная квалификационная категория {passport.TeacherInfo.QualificationCategory ?? "высшая"}");

                            col.Item().PaddingTop(5).Text("Документы, материалы, информация, подтверждающие профессиональную деятельность").Bold();

                            // Параметр I
                            col.Item().PaddingTop(10).Text("Параметр I. Результаты освоения обучающимися образовательных программ").Bold();
                            col.Item().PaddingTop(5).Text("1.1. 1.1.1 Таблица с указанием итоговых результатов").Bold();
                            AddAcademicPerformancePassportTable(col, passport.Parameter1.AcademicPerformances);

                            col.Item().PaddingTop(10).Text("1.2. 1.2.1 Таблица с указанием результатов").Bold();
                            AddGraduationResultPassportTable(col, passport.Parameter1.GraduationResults);

                            col.Item().PaddingTop(10).Text("1.3. 1.3.1 Таблица с указанием результатов").Bold();
                            AddStudentAchievementsPassportTable(col, passport.Parameter1.StudentAchievements);

                            // Параметр II
                            col.Item().PaddingTop(15).Text("Параметр II. Личный вклад педагогического работника в повышение качества образования").Bold();
                            col.Item().PaddingTop(5).Text("2.1. 2.1.1. Таблица с указанием результатов").Bold();
                            AddMethodicalMaterialsPassportTable(col, passport.Parameter2.MethodicalMaterials);

                            col.Item().PaddingTop(10).Text("2.1.3. Таблица с указанием результатов").Bold();
                            AddElectronicResourcesPassportTable(col, passport.Parameter2.ElectronicResources);

                            col.Item().PaddingTop(10).Text("2.3. 2.3.1 Таблица с указанием результатов").Bold();
                            AddExperienceSharingPassportTable(col, passport.Parameter2.ExperienceSharings);

                            col.Item().PaddingTop(10).Text("2.4. 2.4.4. Таблица с указанием результатов").Bold();
                            AddTeacherContestsPassportTable(col, passport.Parameter2.TeacherContests);

                            col.Item().PaddingTop(10).Text("2.5. Таблица с указанием результатов").Bold();
                            AddExpertActivitiesPassportTable(col, passport.Parameter2.ExpertActivities);

                            col.Item().PaddingTop(10).Text("2.6. 2.6.1 Таблица с указанием результатов").Bold();
                            AddEducationalTechnologiesPassportTable(col, passport.Parameter2.EducationalTechnologies);

                            // Подписи
                            col.Item().PaddingTop(20).Text($"{DateTime.Now:dd.MM.yyyy}");
                            col.Item().Text($"Работодатель {"_________________"} (Ф.И.О. работодателя)");
                            col.Item().Text($"Руководитель структурного подразделения {"_________________"} (Ф.И.О. руководителя структурного подразделения)");
                            col.Item().PaddingTop(10).Text("подтверждают достоверность представленной информации");
                            col.Item().PaddingTop(5).Text($"{passport.TeacherInfo.FullName} (Ф.И.О. педагогического работника)");
                            col.Item().Text($"аттестуемого(ой) с целью установления {passport.TotalScores.RecommendedCategory} по должности «{passport.TeacherInfo.Position}»");
                            col.Item().PaddingTop(10).Text($"{DateTime.Now:dd.MM.yyyy}  (подпись руководителя структурного подразделения)");
                            col.Item().Text($"{DateTime.Now:dd.MM.yyyy}  (подпись работодателя)");
                        });
                    });
                });
                return document.GeneratePdf();
            }

            // ========== ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ ДЛЯ ПАСПОРТА ==========

            private void AddAcademicPerformancePassportTable(ColumnDescriptor col, List<AcademicPerformanceTableDto> data)
            {
                if (!data.Any()) return;
                var years = data.Select(x => x.AcademicYear).Distinct().OrderBy(x => x).ToList();
                var disciplines = data.GroupBy(x => x.Discipline);
                col.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns => { columns.RelativeColumn(2); foreach (var _ in years) columns.RelativeColumn(1); });
                    table.Header(header => { header.Cell().Border(0.5f).Padding(3).Text("Предмет").Bold(); foreach (var year in years) header.Cell().Border(0.5f).Padding(3).Text(year).Bold(); });
                    foreach (var discipline in disciplines)
                    {
                        table.Cell().Border(0.5f).Padding(3).Text(discipline.Key);
                        foreach (var year in years)
                        {
                            var item = discipline.FirstOrDefault(x => x.AcademicYear == year);
                            if (item != null) table.Cell().Border(0.5f).Padding(3).Text($"{Convert.ToSingle(item.QualityPercent):F0}% / {Convert.ToSingle(item.SuccessPercent):F0}%");
                            else table.Cell().Border(0.5f).Padding(3).Text("—");
                        }
                    }
                });
            }

            private void AddGraduationResultPassportTable(ColumnDescriptor col, List<GraduationResultTableDto> data)
            {
                if (!data.Any()) return;
                var years = data.Select(x => x.AcademicYear).Distinct().OrderBy(x => x).ToList();
                col.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns => { columns.RelativeColumn(2); foreach (var _ in years) columns.RelativeColumn(1); });
                    table.Header(header => { header.Cell().Border(0.5f).Padding(3).Text("Предмет").Bold(); foreach (var year in years) header.Cell().Border(0.5f).Padding(3).Text(year).Bold(); });
                    table.Cell().Border(0.5f).Padding(3).Text("Выпускная квалификационная работа");
                    foreach (var _ in years) table.Cell().Border(0.5f).Padding(3).Text("");
                    table.Cell().Border(0.5f).Padding(3).Text("Успеваемость");
                    foreach (var year in years)
                    {
                        var yearData = data.Where(x => x.AcademicYear == year);
                        var success = yearData.Any() && yearData.All(x => !string.IsNullOrEmpty(x.Grade) && x.Grade != "неудовлетворительно") ? 100 : 0;
                        table.Cell().Border(0.5f).Padding(3).Text($"{success}%");
                    }
                    table.Cell().Border(0.5f).Padding(3).Text("Качество");
                    foreach (var year in years)
                    {
                        var yearData = data.Where(x => x.AcademicYear == year).ToList();
                        if (yearData.Any())
                        {
                            var goodCount = yearData.Count(x => x.Grade == "отлично" || x.Grade == "хорошо");
                            var quality = (decimal)goodCount / yearData.Count * 100;
                            table.Cell().Border(0.5f).Padding(3).Text($"{quality:F0}%");
                        }
                        else table.Cell().Border(0.5f).Padding(3).Text("—");
                    }
                });
            }

            private void AddStudentAchievementsPassportTable(ColumnDescriptor col, List<StudentAchievementTableDto> data)
            {
                if (!data.Any()) return;
                col.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns => { columns.RelativeColumn(1); columns.RelativeColumn(2); columns.RelativeColumn(2); columns.RelativeColumn(2); columns.RelativeColumn(1); columns.RelativeColumn(2); });
                    table.Header(header =>
                    {
                        header.Cell().Border(0.5f).Padding(3).Text("Учебный год").Bold();
                        header.Cell().Border(0.5f).Padding(3).Text("Ф.И. обучающегося, группа").Bold();
                        header.Cell().Border(0.5f).Padding(3).Text("Предмет").Bold();
                        header.Cell().Border(0.5f).Padding(3).Text("Наименование этапа/уровня чемпионата").Bold();
                        header.Cell().Border(0.5f).Padding(3).Text("Результат участия").Bold();
                        header.Cell().Border(0.5f).Padding(3).Text("Реквизиты приказа об итогах участия в чемпионатах").Bold();
                    });
                    foreach (var item in data)
                    {
                        table.Cell().Border(0.5f).Padding(3).Text(item.AcademicYear);
                        table.Cell().Border(0.5f).Padding(3).Text($"{item.StudentName}, {item.GroupName}");
                        table.Cell().Border(0.5f).Padding(3).Text(item.EventName);
                        table.Cell().Border(0.5f).Padding(3).Text(item.Level);
                        table.Cell().Border(0.5f).Padding(3).Text(item.Result);
                        table.Cell().Border(0.5f).Padding(3).Text(item.DocumentDetails ?? "—");
                    }
                });
            }

            private void AddMethodicalMaterialsPassportTable(ColumnDescriptor col, List<MethodicalMaterialTableDto> data)
            {
                if (!data.Any()) return;
                col.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns => { columns.RelativeColumn(1); columns.RelativeColumn(2); columns.RelativeColumn(2); columns.RelativeColumn(2); columns.RelativeColumn(2); });
                    table.Header(header =>
                    {
                        header.Cell().Border(0.5f).Padding(3).Text("Период работы").Bold();
                        header.Cell().Border(0.5f).Padding(3).Text("Вид программно-методического материала, созданного педагогом").Bold();
                        header.Cell().Border(0.5f).Padding(3).Text("Тема (наименование) программно-методического материала").Bold();
                        header.Cell().Border(0.5f).Padding(3).Text("Ссылка на размещение материалов в сети интернет").Bold();
                        header.Cell().Border(0.5f).Padding(3).Text("Реквизиты приказа об утверждении").Bold();
                    });
                    foreach (var item in data)
                    {
                        table.Cell().Border(0.5f).Padding(3).Text(item.AcademicYear);
                        table.Cell().Border(0.5f).Padding(3).Text(item.MaterialType);
                        table.Cell().Border(0.5f).Padding(3).Text(item.Topic);
                        table.Cell().Border(0.5f).Padding(3).Text(item.Link ?? "—");
                        table.Cell().Border(0.5f).Padding(3).Text(item.ApprovalDetails ?? "—");
                    }
                });
            }

            private void AddElectronicResourcesPassportTable(ColumnDescriptor col, List<ElectronicResourceTableDto> data)
            {
                if (!data.Any()) return;
                col.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns => { columns.RelativeColumn(2); columns.RelativeColumn(2); columns.RelativeColumn(3); columns.RelativeColumn(2); });
                    table.Header(header =>
                    {
                        header.Cell().Border(0.5f).Padding(3).Text("Тема урока/занятия/мероприятия").Bold();
                        header.Cell().Border(0.5f).Padding(3).Text("Наименование ЭОР").Bold();
                        header.Cell().Border(0.5f).Padding(3).Text("Формы взаимодействия с ЭОР").Bold();
                        header.Cell().Border(0.5f).Padding(3).Text("Ссылка на ЭОР").Bold();
                    });
                    foreach (var item in data)
                    {
                        table.Cell().Border(0.5f).Padding(3).Text(item.Topic);
                        table.Cell().Border(0.5f).Padding(3).Text(item.Name);
                        table.Cell().Border(0.5f).Padding(3).Text(item.InteractionForm);
                        table.Cell().Border(0.5f).Padding(3).Text(item.Link ?? "—");
                    }
                });
            }

            private void AddExperienceSharingPassportTable(ColumnDescriptor col, List<ExperienceSharingTableDto> data)
            {
                if (!data.Any()) return;
                col.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns => { columns.RelativeColumn(1); columns.RelativeColumn(2); columns.RelativeColumn(1); columns.RelativeColumn(1); columns.RelativeColumn(2); columns.RelativeColumn(2); });
                    table.Header(header =>
                    {
                        header.Cell().Border(0.5f).Padding(3).Text("Дата проведения").Bold();
                        header.Cell().Border(0.5f).Padding(3).Text("Организатор мероприятия").Bold();
                        header.Cell().Border(0.5f).Padding(3).Text("Уровень мероприятия").Bold();
                        header.Cell().Border(0.5f).Padding(3).Text("Формат мероприятия").Bold();
                        header.Cell().Border(0.5f).Padding(3).Text("Полное наименование мероприятия").Bold();
                        header.Cell().Border(0.5f).Padding(3).Text("Форма транслирования опыта / Тема").Bold();
                    });
                    foreach (var item in data)
                    {
                        table.Cell().Border(0.5f).Padding(3).Text(item.EventDate.ToString("dd.MM.yyyy"));
                        table.Cell().Border(0.5f).Padding(3).Text(item.Organizer ?? "—");
                        table.Cell().Border(0.5f).Padding(3).Text(item.Level);
                        table.Cell().Border(0.5f).Padding(3).Text(item.Format);
                        table.Cell().Border(0.5f).Padding(3).Text(item.EventName);
                        table.Cell().Border(0.5f).Padding(3).Text($"{item.SharingForm} / {item.Topic}");
                    }
                });
            }

            private void AddTeacherContestsPassportTable(ColumnDescriptor col, List<TeacherContestTableDto> data)
            {
                if (!data.Any()) return;
                col.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns => { columns.RelativeColumn(1); columns.RelativeColumn(2); columns.RelativeColumn(2); columns.RelativeColumn(1); columns.RelativeColumn(1); columns.RelativeColumn(1); columns.RelativeColumn(1); });
                    table.Header(header =>
                    {
                        header.Cell().Border(0.5f).Padding(3).Text("Учебный год").Bold();
                        header.Cell().Border(0.5f).Padding(3).Text("Конкурсное мероприятие").Bold();
                        header.Cell().Border(0.5f).Padding(3).Text("Организация").Bold();
                        header.Cell().Border(0.5f).Padding(3).Text("Уровень").Bold();
                        header.Cell().Border(0.5f).Padding(3).Text("Результат").Bold();
                        header.Cell().Border(0.5f).Padding(3).Text("Реквизиты приказа").Bold();
                        header.Cell().Border(0.5f).Padding(3).Text("Ссылка").Bold();
                    });
                    foreach (var item in data)
                    {
                        table.Cell().Border(0.5f).Padding(3).Text(item.AcademicYear);
                        table.Cell().Border(0.5f).Padding(3).Text(item.ContestName);
                        table.Cell().Border(0.5f).Padding(3).Text(item.Organizer);
                        table.Cell().Border(0.5f).Padding(3).Text(item.Level);
                        table.Cell().Border(0.5f).Padding(3).Text(item.Result);
                        table.Cell().Border(0.5f).Padding(3).Text(item.OrderDetails ?? "—");
                        table.Cell().Border(0.5f).Padding(3).Text(item.Link ?? "—");
                    }
                });
            }

            private void AddExpertActivitiesPassportTable(ColumnDescriptor col, List<ExpertActivityTableDto> data)
            {
                if (!data.Any()) return;
                col.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns => { columns.RelativeColumn(1); columns.RelativeColumn(2); columns.RelativeColumn(1); columns.RelativeColumn(2); });
                    table.Header(header =>
                    {
                        header.Cell().Border(0.5f).Padding(3).Text("Дата проведения").Bold();
                        header.Cell().Border(0.5f).Padding(3).Text("Наименование мероприятия").Bold();
                        header.Cell().Border(0.5f).Padding(3).Text("Уровень").Bold();
                        header.Cell().Border(0.5f).Padding(3).Text("Реквизиты документов").Bold();
                    });
                    foreach (var item in data)
                    {
                        table.Cell().Border(0.5f).Padding(3).Text(item.EventDate?.ToString("dd.MM.yyyy") ?? "—");
                        table.Cell().Border(0.5f).Padding(3).Text(item.EventName);
                        table.Cell().Border(0.5f).Padding(3).Text(item.Level);
                        table.Cell().Border(0.5f).Padding(3).Text(item.DocumentDetails ?? "—");
                    }
                });
            }

            private void AddEducationalTechnologiesPassportTable(ColumnDescriptor col, List<EducationalTechnologyTableDto> data)
            {
                if (!data.Any()) return;
                col.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns => { columns.RelativeColumn(2); columns.RelativeColumn(3); columns.RelativeColumn(3); columns.RelativeColumn(2); });
                    table.Header(header =>
                    {
                        header.Cell().Border(0.5f).Padding(3).Text("Наименование технологии").Bold();
                        header.Cell().Border(0.5f).Padding(3).Text("Цель использования").Bold();
                        header.Cell().Border(0.5f).Padding(3).Text("Результат использования").Bold();
                        header.Cell().Border(0.5f).Padding(3).Text("Ссылка на ресурс").Bold();
                    });
                    foreach (var item in data)
                    {
                        table.Cell().Border(0.5f).Padding(3).Text(item.TechnologyName);
                        table.Cell().Border(0.5f).Padding(3).Text(item.Purpose ?? "—");
                        table.Cell().Border(0.5f).Padding(3).Text(item.Result ?? "—");
                        table.Cell().Border(0.5f).Padding(3).Text(item.ResourceLink ?? "—");
                    }
                });
            }

        // Вспомогательные методы для построения таблиц
    }
}