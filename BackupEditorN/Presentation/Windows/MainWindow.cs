using BackupEditorN.Presentation.Components;
using P3ABackupN.Enums;
using P3ABackupN.Models;
using BackupEditorN.Services;
using System.IO;

namespace BackupEditorN.Presentation.Windows;

// Wrapper třída pro zobrazení jobů v tabulce (bez sources a targets)
public class JobDisplayItem
{
    public string JobName { get; set; }
    public string Method { get; set; }
    
    public JobDisplayItem(string jobName, string method)
    {
        JobName = jobName;
        Method = method;
    }
}

public class MainWindow : BaseWindow
{
    private EditorConfigurationService EditorConfigurationService { get; set; }
    private List<BackupJob> _jobs { get; set; }
    
    //komponenty
    private Table<JobDisplayItem> _jobTable { get; set; }
    private Label _detailLabel { get; set; }
    private Button _newJobButton { get; set; }
    private Button _editJobButton { get; set; }
    private Button _deleteJobButton { get; set; }
    private Button _saveButton { get; set; }
    private TextBox _configPathBox { get; set; }
    
    public MainWindow(EditorConfigurationService editorConfigurationService, Application app) : base("Backup Editor", app)
    {
        EditorConfigurationService = editorConfigurationService;
        _jobs = new List<BackupJob>();
        
        _jobTable = new Table<JobDisplayItem>();
        _detailLabel = new Label("");
        _newJobButton = new Button("New [N]", inline:true, selectable: false);
        _editJobButton = new Button("Edit [Enter]", inline:true, selectable: false);
        _deleteJobButton = new Button("Delete [Del]", inline:true, selectable: false);
        _saveButton = new Button("Save [S]", inline:true, selectable: false);
        
        RegisterComponent(_jobTable);
        RegisterComponent(_detailLabel);
        _configPathBox = new TextBox("Config path: ", 80);
        _configPathBox.Value = "Config.json";
        RegisterComponent(_configPathBox);
        RegisterComponent(_newJobButton);
        RegisterComponent(_editJobButton);
        RegisterComponent(_deleteJobButton);
        RegisterComponent(_saveButton);
        LoadJobs();
    }
    
    private void UpdateJobTable()
    {
        _jobTable.Items = _jobs
            .Select(j => new JobDisplayItem(j.JobName ?? "Unnamed", j.Method.ToString()))
            .ToList();
    }

    private BackupJob? GetSelectedJob()
    {
        var selectedItem = _jobTable.SelectedItem;
        if (selectedItem == null || _jobTable.Items.Count == 0)
            return null;
        
        int selectedIndex = _jobTable.Items.IndexOf(selectedItem);
        if (selectedIndex >= 0 && selectedIndex < _jobs.Count)
            return _jobs[selectedIndex];
        
        return null;
    }

    private void LoadJobs()
    {
        UpdateJobTable();
        RefreshDetail();
    }

    private void RefreshDetail()
    {
        var job = GetSelectedJob();
        if (job == null)
        {
            _detailLabel._text = "No job selected";
            return;
        }

        _detailLabel._text =
            $"Job Name: {job.JobName ?? "Unnamed"}\n" +
            $"Method: {job.Method}\n" +
            $"Timing: {job.Timing}\n" +
            $"Retention: {job.Retention?.Count} count, {job.Retention?.Size} size\n" +
            $"Sources: {string.Join(", ", job.Sources ?? new List<string>())}\n" +
            $"Destination: {string.Join(", ", job.Targets ?? new List<string>())}";
    }
    
    private void NewButtonClicked()
    {
        var newJob = new BackupJob()
        {
            JobName = "New Backup",
            Sources = new List<string>(),
            Targets = new List<string>(),
            Method = BackupMethod.FullBackup,
            Timing = "0 0 * * *",
            Retention = new RetentionPolicy{Count = 5, Size = 1}
        };
        var editWindow = new JobEditWindow(newJob, _application, this);
        editWindow.Submitted += () =>
        {
            _jobs.Add(newJob);
            UpdateJobTable();
            RefreshDetail();
        };
        editWindow.Show();

    }
    
    private void DeleteButtonClicked()
    {
        var job = GetSelectedJob();
        if (job == null)
        {
            return;
        }
        var dialog = new DialogWindow(
            "Confirm delete", 
            "Are you sure you want to delete this configuration?",
            _application, this);
        
        dialog.Submitted += () =>
        {
            _jobs.Remove(job);
            UpdateJobTable();
            RefreshDetail();
        };
        dialog.Show();
    }
    
    private void EditButtonClicked()
    {
        var job = GetSelectedJob();
        if (job == null)
        {
            return;
        }
        var editWindow = new JobEditWindow(job, _application, this);
        editWindow.Submitted += () =>
        {
            UpdateJobTable();
            RefreshDetail();
        };
        editWindow.Show();
    }
    private void SaveButtonClicked()
    {
        if (_jobs.Count == 0)
        {
            _detailLabel._text = "No jobs found to Save";
            return;
        }
        
        string configPath = _configPathBox.Value ?? "config.json";
        string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", configPath);
        
        try
        {
            // Zajistit, že adresář existuje
            string? directory = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            
            // Vytvořit nový JSON soubor
            EditorConfigurationService.SaveToFile(fullPath, _jobs);
            _detailLabel._text = $"Save has completed successfully\nSaved to: {fullPath}";
        }
        catch (Exception e)
        {
            _detailLabel._text = $"Save error: {e.Message}";
        }
    }

    public override void HandleKey(ConsoleKeyInfo keyInfo)
    {
        if (_components[_selectedIndex] == _configPathBox)
        {
            base.HandleKey(keyInfo);
            return;
        }

        if (keyInfo.Key == ConsoleKey.Delete)
        {
            if (_jobs.Count == 0)
            {
                _detailLabel._text = "No jobs found to Delete";
                return;
            }
            DeleteButtonClicked();
            return;
        }
        if (keyInfo.Key == ConsoleKey.Enter)
        {
            if (_jobs.Count == 0)
            {
                _detailLabel._text = "No jobs found to Edit";
                return;
            }
            EditButtonClicked();
            return;
        } 
        if (keyInfo.Key == ConsoleKey.N)
        {
            NewButtonClicked();
            return;
        } 
        if (keyInfo.Key == ConsoleKey.S)
        {
            SaveButtonClicked();
            return;
        }
        
        base.HandleKey(keyInfo);
        
        if (_jobTable.Items.Count > 0)
        {
            RefreshDetail();
        }
    }

}