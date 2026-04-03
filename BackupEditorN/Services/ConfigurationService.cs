namespace BackupEditorN.Services;
using P3ABackupN.Models;

public class ConfigurationService
{
    private string _filePath { get; set; }
    private List<BackupJob> _jobs { get; set; }

    public List<BackupJob> LoadFromFile(string path)
    {
        return new List<BackupJob>();
    }
    public void SaveToFile(string path, List<BackupJob> jobs)
    {
        
    }
    
    public string? CurrentFilePath { get; }
}