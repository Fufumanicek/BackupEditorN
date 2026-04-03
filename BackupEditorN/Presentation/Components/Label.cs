namespace BackupEditorN.Presentation.Components
{
    public class Label : BaseComponent
    {
        public override bool Selectable => false;

        private string _text;

        public Label(string text, bool inline = false)
            : base(inline)
        {
            _text = text;
        }

        public override void Render(bool selected)
        {
            Console.Write(_text);

            base.Render(selected);
        }
    }
}
