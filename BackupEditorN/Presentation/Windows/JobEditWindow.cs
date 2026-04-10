using System.Runtime.CompilerServices;
using BackupEditorN.Services;
using BackupEditorN.Presentation.Components;
using P3ABackupN.Enums;
using P3ABackupN.Models;

namespace BackupEditorN.Presentation.Windows;

// Wrapper třída pro zobrazení cest v tabulce
public class PathItem
{
    public string Path { get; set; }
    
    public PathItem(string path)
    {
        Path = path;
    }
}

public class JobEditWindow : BaseWindow
{
    private BackupJob _job { get; set; }
    private CronValidator _cronValidator { get; set; }

    // Editovatelná pole pro informace o jobu
    private TextBox _jobNameBox { get; set; }
    private TextBox _methodBox { get; set; }
    private TextBox _timingBox { get; set; }
    private Label _cronStatusLabel { get; set; }
    private Label _nextRunLabel { get; set; }
    private TextBox _retentionCountBox { get; set; }
    private TextBox _retentionSizeBox { get; set; }
    
    // Tabulky pro cesty
    private Label _sourcesHeaderLabel { get; set; }
    private Table<PathItem> _sourcesTable { get; set; }
    private Button _editSourceButton { get; set; }
    
    private Label _targetsHeaderLabel { get; set; }
    private Table<PathItem> _targetsTable { get; set; }
    private Button _editTargetButton { get; set; }
    
    private Button _saveButton { get; set; }
    private Button _cancelButton { get; set; }
    private Label _errorLabel { get; set; }

    public JobEditWindow(BackupJob job, Application app, IWindow returnWindow) : base("Edit Job", app, returnWindow)
    {
        _job = job;
        _cronValidator = new CronValidator();

        // Inicializace editovatelných polí pro informace o jobu
        _jobNameBox = new TextBox("Job Name: ");
        _methodBox = new TextBox("Method: ");
        _timingBox = new TextBox("Timing: ");
        _cronStatusLabel = new Label("");
        _nextRunLabel = new Label("");
        _retentionCountBox = new TextBox("Retention Count: ");
        _retentionSizeBox = new TextBox("Retention Size: ");
        
        // Inicializace tabulek pro cesty
        _sourcesHeaderLabel = new Label("Sources:");
        _sourcesTable = new Table<PathItem>(3);
        _editSourceButton = new Button("Edit Sources");
        
        _targetsHeaderLabel = new Label("Targets:");
        _targetsTable = new Table<PathItem>(3);
        _editTargetButton = new Button("Edit Targets");
        
        _saveButton = new Button("Save", inline: true);
        _cancelButton = new Button("Cancel", inline: true);
        _errorLabel = new Label("");

        // Registrace komponent - editovatelná pole pro informace
        RegisterComponent(_jobNameBox);
        RegisterComponent(_methodBox);
        RegisterComponent(_timingBox);
        RegisterComponent(_cronStatusLabel);
        RegisterComponent(_nextRunLabel);
        RegisterComponent(_retentionCountBox);
        RegisterComponent(_retentionSizeBox);
        
        // Registrace komponent - sources tabulka
        RegisterComponent(_sourcesHeaderLabel);
        RegisterComponent(_sourcesTable);
        RegisterComponent(_editSourceButton);
        
        // Registrace komponent - targets tabulka
        RegisterComponent(_targetsHeaderLabel);
        RegisterComponent(_targetsTable);
        RegisterComponent(_editTargetButton);
        
        // Registrace tlačítek
        RegisterComponent(_saveButton);
        RegisterComponent(_cancelButton);
        RegisterComponent(_errorLabel);

        _sourcesTable.ItemDeleted += DeleteSourceItem;
        _targetsTable.ItemDeleted += DeleteTargetItem;
        _editSourceButton.Clicked += EditSourcesClicked;
        _editTargetButton.Clicked += EditTargetsClicked;
        _saveButton.Clicked += SaveClicked;
        _cancelButton.Clicked += CancelClicked;

        SetComponentValues();
    }

