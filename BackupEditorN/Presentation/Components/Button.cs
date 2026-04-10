
namespace BackupEditorN.Presentation.Components
{
    public class Button : BaseComponent
    {
        public event Action? Clicked;

        public override bool Selectable => _selectable;

        private string _text;
        private readonly bool _selectable;

        public Button(string text, bool inline = false, bool selectable = true)
            : base(inline)
        {
            _text = text;
            _selectable = selectable;
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
