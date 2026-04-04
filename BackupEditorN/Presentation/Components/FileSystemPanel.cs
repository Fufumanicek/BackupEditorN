namespace BackupEditorN.Presentation.Components;

public class FileSystemPanel : BaseComponent
{
    public event Action<string>? folderSelected;
    
    public override bool Selectable => true;
    
    private string _currentPath { get;  set; }
    private List<string> _entries { get; set; }
    private int _selectedIndex { get; set; }
    private int _visibleCount { get; set; }
    private string _title { get; set; }

    public FileSystemPanel(string title = "Průzkumník", int visibleCount = 12)
    {
        _title = title;
        _visibleCount = visibleCount;
        _entries = new List<string>();
        _selectedIndex = 0;
        _currentPath = string.Empty;
        LoadEntries();
    }
    
    private void LoadEntries()
    {
        _entries.Clear();
        _selectedIndex = 0;
        
        if(string.IsNullOrEmpty(_currentPath))
        {
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if(drive.IsReady)
                    _entries.Add(drive.Name);
            }
            
        } 
        else
                {
                    _entries.Add(".."); // návrat do nadřazené složky
                    try
                    {
                        foreach (string dir in Directory.GetDirectories(_currentPath))
                            _entries.Add(Path.GetFileName(dir) ?? dir);
                    }
                    catch { /* nemáme přístup k některým složkám */ } 
                }
    }
    
    private void NavigateTo(string path)
    {
        _currentPath = path;
        LoadEntries();
    }

    public override void Render(bool selected)
    {
         Console.WriteLine($"[ {_title} ]");
                Console.WriteLine(!string.IsNullOrEmpty(_currentPath) ? _currentPath : "Disky");
                Console.WriteLine(new string('-', 30));
        
                // Posuvné okno — zobrazíme jen _visibleCount položek
                int start = Math.Max(0, _selectedIndex - _visibleCount / 2);
                int end = Math.Min(_entries.Count, start + _visibleCount);
        
                for (int i = start; i < end; i++)
                {
                    bool isHighlighted = selected && i == _selectedIndex;
                    if (isHighlighted) Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine(_entries[i]);
                    Console.ResetColor();
                }
        
                // Doplníme prázdné řádky aby layout byl stabilní
                for (int i = end - start; i < _visibleCount; i++)
                    Console.WriteLine();
            
    }
    
    public override void HandleKey(ConsoleKeyInfo keyInfo)
    {
        if (keyInfo.Key == ConsoleKey.UpArrow && _selectedIndex > 0)
            _selectedIndex--;
        else if (keyInfo.Key == ConsoleKey.DownArrow && _selectedIndex < _entries.Count - 1)
                    _selectedIndex++; 
        else if (keyInfo.Key == ConsoleKey.Enter && _entries.Count > 0)
                {
                    string entry = _entries[_selectedIndex];
        
                    if (entry == "..")
                    {
                        // Jdeme o složku výš
                        string? parent = Directory.GetParent(_currentPath)?.FullName;
                        NavigateTo(parent ?? string.Empty);
                    }
                    else
                    {
                        // Vstoupíme do podsložky
                        string newPath = string.IsNullOrEmpty(_currentPath)
                            ? entry                            // disk (C:\)
                            : Path.Combine(_currentPath, entry);
                        NavigateTo(newPath);
                    }
                } 
        else if (keyInfo.Key == ConsoleKey.Spacebar && _entries.Count > 0)
                {
                    string entry = _entries[_selectedIndex];
                    if (entry == "..") return;
        
                    string fullPath = string.IsNullOrEmpty(_currentPath)
                        ? entry
                        : Path.Combine(_currentPath, entry);
        
                    folderSelected?.Invoke(fullPath); // oznámíme rodičovskému oknu
                }
    }
}