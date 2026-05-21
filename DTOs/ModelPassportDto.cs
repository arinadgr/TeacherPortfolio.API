namespace TeacherPortfolio.API.DTOs;

// Главный DTO модельного паспорта
public class ModelPassportDto
{
    // Информация о преподавателе
    public TeacherInfoDto TeacherInfo { get; set; } = new();

    // Параметр I. Результаты освоения образовательных программ
    public Parameter1Dto Parameter1 { get; set; } = new();

    // Параметр II. Личный вклад в повышение качества образования
    public Parameter2Dto Parameter2 { get; set; } = new();

    // Итоговые баллы
    public TotalScoresDto TotalScores { get; set; } = new();
}

// Информация о преподавателе
public class TeacherInfoDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public string Workplace { get; set; } = string.Empty;
    public string? QualificationCategory { get; set; }
    public string? OrderNumber { get; set; }
    public DateOnly? OrderDate { get; set; }
    public string Email { get; set; } = string.Empty;
}

// Параметр I
public class Parameter1Dto
{
    // 1.1 Успеваемость и качество знаний
    public List<AcademicPerformanceTableDto> AcademicPerformances { get; set; } = new();
    public AcademicPerformanceSummaryDto? AcademicPerformanceSummary { get; set; }

    // 1.2 Результаты ГИА
    public List<GraduationResultTableDto> GraduationResults { get; set; } = new();
    public GraduationResultSummaryDto? GraduationResultSummary { get; set; }

    // 1.3 Достижения обучающихся
    public List<StudentAchievementTableDto> StudentAchievements { get; set; } = new();
    public StudentAchievementScoresDto? StudentAchievementScores { get; set; }
}

// Таблица успеваемости
public class AcademicPerformanceTableDto
{
    public string AcademicYear { get; set; } = string.Empty;
    public string Discipline { get; set; } = string.Empty;
    public string GroupName { get; set; } = string.Empty;
    public decimal QualityPercent { get; set; }
    public decimal SuccessPercent { get; set; }
}

// Сводка по успеваемости
public class AcademicPerformanceSummaryDto
{
    public string Discipline { get; set; } = string.Empty;
    public decimal AverageQuality { get; set; }
    public decimal AverageSuccess { get; set; }
    public int Score { get; set; }
}

// Таблица ГИА
public class GraduationResultTableDto
{
    public string AcademicYear { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public string GroupName { get; set; } = string.Empty;
    public string ThesisTopic { get; set; } = string.Empty;
    public string Grade { get; set; } = string.Empty;
}

// Сводка по ГИА
public class GraduationResultSummaryDto
{
    public string AcademicYear { get; set; } = string.Empty;
    public int TotalCount { get; set; }
    public decimal QualityPercent { get; set; }
    public decimal SuccessPercent { get; set; }
    public int Score { get; set; }
}

// Таблица достижений студентов
public class StudentAchievementTableDto
{
    public string AcademicYear { get; set; } = string.Empty;
    public DateOnly EventDate { get; set; }
    public string EventName { get; set; } = string.Empty;
    public string Organizer { get; set; } = string.Empty;
    public string Direction { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public string GroupName { get; set; } = string.Empty;
    public string Result { get; set; } = string.Empty;
    public string? DocumentDetails { get; set; }
    public string? Link { get; set; }
}

// Баллы за достижения студентов
public class StudentAchievementScoresDto
{
    public int WinnersScore { get; set; }
    public int ParticipantsScore { get; set; }
    public int TotalScore { get; set; }
}

// Параметр II
public class Parameter2Dto
{
    // 2.1.1 Программно-методические материалы
    public List<MethodicalMaterialTableDto> MethodicalMaterials { get; set; } = new();
    public int MethodicalMaterialScore { get; set; }

    // 2.1.2 Конкурсы методических разработок
    public List<MethodicalContestTableDto> MethodicalContests { get; set; } = new();
    public int MethodicalContestScore { get; set; }

    // 2.1.3 Электронные ресурсы
    public List<ElectronicResourceTableDto> ElectronicResources { get; set; } = new();
    public int ElectronicResourceScore { get; set; }

    // 2.3.1 Трансляция опыта
    public List<ExperienceSharingTableDto> ExperienceSharings { get; set; } = new();
    public int ExperienceSharingScore { get; set; }

    // 2.4 Конкурсы профмастерства
    public List<TeacherContestTableDto> TeacherContests { get; set; } = new();
    public int TeacherContestScore { get; set; }

    // 2.5 Экспертная деятельность
    public List<ExpertActivityTableDto> ExpertActivities { get; set; } = new();
    public int ExpertActivityScore { get; set; }

    // 2.6 Образовательные технологии
    public List<EducationalTechnologyTableDto> EducationalTechnologies { get; set; } = new();
    public int EducationalTechnologyScore { get; set; }
}

// Таблица методических материалов
public class MethodicalMaterialTableDto
{
    public string AcademicYear { get; set; } = string.Empty;
    public string MaterialType { get; set; } = string.Empty;
    public string Topic { get; set; } = string.Empty;
    public string? Link { get; set; }
    public string? ApprovalDetails { get; set; }
    public string? ReviewOrganization { get; set; }
}

// Таблица конкурсов методических разработок
public class MethodicalContestTableDto
{
    public string AcademicYear { get; set; } = string.Empty;
    public string ContestName { get; set; } = string.Empty;
    public string Organizer { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public string Result { get; set; } = string.Empty;
    public string? Link { get; set; }
}

// Таблица электронных ресурсов
public class ElectronicResourceTableDto
{
    public string AcademicYear { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Topic { get; set; } = string.Empty;
    public string InteractionForm { get; set; } = string.Empty;
    public string? Link { get; set; }
}

// Таблица трансляции опыта
public class ExperienceSharingTableDto
{
    public DateOnly EventDate { get; set; }
    public string EventName { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public string Format { get; set; } = string.Empty;
    public string SharingForm { get; set; } = string.Empty;
    public string Topic { get; set; } = string.Empty;
    public string? Organizer { get; set; }
}

// Таблица конкурсов профмастерства
public class TeacherContestTableDto
{
    public string AcademicYear { get; set; } = string.Empty;
    public string ContestName { get; set; } = string.Empty;
    public string Organizer { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public string Result { get; set; } = string.Empty;
    public string? OrderDetails { get; set; }
    public string? Link { get; set; }
}

// Таблица экспертной деятельности
public class ExpertActivityTableDto
{
    public DateOnly? EventDate { get; set; }
    public string AcademicYear { get; set; } = string.Empty;
    public string EventName { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public string ActivityType { get; set; } = string.Empty;
    public string? DocumentDetails { get; set; }
}

// Таблица образовательных технологий
public class EducationalTechnologyTableDto
{
    public string TechnologyName { get; set; } = string.Empty;
    public string? Purpose { get; set; }
    public string? Result { get; set; }
    public string? ResourceLink { get; set; }
}

// Итоговые баллы
public class TotalScoresDto
{
    public int Parameter1Total { get; set; }
    public int Parameter2Total { get; set; }
    public int GrandTotal { get; set; }
    public string RecommendedCategory { get; set; } = string.Empty;
    public int RequiredForFirst { get; set; } = 115;
    public int RequiredForHigher { get; set; } = 230;
}