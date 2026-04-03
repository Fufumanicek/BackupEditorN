using BackupEditorN.Helpers;

namespace BackupEditorN.Presentation.Components
{
    public class CheckBox : BaseComponent
    {
        public override bool Selectable => true;

        public bool Value { get; set; }

        private string _text;

        public CheckBox(string text, bool inline = false)
            : base(inline)
        {
            Value = false;
            _text = text;
        }

        public override void Render(bool selected)
        {
            string content = Value ? "[X]" : "[ ]";

            ConsoleHelper.WriteConditionalColor($"{_text}{content}", selected, ConsoleColor.Red);

            base.Render(selected);
        }

        public override void HandleKey(ConsoleKeyInfo keyInfo)
        {
            if (keyInfo.Key == ConsoleKey.Spacebar)
                Value = !Value;
        }
    }
}
