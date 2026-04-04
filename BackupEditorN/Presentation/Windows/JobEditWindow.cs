using System.Runtime.CompilerServices;
using BackupEditorN.Services;
using BackupEditorN.Presentation.Components;
using P3ABackupN.Enums;
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
        _cronValidator = new CronValidator();

        _methodBox = new TextBox("Method: ");
        _timingBox = new TextBox("Timing: ");
        _cronStatusLabel = new Label("CRON: ");
        _nextRunLabel = new Label("");
        _retentionCountBox = new TextBox("Retention Count: ");
        _retentionSizeBox = new TextBox("Retention Size: ");
        _editSourceButton = new Button("Edit Sources");
        _editTargetButton = new Button("Edit Targets");
        _sourcesPreviewLabel = new Label("");
        _targetsPreviewLabel = new Label("");
        _saveButton = new Button("Save", inline: true);
        _cancelButton = new Button("Cancel", inline: true);

        RegisterComponent(_methodBox);
        RegisterComponent(_timingBox);
        RegisterComponent(_cronStatusLabel);
        RegisterComponent(_nextRunLabel);
        RegisterComponent(_retentionCountBox);
        RegisterComponent(_retentionSizeBox);
        RegisterComponent(_editSourceButton);
        RegisterComponent(_sourcesPreviewLabel);
        RegisterComponent(_editTargetButton);
        RegisterComponent(_targetsPreviewLabel);
        RegisterComponent(_saveButton);
        RegisterComponent(_cancelButton);

        _timingBox.HandleKey(new ConsoleKeyInfo());
        _editSourceButton.Clicked += EditSourcesClicked;
        _editTargetButton.Clicked += EditTargetsClicked;
        _saveButton.Clicked += SaveClicked;
        _cancelButton.Clicked += CancelClicked;

        SetComponentValues();
    }

    private void SetComponentValues()
    {
        _methodBox.Value = _job.Method.ToString().ToLower();
        _timingBox.Value = _job.Timing ?? "";
        _retentionCountBox.Value = _job.Retention.Count.ToString() ?? "0";
        _retentionSizeBox.Value = _job.Retention.Size.ToString() ?? "1";
        _sourcesPreviewLabel._text = "Sources: " + string.Join(", ", _job.Sources ?? new());
        _targetsPreviewLabel._text = "Targets: " + string.Join(", ", _job.Targets ?? new());

        ValidateCron();
    }

    private void SetEntityValues()
    {
        if (Enum.TryParse<BackupMethod>(_methodBox.Value, true, out var method))
        {
            _job.Method = method;
        }
        _job.Timing = _timingBox.Value; 
        
        if(_job.Retention == null)
            _job.Retention = new RetentionPolicy();
        
        if(int.TryParse(_retentionCountBox.Value, out var count))
            _job.Retention.Count = count;
        if(int.TryParse(_retentionSizeBox.Value, out var size))
            _job.Retention.Size = size;
        
    }

    private void ValidateCron()
    {
        bool valid = _cronValidator.IsValid(_timingBox.Value);
        _cronStatusLabel._text = valid ? "Valid" : "Invalid";
        _nextRunLabel._text = "Next run:" + (_cronValidator.GetNextOccurence(_timingBox.Value) ?? "N/A");
    }

    private void EditSourcesClicked()
    {
        var selectorWindow = new PathSelectorWindow(
            "Select sources:", _job.Sources ?? new List<string>(), _application, this);

        selectorWindow.Submitted += () =>
        {
            _job.Sources = selectorWindow.ResultPaths;
            _sourcesPreviewLabel._text = "Sources: " + string.Join(", ", _job.Sources);
        };
        
        selectorWindow.Show();
    }

    private void EditTargetsClicked()
    {
        var selectorWindow = new PathSelectorWindow(
            "Select targets:", _job.Targets ?? new List<string>(), _application, this);

        selectorWindow.Submitted += () =>
        {
            _job.Targets = selectorWindow.ResultPaths;
            _targetsPreviewLabel._text = "Targets: " + string.Join(", ", _job.Targets);
        };
        
        selectorWindow.Show();
    }

    private void SaveClicked()
    {
        SetEntityValues();
        Submit();
    }

    private void CancelClicked()
    {
        Close();
    }

    // Override HandleKey aby ValidateCron reagoval na změny v TextBoxu
    public override void HandleKey(ConsoleKeyInfo keyInfo)
    {
        base.HandleKey(keyInfo);
        ValidateCron(); //průběžně validujeme při každém stisku
    }
}