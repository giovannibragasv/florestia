using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

[System.Serializable]
public class CropSlotSaveData
{
    public int slotIndex;
    public string cropType;   // "Mandioca" | "Cacau" | "Acai" | ""
    public int daysPlanted;
    public bool isWatered;
}

[System.Serializable]
public class InventorySaveData
{
    public int mandiocaCount;
    public int cacauCount;
    public int acaiCount;
}

[System.Serializable]
public class StudentProfile
{
    public string name;
    public string createdAt;
}

[System.Serializable]
class StudentProfileIndex
{
    public List<StudentProfile> profiles = new();
}

[System.Serializable]
public class PlantingRecord
{
    public int day;
    public string cropName;
    public int slotIndex;
}

[System.Serializable]
public class DailySaleRecord
{
    public int day;
    public string cropName;
    public int quantity;
    public float pricePerUnit;
    public float total;
    public string buyerName;
}

[System.Serializable]
public class QuestionAnswerRecord
{
    public int day;
    public string cropName;
    public string questionId;
    public string category;
    public bool correct;
    public float timeTakenSeconds;
}

[System.Serializable]
public class SaveData
{
    public string studentName;
    public int day;
    public float balance;
    public bool tutorialCompleted;
    public InventorySaveData inventory;
    public CropSlotSaveData[] cropSlots;
    public float[] dailyBalanceHistory;
    public PlantingRecord[] plantings;
    public DailySaleRecord[] sales;
    public QuestionAnswerRecord[] questions;
}

public static class SaveSystem
{
    const string LegacySaveFile = "florestia_save.json";
    const string IndexFile = "florestia_profiles.json";
    static string IndexPath => Path.Combine(Application.persistentDataPath, IndexFile);
    static string LegacySavePath => Path.Combine(Application.persistentDataPath, LegacySaveFile);

    public static StudentProfile ActiveProfile { get; private set; }

    public static List<StudentProfile> ListProfiles()
    {
        if (!File.Exists(IndexPath)) return new List<StudentProfile>();
        var idx = JsonUtility.FromJson<StudentProfileIndex>(File.ReadAllText(IndexPath));
        return idx?.profiles ?? new List<StudentProfile>();
    }

    public static StudentProfile FindOrCreateProfile(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return null;
        string trimmed = name.Trim();
        var list = ListProfiles();
        foreach (var p in list)
        {
            if (string.Equals(p.name, trimmed, System.StringComparison.OrdinalIgnoreCase))
                return p;
        }
        var profile = new StudentProfile
        {
            name = trimmed,
            createdAt = System.DateTime.UtcNow.ToString("o")
        };
        list.Add(profile);
        WriteIndex(list);
        return profile;
    }

    public static void TryMigrateLegacySave()
    {
        if (!File.Exists(LegacySavePath)) return;

        SaveData legacy = JsonUtility.FromJson<SaveData>(File.ReadAllText(LegacySavePath));
        if (legacy == null) return;

        string name = !string.IsNullOrWhiteSpace(legacy.studentName)
            ? legacy.studentName
            : "Aluno importado";
        StudentProfile profile = FindOrCreateImportProfile(name);
        if (profile == null) return;

        SetActiveProfile(profile);
        Save(legacy);
        File.Delete(LegacySavePath);
    }

    public static void DeleteProfile(StudentProfile p)
    {
        if (p == null) return;
        var list = ListProfiles();
        list.RemoveAll(x => string.Equals(x.name, p.name, System.StringComparison.OrdinalIgnoreCase));
        WriteIndex(list);
        string path = SaveFilePath(p);
        if (File.Exists(path)) File.Delete(path);
        if (ActiveProfile != null
            && string.Equals(ActiveProfile.name, p.name, System.StringComparison.OrdinalIgnoreCase))
            ActiveProfile = null;
    }

    public static void SetActiveProfile(StudentProfile p) => ActiveProfile = p;
    public static void ClearActiveProfile() => ActiveProfile = null;

    public static SaveData Load() => LoadFor(ActiveProfile);

    public static SaveData LoadFor(StudentProfile profile)
    {
        if (profile == null) return null;
        string path = SaveFilePath(profile);
        if (!File.Exists(path)) return null;
        return JsonUtility.FromJson<SaveData>(File.ReadAllText(path));
    }

    public static void Save(SaveData data)
    {
        if (ActiveProfile == null || data == null) return;
        data.studentName = ActiveProfile.name;
        File.WriteAllText(SaveFilePath(ActiveProfile), JsonUtility.ToJson(data, true));
    }

    public static void Delete()
    {
        if (ActiveProfile == null) return;
        string path = SaveFilePath(ActiveProfile);
        if (File.Exists(path)) File.Delete(path);
    }

    static void WriteIndex(List<StudentProfile> profiles)
    {
        var idx = new StudentProfileIndex { profiles = profiles };
        File.WriteAllText(IndexPath, JsonUtility.ToJson(idx, true));
    }

    static StudentProfile FindOrCreateImportProfile(string preferredName)
    {
        StudentProfile preferred = FindOrCreateProfile(preferredName);
        if (preferred == null || !File.Exists(SaveFilePath(preferred)))
            return preferred;

        for (int i = 2; i < 100; i++)
        {
            StudentProfile candidate = FindOrCreateProfile($"{preferredName} {i}");
            if (candidate != null && !File.Exists(SaveFilePath(candidate)))
                return candidate;
        }

        return null;
    }

    static string SaveFilePath(StudentProfile p) =>
        Path.Combine(Application.persistentDataPath, $"florestia_save_{SanitizeName(p.name)}.json");

    static string SanitizeName(string name)
    {
        if (string.IsNullOrEmpty(name)) return "anon";
        var sb = new StringBuilder();
        foreach (var c in name)
        {
            if (char.IsLetterOrDigit(c)) sb.Append(c);
            else if (c == ' ' || c == '_' || c == '-') sb.Append('_');
        }
        string safe = sb.ToString().ToLowerInvariant();
        return string.IsNullOrEmpty(safe) ? "anon" : safe;
    }
}
