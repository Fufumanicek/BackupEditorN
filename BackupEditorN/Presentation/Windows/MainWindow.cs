using BackupEditorN.Presentation.Components;
using P3ABackupN.Models;
using P3ABackupN.Services;

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
    }
    
    private void LoadJobs()
    {
        
    }

    private void RefreshDetail()
    {
        
    }
    
    private void NewButtonClicked()
    {
        
    }
    
    private void DeleteButtonClicked()
    {
        
    }
    
    private void EditButtonClicked()
    {
        
    }
    private void LoadButtonClicked()
    {
        
    }
    private void SaveButtonClicked()
    {
        
    }

    public override void HandleKey(ConsoleKeyInfo keyInfo)
    {
        
    }
    
}