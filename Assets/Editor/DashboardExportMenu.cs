using UnityEditor;
using UnityEngine;

public static class DashboardExportMenu
{
    [MenuItem("Florestia/Exportar Dashboard (JSON)")]
    public static void ExportToFile()
    {
        string path = DashboardExporter.WriteToFile();
        Debug.Log($"Florestia Dashboard exportado para: {path}");
        EditorUtility.RevealInFinder(path);
    }

    [MenuItem("Florestia/Exportar Dashboard (CSV para professora)")]
    public static void ExportToCsv()
    {
        string path = DashboardExporter.WriteCsvToFile();
        Debug.Log($"Florestia Dashboard CSV exportado para: {path}");
        EditorUtility.RevealInFinder(path);
    }

    [MenuItem("Florestia/Configurar URL do Dashboard")]
    public static void ConfigurePostUrl()
    {
        string current = DashboardExporter.PostUrl;
        string newUrl = EditorInputDialog.Show(
            "URL do dashboard",
            "Cole a URL POST do dashboard (deixe em branco para desativar):",
            current);
        if (newUrl == null) return;
        DashboardExporter.PostUrl = newUrl.Trim();
        Debug.Log(string.IsNullOrEmpty(DashboardExporter.PostUrl)
            ? "URL do dashboard removida."
            : $"URL do dashboard salva: {DashboardExporter.PostUrl}");
    }

    [MenuItem("Florestia/Configurar Turma do Dashboard")]
    public static void ConfigureClassName()
    {
        string current = DashboardExporter.ClassName;
        string newName = EditorInputDialog.Show(
            "Turma",
            "Qual turma esses alunos pertencem? (ex.: 5º ano A)",
            current);
        if (newName == null) return;
        DashboardExporter.ClassName = newName.Trim();
        Debug.Log($"Turma salva: {DashboardExporter.ClassName}");
    }
}

public class EditorInputDialog : EditorWindow
{
    string _input;
    string _description;
    System.Action<string> _onConfirm;
    bool _focused;

    public static string Show(string title, string description, string defaultValue)
    {
        string result = null;
        bool done = false;
        var window = CreateInstance<EditorInputDialog>();
        window.titleContent = new GUIContent(title);
        window._input = defaultValue ?? "";
        window._description = description;
        window._onConfirm = v => { result = v; done = true; };
        window.minSize = new Vector2(420, 130);
        window.ShowModalUtility();
        return done ? result : null;
    }

    void OnGUI()
    {
        EditorGUILayout.Space(8);
        EditorGUILayout.LabelField(_description, EditorStyles.wordWrappedLabel);
        EditorGUILayout.Space(6);
        GUI.SetNextControlName("InputField");
        _input = EditorGUILayout.TextField(_input);
        if (!_focused)
        {
            EditorGUI.FocusTextInControl("InputField");
            _focused = true;
        }
        EditorGUILayout.Space(12);
        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Cancelar")) { Close(); }
            if (GUILayout.Button("Salvar"))
            {
                _onConfirm?.Invoke(_input);
                Close();
            }
        }
    }
}
