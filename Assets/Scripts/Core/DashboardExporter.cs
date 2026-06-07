using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public static class DashboardExporter
{
    public const string ExportFileName = "florestia_dashboard_export.json";
    public const string CsvFileName = "florestia_dashboard_export.csv";
    const string ClassNameKey = "florestia.dashboard.className";
    const string PostUrlKey = "florestia.dashboard.postUrl";
    const string SchoolNameKey = "florestia.dashboard.schoolName";

    public static string ClassName
    {
        get => PlayerPrefs.GetString(ClassNameKey, "5º ano A");
        set { PlayerPrefs.SetString(ClassNameKey, value ?? ""); PlayerPrefs.Save(); }
    }

    public static string SchoolName
    {
        get => PlayerPrefs.GetString(SchoolNameKey, "");
        set { PlayerPrefs.SetString(SchoolNameKey, value ?? ""); PlayerPrefs.Save(); }
    }

    public static string PostUrl
    {
        get => PlayerPrefs.GetString(PostUrlKey, "");
        set { PlayerPrefs.SetString(PostUrlKey, value ?? ""); PlayerPrefs.Save(); }
    }

    public static string ExportFilePath =>
        Path.Combine(Application.persistentDataPath, ExportFileName);

    public static string CsvFilePath =>
        Path.Combine(Application.persistentDataPath, CsvFileName);

    public static string WriteToFile()
    {
        string json = BuildJson();
        File.WriteAllText(ExportFilePath, json, Encoding.UTF8);
        return ExportFilePath;
    }

    public static string WriteCsvToFile()
    {
        string csv = BuildCsv();
        File.WriteAllText(CsvFilePath, csv, Encoding.UTF8);
        return CsvFilePath;
    }

    public static string BuildCsv()
    {
        var snapshot = BuildSnapshot();
        var sb = new StringBuilder();

        sb.AppendLine("# Florestia · Resumo da turma");
        sb.AppendLine($"# Turma: {CsvEscape(snapshot.className)}");
        if (!string.IsNullOrWhiteSpace(snapshot.schoolName))
            sb.AppendLine($"# Escola: {CsvEscape(snapshot.schoolName)}");
        sb.AppendLine($"# Exportado em: {snapshot.exportedAt}");
        sb.AppendLine($"# Alunos: {snapshot.students.Count}  ·  Ciclo: {snapshot.totalDays} dias");
        sb.AppendLine();

        sb.AppendLine("Aluno,Dia,Dinheiro,Perguntas respondidas,Acertos,% de acerto,Tempo médio por pergunta (s),Categoria forte,Categoria fraca,Status,Tutorial concluído,Criado em");
        foreach (var s in snapshot.students)
        {
            sb.Append(CsvEscape(s.name)).Append(',');
            sb.Append(s.currentDay).Append(',');
            sb.Append(s.balance.ToString("F0", System.Globalization.CultureInfo.InvariantCulture)).Append(',');
            sb.Append(s.questionsAnswered).Append(',');
            sb.Append(s.correctCount).Append(',');
            sb.Append(s.correctPercent.ToString("F1", System.Globalization.CultureInfo.InvariantCulture)).Append(',');
            sb.Append(s.avgTimePerQuestion.ToString("F1", System.Globalization.CultureInfo.InvariantCulture)).Append(',');
            sb.Append(CsvEscape(s.strongCategory)).Append(',');
            sb.Append(CsvEscape(s.weakCategory)).Append(',');
            sb.Append(CsvEscape(s.status)).Append(',');
            sb.Append(s.tutorialCompleted ? "sim" : "não").Append(',');
            sb.AppendLine(CsvEscape(s.createdAt));
        }
        sb.AppendLine();

        sb.AppendLine("# Acerto por categoria · por aluno");
        sb.AppendLine("Aluno,Categoria,Respondidas,Acertos,% de acerto,Tempo médio (s)");
        foreach (var s in snapshot.students)
            foreach (var c in s.categoryStats)
            {
                sb.Append(CsvEscape(s.name)).Append(',');
                sb.Append(CsvEscape(c.category)).Append(',');
                sb.Append(c.total).Append(',');
                sb.Append(c.correct).Append(',');
                sb.Append(c.percent.ToString("F1", System.Globalization.CultureInfo.InvariantCulture)).Append(',');
                sb.AppendLine(c.avgTimeSeconds.ToString("F1", System.Globalization.CultureInfo.InvariantCulture));
            }
        sb.AppendLine();

        sb.AppendLine("# Acerto por dia · por aluno");
        sb.Append("Aluno");
        for (int d = 1; d <= snapshot.totalDays; d++) sb.Append(",Dia ").Append(d).Append(" (%)");
        sb.AppendLine();
        foreach (var s in snapshot.students)
        {
            sb.Append(CsvEscape(s.name));
            foreach (var day in s.dailyAccuracy)
            {
                sb.Append(',');
                if (day.total > 0)
                    sb.Append(day.percent.ToString("F0", System.Globalization.CultureInfo.InvariantCulture));
            }
            sb.AppendLine();
        }
        return sb.ToString();
    }

    static string CsvEscape(string value)
    {
        if (string.IsNullOrEmpty(value)) return "";
        bool needsQuotes = value.IndexOfAny(new[] { ',', '"', '\n', '\r' }) >= 0;
        string escaped = value.Replace("\"", "\"\"");
        return needsQuotes ? $"\"{escaped}\"" : escaped;
    }

    public static IEnumerator PostToDashboard(System.Action<bool, string> onDone = null)
    {
        string url = PostUrl;
        if (string.IsNullOrWhiteSpace(url))
        {
            onDone?.Invoke(false, "Nenhuma URL configurada.");
            yield break;
        }

        string json = BuildJson();
        var request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST);
        byte[] payload = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(payload);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();

        bool ok = request.result == UnityWebRequest.Result.Success;
        onDone?.Invoke(ok, ok ? request.downloadHandler.text : request.error);
    }

    public static string BuildJson()
    {
        var snapshot = BuildSnapshot();
        return JsonUtility.ToJson(snapshot, true);
    }

    public static DashboardSnapshot BuildSnapshot()
    {
        var snapshot = new DashboardSnapshot
        {
            schoolName = SchoolName,
            className = ClassName,
            exportedAt = System.DateTime.UtcNow.ToString("o"),
            totalDays = GameManager.TotalDays,
            students = new List<DashboardStudent>()
        };

        foreach (var profile in SaveSystem.ListProfiles())
        {
            var data = SaveSystem.LoadFor(profile);
            snapshot.students.Add(BuildStudent(profile, data));
        }
        return snapshot;
    }

    static DashboardStudent BuildStudent(StudentProfile profile, SaveData data)
    {
        var s = new DashboardStudent
        {
            name = profile?.name ?? "",
            createdAt = profile?.createdAt ?? "",
            currentDay = data?.day ?? 1,
            balance = data?.balance ?? 0f,
            tutorialCompleted = data?.tutorialCompleted ?? false,
            dailyBalance = data?.dailyBalanceHistory ?? new float[0],
            questions = new List<DashboardQuestion>(),
            categoryStats = new List<DashboardCategoryStat>(),
            dailyAccuracy = new List<DashboardDailyAccuracy>()
        };

        QuestionAnswerRecord[] qs = data?.questions ?? new QuestionAnswerRecord[0];
        int total = qs.Length;
        int correct = 0;
        float totalTime = 0f;

        var byCategory = new Dictionary<string, (int correct, int total, float time)>();
        var byDay = new Dictionary<int, (int correct, int total)>();

        foreach (var q in qs)
        {
            string cat = string.IsNullOrEmpty(q.category) ? "Outro" : q.category;
            if (q.correct) correct++;
            totalTime += q.timeTakenSeconds;

            byCategory.TryGetValue(cat, out var c);
            c.total++;
            if (q.correct) c.correct++;
            c.time += q.timeTakenSeconds;
            byCategory[cat] = c;

            byDay.TryGetValue(q.day, out var d);
            d.total++;
            if (q.correct) d.correct++;
            byDay[q.day] = d;

            s.questions.Add(new DashboardQuestion
            {
                day = q.day,
                category = cat,
                cropName = q.cropName,
                questionId = q.questionId,
                correct = q.correct,
                timeSeconds = q.timeTakenSeconds
            });
        }

        s.questionsAnswered = total;
        s.correctCount = correct;
        s.correctPercent = total > 0 ? (float)correct / total * 100f : 0f;
        s.avgTimePerQuestion = total > 0 ? totalTime / total : 0f;

        string strong = null, weak = null;
        float strongRate = -1f, weakRate = 2f;
        foreach (var kv in byCategory)
        {
            float rate = kv.Value.total > 0 ? (float)kv.Value.correct / kv.Value.total : 0f;
            s.categoryStats.Add(new DashboardCategoryStat
            {
                category = kv.Key,
                correct = kv.Value.correct,
                total = kv.Value.total,
                percent = rate * 100f,
                avgTimeSeconds = kv.Value.total > 0 ? kv.Value.time / kv.Value.total : 0f
            });
            if (kv.Value.total >= 2)
            {
                if (rate > strongRate) { strongRate = rate; strong = kv.Key; }
                if (rate < weakRate) { weakRate = rate; weak = kv.Key; }
            }
        }
        s.strongCategory = strong ?? "";
        s.weakCategory = weak ?? "";

        for (int d = 1; d <= GameManager.TotalDays; d++)
        {
            byDay.TryGetValue(d, out var v);
            s.dailyAccuracy.Add(new DashboardDailyAccuracy
            {
                day = d,
                correct = v.correct,
                total = v.total,
                percent = v.total > 0 ? (float)v.correct / v.total * 100f : 0f
            });
        }

        s.status = ClassifyStatus(s);
        return s;
    }

    static string ClassifyStatus(DashboardStudent s)
    {
        if (s.questionsAnswered == 0) return "sem dados";
        if (s.correctPercent < 60f) return "em risco";
        if (s.correctPercent < 80f) return "em progresso";
        return "no caminho";
    }
}

[System.Serializable]
public class DashboardSnapshot
{
    public string schoolName;
    public string className;
    public string exportedAt;
    public int totalDays;
    public List<DashboardStudent> students;
}

[System.Serializable]
public class DashboardStudent
{
    public string name;
    public string createdAt;
    public int currentDay;
    public float balance;
    public bool tutorialCompleted;
    public int questionsAnswered;
    public int correctCount;
    public float correctPercent;
    public float avgTimePerQuestion;
    public string strongCategory;
    public string weakCategory;
    public string status;
    public float[] dailyBalance;
    public List<DashboardCategoryStat> categoryStats;
    public List<DashboardDailyAccuracy> dailyAccuracy;
    public List<DashboardQuestion> questions;
}

[System.Serializable]
public class DashboardCategoryStat
{
    public string category;
    public int correct;
    public int total;
    public float percent;
    public float avgTimeSeconds;
}

[System.Serializable]
public class DashboardDailyAccuracy
{
    public int day;
    public int correct;
    public int total;
    public float percent;
}

[System.Serializable]
public class DashboardQuestion
{
    public int day;
    public string category;
    public string cropName;
    public string questionId;
    public bool correct;
    public float timeSeconds;
}
