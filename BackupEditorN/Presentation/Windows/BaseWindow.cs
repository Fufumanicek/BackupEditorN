using BackupEditorN.Presentation.Components;

namespace BackupEditorN.Presentation.Windows
{
    public abstract class BaseWindow : IWindow
    {
        public event Action? Closed;
        public event Action? Submitted;

        protected string _title;
        protected Application _application;
        protected IWindow? _returnWindow;

        private List<IComponent> _components;
        private int _selectedIndex;

        protected BaseWindow(string title, Application application, IWindow? returnWindow = null)
        {
            _title = title;
            _application = application;
            _returnWindow = returnWindow;
            _components = new List<IComponent>();
            _selectedIndex = 0;
        }

        public void Show()
        {
            _application.SwitchWindow(this);
        }

        public void Render()
        {
            Console.WriteLine($"{_title}\n");

            for (int i = 0; i < _components.Count; i++)
            {
                bool selected = i == _selectedIndex;
                _components[i].Render(selected);
            }
        }

        public virtual void HandleKey(ConsoleKeyInfo keyInfo)
        {
            if (keyInfo.Key == ConsoleKey.Escape)
            {
                Close();
            }
            else if (keyInfo.Key == ConsoleKey.Tab)
            {
                do _selectedIndex = (_selectedIndex + 1) % _components.Count;
                while (!_components[_selectedIndex].Selectable);
            }
            else
            {
                _components[_selectedIndex].HandleKey(keyInfo);
            }
        }

        protected void RegisterComponent(IComponent component)
        {
            _components.Add(component);

            // Find the first selectable component starting from index 0
            _selectedIndex = 0;
            while (_selectedIndex < _components.Count && !_components[_selectedIndex].Selectable)
            {
                _selectedIndex++;
            }
            
            // If no selectable component found, reset to 0 (will be caught by HandleKey)
            if (_selectedIndex >= _components.Count)
            {
                _selectedIndex = 0;
            }
        }

        protected void Close()
        {
            Closed?.Invoke();
            _returnWindow?.Show();
        }
        protected void Submit()
        {
            Submitted?.Invoke();
            Close();
        }
    }
}
