using System.Text;
using BackupEditorN.Presentation.Components;

namespace BackupEditorN.Presentation.Windows;

public class PathSelectorWindow : BaseWindow
{
    private List<string> _selectedPaths;
    private Table<FileEntry> _fileTable;
    private ListPanel _listPanel;
    private Button _okButton;
    private Button _cancelButton;

    private string _currentDir;
    private bool _creatingNewFolder;
    private StringBuilder _newFolderNameBuilder;

    public List<string> ResultPaths { get; private set; } = new();

    private class FileEntry
    {
        public string Name { get; set; } = "";
        public bool IsDirectory { get; set; }
    }

    private class FilePathTable : Table<FileEntry>
    {
        private readonly PathSelectorWindow _window;

        public FilePathTable(PathSelectorWindow window) : base(12)
        {
            _window = window;
        }

        public override void HandleKey(ConsoleKeyInfo keyInfo)
        {
            if (_window._creatingNewFolder)
            {
                var key = keyInfo.Key;
                var ch = keyInfo.KeyChar;
                if (key == ConsoleKey.Enter)
                {
                    string name = _window._newFolderNameBuilder.ToString().Trim();
                    if (!string.IsNullOrEmpty(name))
                    {
                        string newPath = Path.Combine(_window._currentDir, name);
                        try
                        {
                            Directory.CreateDirectory(newPath);
                            _window.Refresh();
                        }
                        catch
                        {
                            Console.Beep();
                        }
                    }
                    _window._creatingNewFolder = false;
                    _window._newFolderNameBuilder.Clear();
                    Console.CursorVisible = false;
                    return;
                }
                else if (key == ConsoleKey.Escape)
                {
                    _window._creatingNewFolder = false;
                    _window._newFolderNameBuilder.Clear();
                    Console.CursorVisible = false;
                    return;
                }
                else if (key == ConsoleKey.Backspace && _window._newFolderNameBuilder.Length > 0)
                {
                    _window._newFolderNameBuilder.Length--;
                    return;
                }
                else if (!char.IsControl(ch) && (char.IsLetterOrDigit(ch) || ch == ' ' || ch == '_' || ch == '-' || ch == '.'))
                {
                    _window._newFolderNameBuilder.Append(ch);
                    return;
                }
                return;
            }

            switch (keyInfo.Key)
            {
                case ConsoleKey.RightArrow:
                    var sel = SelectedItem as FileEntry;
                    if (sel != null && sel.IsDirectory && sel.Name != "New folder")
                    {
                        if (sel.Name == "..")
                        {
                            var parent = Directory.GetParent(_window._currentDir);
                            if (parent != null)
                            {
                                _window._currentDir = parent.FullName;
                            }
                            else
                            {
                                _window._currentDir = ""; // Disky
                            }
                        }
                        else
                        {
                            _window._currentDir = string.IsNullOrEmpty(_window._currentDir) 
                                ? sel.Name 
                                : Path.Combine(_window._currentDir, sel.Name);
                        }
                        _window.Refresh();
                    }
                    break;
                case ConsoleKey.Enter:
                    var selEntry = SelectedItem as FileEntry;
                    if (selEntry == null) return;
                    if (selEntry.Name == "New folder")
                    {
                        if (string.IsNullOrEmpty(_window._currentDir))
                        {
                            Console.Beep();
                            return;
                        }
                        _window._creatingNewFolder = true;
                        _window._newFolderNameBuilder.Clear();
                        _window._newFolderNameBuilder.Append("New folder");
                        Console.CursorVisible = true;
                        return;
                    }
                    
                    string fullPath = string.IsNullOrEmpty(_window._currentDir)
                        ? selEntry.Name
                        : Path.Combine(_window._currentDir, selEntry.Name);
                    
                    _window.OnFolderSelected(fullPath);
                    break;
                default:
                    base.HandleKey(keyInfo);
                    break;
            }
        }
    }

    public PathSelectorWindow(string title, List<string> currentPaths, Application app, IWindow returnWindow)
        : base(title, app, returnWindow)
    {
        _selectedPaths = new List<string>(currentPaths);
        _currentDir = Directory.GetCurrentDirectory();
        _creatingNewFolder = false;
        _newFolderNameBuilder = new StringBuilder();

        _fileTable = new FilePathTable(this);
        _listPanel = new ListPanel("Selected Paths");
        _okButton = new Button("OK", inline: true);
        _cancelButton = new Button("Cancel", inline: true);

        foreach (var path in _selectedPaths)
            _listPanel.AddItem(path);

        _okButton.Clicked += OkClicked;
        _cancelButton.Clicked += CancelClicked;

        RegisterComponent(_fileTable);
        RegisterComponent(_listPanel);
        RegisterComponent(_okButton);
        RegisterComponent(_cancelButton);
        
        Refresh();
    }

    public void Refresh()
    {
        try
        {
            var entries = new List<FileEntry>();
            if (string.IsNullOrEmpty(_currentDir))
            {
                foreach (DriveInfo drive in DriveInfo.GetDrives())
                {
                    if (drive.IsReady)
                        entries.Add(new FileEntry { Name = drive.Name, IsDirectory = true });
                }
            }
            else
            {
                entries.Add(new FileEntry { Name = "New folder", IsDirectory = true });
                entries.Add(new FileEntry { Name = "..", IsDirectory = true });
                entries.AddRange(Directory.GetDirectories(_currentDir)
                    .Select(d => new FileEntry { Name = Path.GetFileName(d), IsDirectory = true })
                    .OrderBy(e => e.Name));
                entries.AddRange(Directory.GetFiles(_currentDir)
                    .Select(f => new FileEntry { Name = Path.GetFileName(f) ?? f, IsDirectory = false })
                    .OrderBy(e => e.Name));
            }
            _fileTable.Items = entries;
        }
        catch
        {
            // Ignore errors
        }
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

    public override void Render()
    {
        Console.WriteLine($"{_title}\n");
        Console.WriteLine($"Current path: {(!string.IsNullOrEmpty(_currentDir) ? _currentDir : "Drives")}");

        if (_creatingNewFolder)
        {
            string inputPrompt = "New folder name: ";
            string inputValue = _newFolderNameBuilder.ToString();
            Console.WriteLine(inputPrompt + inputValue + "_");

            Console.WriteLine("Chars/-._ : type, Backspace: del, Enter: create, Esc: cancel");
        }
        else
        {
            Console.CursorVisible = false;
            // Vypíšeme komponenty bez opětovného vypisování titulku
            for (int i = 0; i < _components.Count; i++)
            {
                bool selected = i == _selectedIndex;
                _components[i].Render(selected);
            }
        }
    }
}