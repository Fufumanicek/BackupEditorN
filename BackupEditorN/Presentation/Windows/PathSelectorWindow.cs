using BackupEditorN.Presentation.Components;
namespace BackupEditorN.Presentation.Windows;

public class PathSelectorWindow : BaseWindow
{
    //data
    private List<string> _selectedPaths { get; set; }
    
    //komponenty
    private FileSystemPanel _fileSystemPanel { get; set; }
    private ListPanel _listPanel { get; set; }
    private Button _okButton { get; set; }
    private Button _cancelButton { get; set; }

    public List<string> ResultPaths { get; private set; } = new();
    
    public PathSelectorWindow(string title, List<string> currentPaths, Application app, IWindow returnWindow):base(title, app, returnWindow)
    {
        _selectedPaths = new List<string>(currentPaths);

        _fileSystemPanel = new FileSystemPanel("Explorer");
        _listPanel = new ListPanel("Selected Paths");
        _okButton = new Button("OK", inline:true);
        _cancelButton = new Button("Cancel", inline:true);
        
        foreach (var path in _selectedPaths)
            _listPanel.AddItem(path);
        
        _fileSystemPanel.folderSelected += OnFolderSelected;
        _okButton.Clicked += OkClicked;
        _cancelButton.Clicked += CancelClicked;
        
        RegisterComponent(_fileSystemPanel);
        RegisterComponent(_listPanel);
        RegisterComponent(_okButton);
        RegisterComponent(_cancelButton);
    }
    
    private void OnFolderSelected(string path)
    {
        _listPanel.AddItem(path);
    }
    
    private void OkClicked()
    {
        ResultPaths = new List<string>(_listPanel.Items);
        Submit();
    }
    private void CancelClicked()
    {
        Close();
    }
}