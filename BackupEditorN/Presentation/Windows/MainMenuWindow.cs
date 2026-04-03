using BackupEditorN.Presentation.Components;
using BackupEditorN.Services;

namespace BackupEditorN.Presentation.Windows
{
    public class MainMenuWindow : BaseWindow
    {
        private ConfigurationService _configurationService;

        private Button _teachersButton;
        private Button _coursesButton;
        private Button _exitButton;

        public MainMenuWindow(ConfigurationService configurationService, Application application) 
            : base("Main Menu", application)
        {
            _configurationService = configurationService;

            _teachersButton = new Button("Teachers");
            _coursesButton = new Button("Courses");
            _exitButton = new Button("Exit");

            RegisterComponent(_teachersButton);
            RegisterComponent(_coursesButton);
            RegisterComponent(_exitButton);

            _teachersButton.Clicked += TeachersButtonClicked;
            _exitButton.Clicked += ExitButtonClicked;
            Closed += WindowClosed;
        }

        private void TeachersButtonClicked()
        {
            IWindow window = new TeachersListWindow(_configurationService, _application, this);
            window.Show();
        }

        private void ExitButtonClicked()
        {
            Close();
        }

        private void WindowClosed()
        {
            _application.Stop();
        }
    }
}
