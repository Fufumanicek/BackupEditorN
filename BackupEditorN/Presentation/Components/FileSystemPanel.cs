namespace BackupEditorN.Presentation.Components;

public class FileSystemPanel : BaseComponent
{
    public event Action<string>? folderSelected;
    
    public override bool Selectable => true;
    
    private string _currentPath { get; set; }
    private List<string> _entries { get; set; }
    private int _selectedIndex { get; set; }
    private int _visibleCount { get; set; }
    private string _title { get; set; }

    public FileSystemPanel(string title = "Průzkumník", int visibleCount = 12)
    {
        
    }
    
    private void LoadEntries()
    {
        
    }
    
    private void NavigateTo(string path)
    {
        
    }

    public override void Render(bool selected)
    {
        
    }
    
    public override void HandleKey(ConsoleKeyInfo keyInfo)
    {
        
    }
}