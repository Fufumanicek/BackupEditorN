using System.Dynamic;
using BackupEditorN.Helpers;

namespace BackupEditorN.Presentation.Components
{
    public class Table<T> : BaseComponent
        where T : class
    {
        public event Action? ItemSelected;
        public event Action? ItemDeleted;

        public override bool Selectable => true;

        public T? SelectedItem => 
            Items != null && Items.Count > 0 && _selectedIndex >= 0 && _selectedIndex < Items.Count
            ? Items[_selectedIndex] 
            : null;

        public List<T> Items { get; set; }

        private int _count;
        private int _offset;
        private int _selectedIndex;
        private List<string> _headers;
        private List<int> _widths;

        public Table(int count = 10)
        {
            Items = new List<T>();
            _count = count;
            _offset = 0;
            _selectedIndex = 0;
            _headers = ExtractPropertyNames(typeof(T));
            _widths = _headers.Select(h => h.Length).ToList();
        }

        public override void Render(bool selected)
        {
            if (_selectedIndex > Items.Count - 1)
                _selectedIndex = Items.Count - 1;

            List<List<string>> rows = Items
                .Select(item => ExtractPropertyValues(typeof(T), item))
                .ToList();

            CalculateWidths(rows);

            RenderRow(null, '+', '-', selected, ConsoleColor.Red);
            RenderRow(_headers, '+', ' ', selected, ConsoleColor.Red);
            RenderRow(null, '+', '=', selected, ConsoleColor.Red);

            for (int i = _offset; i < _offset + _count; i++)
            {
                if (i < Items.Count)
                {
                    bool selectedRow = i == _selectedIndex;
                    RenderRow(rows[i], '|', ' ', selectedRow, ConsoleColor.Green);
                }
                else
                {
                    RenderRow(null, '|', ' ', false, ConsoleColor.White);
                }
            }

            RenderRow(null, '+', '-', selected, ConsoleColor.Red);
        }

        public override void HandleKey(ConsoleKeyInfo keyInfo)
        {
            if (_selectedIndex > Items.Count - 1)
                _selectedIndex = Items.Count - 1;

            if (keyInfo.Key == ConsoleKey.UpArrow && _selectedIndex > 0)
            {
                _selectedIndex--;

                if (_selectedIndex == _offset - 1)
                    _offset--;
            }
            else if (keyInfo.Key == ConsoleKey.DownArrow && _selectedIndex < Items.Count - 1)
            {
                _selectedIndex++;

                if (_selectedIndex == _offset + _count)
                    _offset++;
            }
            else if (keyInfo.Key == ConsoleKey.Enter)
            {
                ItemSelected?.Invoke();
            }
            else if (keyInfo.Key == ConsoleKey.Delete && Items.Count > 0 && _selectedIndex >= 0 && _selectedIndex < Items.Count)
            {
                ItemDeleted?.Invoke();
            }
        }

        private void RenderRow(List<string>? values, char sep, char pad, bool selected, ConsoleColor color)
        {
            for (int i = 0; i < _widths.Count; i++)
            {
                string value = values != null ? values[i] : string.Empty;
                string text = value.PadRight(_widths[i], pad);
                ConsoleHelper.WriteConditionalColor($"{sep}{pad}{text}{pad}", selected, color);
            }
            ConsoleHelper.WriteLineConditionalColor($"{sep}", selected, color);
        }

        private void CalculateWidths(List<List<string>> rows)
        {
            for (int i = 0; i < _widths.Count; i++)
            {
                foreach (List<string> row in rows)
                {
                    if (row[i].Length > _widths[i])
                        _widths[i] = row[i].Length;
                }
            }
        }

        private List<string> ExtractPropertyNames(Type type)
        {
            return type
                .GetProperties()
                .Select(p => p.Name)
                .ToList();
        }

        private List<string> ExtractPropertyValues(Type type, object? obj)
        {
            return type
                .GetProperties()
                .Select(p =>
                {
                    var val = p.GetValue(obj);
                    if (val is System.Collections.IEnumerable enumerable && val is not string)
                    {
                        var list = new List<string>();
                        foreach (var item in enumerable)
                        {
                            list.Add(item?.ToString() ?? "null");
                        }
                        return string.Join(", ", list);
                    }
                    return val?.ToString() ?? string.Empty;
                })
                .ToList();
        }
    }
}