    private void SetComponentValues()
    {
        _jobNameBox.Value = _job.JobName ?? "";
        _methodBox.Value = _job.Method.ToString();
        _timingBox.Value = _job.Timing ?? "";
        _retentionCountBox.Value = _job.Retention?.Count.ToString() ?? "0";
        _retentionSizeBox.Value = _job.Retention?.Size.ToString() ?? "1";
        
        UpdateSourcesTable();
        UpdateTargetsTable();
        ValidateCron();
    }
    
    private void UpdateSourcesTable()
    {
        _sourcesTable.Items = (_job.Sources ?? new List<string>())
            .Select(s => new PathItem(s))
            .ToList();
    }
    
    private void UpdateTargetsTable()
    {
        _targetsTable.Items = (_job.Targets ?? new List<string>())
            .Select(t => new PathItem(t))
            .ToList();
    }

    private void ValidateCron()
    {
        bool valid = _cronValidator.IsValid(_timingBox.Value);
        _cronStatusLabel._text = "CRON Status: " + (valid ? "Valid" : "Invalid");
        _nextRunLabel._text = "Next run: " + (_cronValidator.GetNextOccurence(_timingBox.Value) ?? "N/A");
    }

    private void SetEntityValues()
    {
        _job.JobName = _jobNameBox.Value;
    
        string methodValue = _methodBox.Value.Trim();
        string methodLower = methodValue.ToLowerInvariant();
        if (methodLower.Contains("full"))
            _job.Method = BackupMethod.FullBackup;
        else if (methodLower.Contains("differen"))
            _job.Method = BackupMethod.DifferentialBackup;
        else if (methodLower.Contains("increm"))
            _job.Method = BackupMethod.IncrementalBackup;
    
        _job.Timing = _timingBox.Value;
        
        if (_job.Retention == null)
            _job.Retention = new RetentionPolicy();
        
        if (int.TryParse(_retentionCountBox.Value, out var count))
            _job.Retention.Count = count;
        if (int.TryParse(_retentionSizeBox.Value, out var size))
            _job.Retention.Size = size;
    }

    private void DeleteSourceItem()
    {
        var selectedItem = _sourcesTable.SelectedItem;
        if (selectedItem != null && _job.Sources != null)
        {
            _job.Sources.Remove(selectedItem.Path);
            UpdateSourcesTable();
        }
    }

    private void DeleteTargetItem()
    {
        var selectedItem = _targetsTable.SelectedItem;
        if (selectedItem != null && _job.Targets != null)
        {
            _job.Targets.Remove(selectedItem.Path);
            UpdateTargetsTable();
        }
    }

    private void EditSourcesClicked()
    {
        var selectorWindow = new PathSelectorWindow(
            "Select sources:", _job.Sources ?? new List<string>(), _application, this);

        selectorWindow.Submitted += () =>
        {
            _job.Sources = selectorWindow.ResultPaths;
            UpdateSourcesTable();
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
            UpdateTargetsTable();
        };
        
        selectorWindow.Show();
    }

    private void SaveClicked()
    {
        _errorLabel._text = "";
        string message = "";
        if (string.IsNullOrWhiteSpace(_timingBox.Value) || !_cronValidator.IsValid(_timingBox.Value))
            message += "Invalid or missing CRON string. ";
        if (_sourcesTable.Items.Count == 0)
            message += "No source paths. ";
        if (_targetsTable.Items.Count == 0)
            message += "No target paths. ";
        string methodValue = _methodBox.Value.Trim();
        string methodLower = methodValue.ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(methodValue) || !IsValidMethod(methodLower))
            message += "Invalid method. ";
        if (!string.IsNullOrWhiteSpace(message))
        {
            _errorLabel._text = $"Validation failed:\n{message.Trim()}";
            return;
        }
        SetEntityValues();
        Submit();
    }

    private void CancelClicked()
    {
        Close();
    }

    public override void HandleKey(ConsoleKeyInfo keyInfo)
    {
        base.HandleKey(keyInfo);
        ValidateCron();
    }

    private bool IsValidMethod(string lowerMethod)
    {
        return lowerMethod switch
        {
            "full" or "fullbackup" or "full backup" => true,
            "differential" or "differentialbackup" or "differential backup" => true,
            "incremental" or "incrementalbackup" or "incremental backup" => true,
            _ => false
        };
    }
}