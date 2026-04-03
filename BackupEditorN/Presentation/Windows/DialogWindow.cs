using BackupEditorN.Presentation.Components;

namespace BackupEditorN.Presentation.Windows
{
    public class DialogWindow : BaseWindow
    {
        private Label _questionLabel;
        private Button _yesButton;
        private Button _noButton;

        public DialogWindow(string title, string question, Application application, IWindow? returnWindow = null) 
            : base(title, application, returnWindow)
        {
            _questionLabel = new Label(question);
            _yesButton = new Button("Yes", true);
            _noButton = new Button("No", true);

            RegisterComponent(_questionLabel);
            RegisterComponent(_yesButton);
            RegisterComponent(_noButton);

            _yesButton.Clicked += Submit;
            _noButton.Clicked += Close;
        }
    }
}
