using System;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using ProjectWerner.Contracts.Extensions;
using ProjectWerner.Face2Speech.Functions;
using ProjectWerner.ServiceLocator;

namespace ProjectWerner.Face2Speech.Views
{
	[AppExtensionMetadataAttribute(Name = "Face2Speech")]
	[Export(typeof(IAppExtension))]
	public partial class Keyboard :  IAppExtension
    {
        public Keyboard()
        {
            //MicroKernel.Kernel.Bind<IDictionaryManager>().To<DictionaryManager>().InSingletonScope();
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

        public void OnApplicationClosed()
        {
            MicroKernel.Get<IDictionaryManager>().OnExit();
        }

        public UserControl AppUserControl { get { return this; } }
    }
}
