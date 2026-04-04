using BackupEditorN.Presentation.Components;
using P3ABackupN.Enums;
using P3ABackupN.Models;
using BackupEditorN.Services;

namespace BackupEditorN.Presentation.Windows;

public class MainWindow : BaseWindow
{
    private ConfigurationService _configurationService { get; set; }
    private List<BackupJob> _jobs { get; set; }
    
    //komponenty
    private Table<BackupJob> _jobTable { get; set; }
    private Label _detailLabel { get; set; }
    private Button _newJobButton { get; set; }
    private Button _editJobButton { get; set; }
    private Button _deleteJobButton { get; set; }
    private Button _saveButton { get; set; }
    private Button _loadButton { get; set; }
    
    public MainWindow(ConfigurationService configurationService, Application app) : base("Backup Editor", app)
    {
        _configurationService = configurationService;
        _jobs = new List<BackupJob>();
        
        _jobTable = new Table<BackupJob>();
        _detailLabel = new Label("");
        _newJobButton = new Button("New [N]", inline:true);
        _editJobButton = new Button("Edit [Enter]", inline:true);
        _deleteJobButton = new Button("Delete [Del]", inline:true);
        _saveButton = new Button("Save [S]", inline:true);
        _loadButton = new Button("Load [L]", inline:true);
        
        RegisterComponent(_jobTable);
        RegisterComponent(_detailLabel);
        RegisterComponent(_newJobButton);
        RegisterComponent(_editJobButton);
        RegisterComponent(_deleteJobButton);
    }
    
    private void LoadJobs()
    {
        _jobTable.Items = _jobs;
        RefreshDetail();
    }

    private void RefreshDetail()
    {
        var job = _jobTable.SelectedItem;
        if (job == null)
        {
            _detailLabel._text = "No job selected";
            return;
        }

        _detailLabel._text =
            $"Method? {job.Method}\n" +
            $"Timing: {job.Timing}\n" +
            $"Retention: {job.Retention?.Count} count, {job.Retention?.Size} size\n" +
            $"Sources: {string.Join(", ", job.Sources ?? new())}\n" +
            $"Destination: {string.Join(", ", job.Targets ?? new())}";
    }
    
    private void NewButtonClicked()
    {
        var newJob = new BackupJob()
        {
            Sources = new List<string>(),
            Targets = new List<string>(),
            Method = BackupMethod.FullBackup,
            Timing = "0 0 * * *",
            Retention = new RetentionPolicy{Count = 5, Size = 1}
        };
        _jobs.Add(newJob);
        _jobTable.Items = _jobs;

        var editWindow = new JobEditWindow(newJob, _application, this);
        editWindow.Show();

    }
    
    private void DeleteButtonClicked()
    {
        var job = _jobTable.SelectedItem;
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
            _jobTable.Items = _jobs;
            RefreshDetail();
        };
        dialog.Show();
    }
    
    private void EditButtonClicked()
    {
        var job = _jobTable.SelectedItem;
        if (job == null)
        {
            return;
        }
        var editWindow = new JobEditWindow(job, _application, this);
        editWindow.Submitted += RefreshDetail;
        editWindow.Show();
    }
    private void LoadButtonClicked()
    {
        string path = "config.json";
        try
        {
            _jobs = _configurationService.LoadFromFile(path);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }
    private void SaveButtonClicked()
    {
        string path = "config.json";
        try
        {
            _configurationService.SaveToFile(path, _jobs);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }

    public override void HandleKey(ConsoleKeyInfo keyInfo)
    {
        if (keyInfo.Key == ConsoleKey.Delete)
        {
            DeleteButtonClicked();
            return;
        }
        if (keyInfo.Key == ConsoleKey.Enter)
        {
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
        if (keyInfo.Key == ConsoleKey.L)
        {
            LoadButtonClicked();
            return;
        }
        
        base.HandleKey(keyInfo);
        RefreshDetail();
    }
    
    

}