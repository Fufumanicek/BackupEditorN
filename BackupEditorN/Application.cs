using BackupEditorN.Presentation.Windows;

namespace BackupEditorN
{
    public class Application
    {
        private bool _running;
        private IWindow? _activeWindow;

        public Application()
        {
            _running = false;
            _activeWindow = null;
        }

        public void Run(IWindow window)
        {
            _running = true;
            _activeWindow = window;

            Console.Title = "GradeRegister Admin [v1.0]";
            Console.CursorVisible = false;
            Console.Clear();

            while (_running)
            {
                Render();
                HandleKey(Console.ReadKey(true));
            }
        }

        public void Stop()
        {
            _running = false;
        }

        public void SwitchWindow(IWindow window)
        {
            Console.Clear();
            _activeWindow = window;
        }

        protected void Render()
        {
            Console.SetCursorPosition(0, 0);
            _activeWindow?.Render();
        }

        protected void HandleKey(ConsoleKeyInfo keyInfo)
        {
            _activeWindow?.HandleKey(keyInfo);
        }
    }
}
