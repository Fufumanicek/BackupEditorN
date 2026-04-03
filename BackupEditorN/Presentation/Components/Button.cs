
namespace BackupEditorN.Presentation.Components
{
    public class Button : BaseComponent
    {
        public event Action? Clicked;

        public override bool Selectable => true;

        private string _text;

        public Button(string text, bool inline = false)
            : base(inline)
        {
            _text = text;
        }

        public override void Render(bool selected)
        {
            if (selected)
                Console.ForegroundColor = ConsoleColor.Red;

            Console.Write($"[ {_text} ]");
            Console.ResetColor();

            base.Render(selected);
        }

        public override void HandleKey(ConsoleKeyInfo keyInfo)
        {
            if (keyInfo.Key == ConsoleKey.Enter)
                Clicked?.Invoke();
        }
    }
}
