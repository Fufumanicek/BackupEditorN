using BackupEditorN.Services;
using BackupEditorN.Presentation.Components;
using P3ABackupN.Models;

namespace BackupEditorN.Presentation.Windows;

public class JobEditWindow : BaseWindow
{
    private BackupJob _job { get; set; }
    private CronValidator _cronValidator { get; set; }
    
    private TextBox _methodBox { get; set; }
    private TextBox _timingBox { get; set; }
    private Label _cronStatusLabel { get; set; }
    private Label _nextRunLabel { get; set; }
    private TextBox _retentionCountBox { get; set; }
    private TextBox _retentionSizeBox { get; set; }
    private Button _editSourceButton { get; set; }
    private Button _editTargetButton { get; set; }
    private Label _sourcesPreviewLabel { get; set; }
    private Label _targetsPreviewLabel { get; set; }
    private Button _saveButton { get; set; }
    private Button _cancelButton { get; set; }

    public JobEditWindow(BackupJob job, Application app, IWindow returnWindow) : base("Edit Job", app, returnWindow)
    {
        _job = job;
    }
    
    private void SetComponentValues()
    {
        
    }
    
    private void SetEntityValues()
    {
        
    }
    private void ValidateCron()
    {
        
    }
    private void EditSourcesClicked()
    {
        
    }
    private void EditTargetsClicked()
    {
        
    }
    private void SaveClicked()
    {
        
    }
    private void CancelClicked()
    {
        
    }
}