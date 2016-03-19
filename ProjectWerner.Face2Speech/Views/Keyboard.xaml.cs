using System.ComponentModel.Composition;
using System.Windows.Controls;
using ProjectWerner.Contracts.Extensions;

namespace ProjectWerner.Face2Speech.Views
{
	[AppExtensionMetadataAttribute(Name = "Face2Speech")]
	[Export(typeof(IAppExtension))]
	public partial class Keyboard : IAppExtension
    {
        public Keyboard()
        {
            InitializeComponent();
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            textBox.CaretIndex = textBox.Text.Length;
            var caretLocation = textBox.GetRectFromCharacterIndex(textBox.CaretIndex).Location;

            if (!double.IsInfinity(caretLocation.X))
            {
                Canvas.SetLeft(Caret, caretLocation.X);
            }

            if (!double.IsInfinity(caretLocation.Y))
            {
                Canvas.SetTop(Caret, caretLocation.Y);
            }
        }

        public UserControl AppUserControl { get { return this; } }
    }
}
