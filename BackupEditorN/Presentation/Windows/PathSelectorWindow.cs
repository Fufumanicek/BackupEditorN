using BackupEditorN.Presentation.Components;
namespace BackupEditorN.Presentation.Windows;

public class PathSelectorWindow
{
    //data
    private List<string> _selectedPaths { get; set; }
    
    //komponenty
    private FileSystemPanel _fileSystemPanel { get; set; }
    private ListPanel _labelPanel { get; set; }
    private Button _okButton { get; set; }
    private Button _cancelButton { get; set; }
    
    public List<string> ResultPaths { get; private set;}
    
    public PathSelectorWindow(string title, List<string> currentPaths, Application app, IWindow returnWindow)
    {
        
    }
    
    private void OnFolderSelected(string path)
    {
        
    }
    
    private void OkClicked()
    {
        
    }
    private void CancelClicked()
    {
        
    }
}