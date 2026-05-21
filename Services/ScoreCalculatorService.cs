using TeacherPortfolio.API.Models;

namespace TeacherPortfolio.API.Services;

public class ScoreCalculatorService
{
    // Расчет баллов за успеваемость (1.1)
    public int CalculateAcademicPerformanceScore(decimal qualityPercent)
    {
        if (qualityPercent >= 60) return 30;
        if (qualityPercent >= 40) return 20;
        if (qualityPercent >= 20) return 10;
        return 0;
    }

    // Расчет баллов за ГИА (1.2)
    public int CalculateGraduationResultScore(decimal qualityPercent, decimal successPercent)
    {
        if (successPercent != 100) return 0;

        if (qualityPercent >= 60) return 30;
        if (qualityPercent >= 40) return 20;
        if (qualityPercent >= 20) return 10;
        return 0;
    }

    // Расчет баллов за достижения студентов (1.3)
    public (int winnersScore, int participantsScore, int totalScore) CalculateStudentAchievementScores(List<Studentachievement> achievements)
    {
        int winnersScore = 0;
        int participantsScore = 0;

        var levelScores = new Dictionary<string, int>
        {
            { "Образовательная организация", 5 },
            { "Муниципальный", 10 },
            { "Региональный", 15 },
            { "Всероссийский", 20 },
            { "Международный", 25 }
        };

        var participantScores = new Dictionary<string, int>
        {
            { "Муниципальный", 5 },
            { "Региональный", 10 },
            { "Всероссийский", 15 },
            { "Международный", 20 }
        };

        foreach (var a in achievements)
        {
            var levelName = a.Level?.Name ?? "";
            var resultName = a.Result?.Name ?? "";

            // Победители/призеры
            if (resultName.Contains("победитель", StringComparison.OrdinalIgnoreCase) ||
                resultName.Contains("призер", StringComparison.OrdinalIgnoreCase) ||
                resultName.Contains("лауреат", StringComparison.OrdinalIgnoreCase))
            {
                if (levelScores.ContainsKey(levelName))
                    winnersScore += levelScores[levelName];
            }
            // Участники
            else if (resultName.Contains("участник", StringComparison.OrdinalIgnoreCase))
            {
                if (participantScores.ContainsKey(levelName))
                    participantsScore += participantScores[levelName];
            }
        }

        return (winnersScore, participantsScore, winnersScore + participantsScore);
    }

    // Расчет баллов за методические материалы (2.1.1)
    public int CalculateMethodicalMaterialScore(bool hasReview)
    {
        return hasReview ? 15 : 10;
    }

    // Расчет баллов за конкурсы методических разработок (2.1.2)
    public int CalculateMethodicalContestScore(string level, string result)
    {
        var isWinner = result.Contains("победитель", StringComparison.OrdinalIgnoreCase) ||
                       result.Contains("призер", StringComparison.OrdinalIgnoreCase) ||
                       result.Contains("лауреат", StringComparison.OrdinalIgnoreCase);

        var scores = new Dictionary<string, int>
        {
            { "Муниципальный", isWinner ? 20 : 5 },
            { "Региональный", isWinner ? 25 : 10 },
            { "Всероссийский", isWinner ? 30 : 15 }
        };

        return scores.ContainsKey(level) ? scores[level] : 0;
    }

    // Расчет баллов за электронные ресурсы (2.1.3)
    public int CalculateElectronicResourceScore(bool hasOwnResource)
    {
        return hasOwnResource ? 15 : 5;
    }

    // Расчет баллов за трансляцию опыта (2.3.1)
    public int CalculateExperienceSharingScore(string level)
    {
        var scores = new Dictionary<string, int>
        {
            { "Образовательная организация", 5 },
            { "Муниципальный", 15 },
            { "Региональный", 20 },
            { "Всероссийский", 30 }
        };

        return scores.ContainsKey(level) ? scores[level] : 0;
    }

    // Расчет баллов за конкурсы профмастерства (2.4)
    public int CalculateTeacherContestScore(string level, string result, bool isOfficial)
    {
        if (!isOfficial) return 0;

        var isWinner = result.Contains("победитель", StringComparison.OrdinalIgnoreCase) ||
                       result.Contains("призер", StringComparison.OrdinalIgnoreCase) ||
                       result.Contains("лауреат", StringComparison.OrdinalIgnoreCase);

        var scores = new Dictionary<string, int>
        {
            { "Образовательная организация", isWinner ? 20 : 0 },
            { "Муниципальный", isWinner ? 70 : 30 },
            { "Региональный", isWinner ? 120 : 80 },
            { "Всероссийский", isWinner ? 200 : 150 }
        };

        return scores.ContainsKey(level) ? scores[level] : 0;
    }

    // Расчет баллов за экспертную деятельность (2.5)
    public int CalculateExpertActivityScore(string level, string activityType, int? qualityPercent = null)
    {
        if (activityType.Contains("эксперт", StringComparison.OrdinalIgnoreCase) && qualityPercent.HasValue)
        {
            if (qualityPercent >= 95) return 100;
            if (qualityPercent >= 85) return 80;
            if (qualityPercent >= 70) return 60;
        }

        var scores = new Dictionary<string, int>
        {
            { "Муниципальный", 10 },
            { "Региональный", 20 },
            { "Федеральный", 110 }
        };

        return scores.ContainsKey(level) ? scores[level] : 0;
    }

    // Расчет баллов за образовательные технологии (2.6)
    public int CalculateEducationalTechnologyScore(string level)
    {
        var scores = new Dictionary<string, int>
        {
            { "Образовательная организация", 10 },
            { "Муниципальный", 15 },
            { "Региональный", 20 }
        };

        return scores.ContainsKey(level) ? scores[level] : 0;
    }
}