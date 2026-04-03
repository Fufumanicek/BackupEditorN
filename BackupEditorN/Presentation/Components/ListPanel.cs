namespace BackupEditorN.Presentation.Components;

public class ListPanel : BaseComponent
{
    public override bool Selectable => true;
    public List<string> Items { get; set; }
    public string? SelectedItem 
        => Items.Count > 0
        ? Items[Math.Min(_selectedIndex, Items.Count - 1)] : null;
    
    private int _selectedIndex { get; set; }
    private int _visibleCount { get; set; }
    private string _title { get; set; }

    public ListPanel(string title, int visibleCount = 10)
    {
        throw new NotImplementedException();
    }

    public void AddItem(string path)
    {
        
    }
    
    public void RemoveSelected()
    {
        
    }
    
    public override void Render(bool selected)
    {
        
    }
    
    public override void HandleKey(ConsoleKeyInfo keyInfo)
    {
        
    }
}