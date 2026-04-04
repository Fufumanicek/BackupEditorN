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
        _title = title;
        _visibleCount = visibleCount;
        Items = new List<string>();
        _selectedIndex = 0;
    }

    public void AddItem(string path)
    {
        if(!Items.Contains(path))
            Items.Add(path);
    }
    
    public void RemoveSelected()
    {
        if (Items.Count == 0) return;
        
        int index = Math.Min(_selectedIndex, Items.Count - 1);
        Items.RemoveAt(index);
        
        if (_selectedIndex >= Items.Count)
            _selectedIndex = Math.Max(0, _selectedIndex - 1);
    }
    
    public override void Render(bool selected)
    {
        Console.WriteLine($"[--{_title}--]");
        Console.WriteLine(new string('-', 30));

        int start = Math.Max(0, _selectedIndex - _visibleCount / 2);
        int end = Math.Min(Items.Count, start + _visibleCount);

        for (int i = start; i < end; i++)
        {
            bool isHighlighted = selected && i == _selectedIndex;
            if (isHighlighted == true) Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{i + 1}. {Items[i]}");
            Console.ResetColor();
        }
        
        for (int i = end - start; i < _visibleCount; i++)
            Console.WriteLine();
    }
    
    public override void HandleKey(ConsoleKeyInfo keyInfo)
    {
        if (keyInfo.Key == ConsoleKey.UpArrow && _selectedIndex > 0)
            _selectedIndex--;
        else if (keyInfo.Key == ConsoleKey.DownArrow && _selectedIndex < Items.Count - 1)
            _selectedIndex++;
        else if (keyInfo.Key == ConsoleKey.Delete)
            RemoveSelected();
    }
}