namespace BackupEditorN.Services;
using P3ABackupN.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

public class EditorConfigurationService
{
    private static JsonSerializerOptions GetOptions() => new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

    public List<BackupJob> LoadFromFile(string path)
    {
        string json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<List<BackupJob>>(json, GetOptions())
               ?? new List<BackupJob>();
    }

    public void SaveToFile(string path, List<BackupJob> jobs)
    {
        string json = JsonSerializer.Serialize(jobs, GetOptions());
        File.WriteAllText(path, json);
    }
    
}