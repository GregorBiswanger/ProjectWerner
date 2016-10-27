using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;

#pragma warning disable 0067

namespace ProjectWerner.Dto
{
    internal class ExtensionDataSetSampledata
    {
        public static ObservableCollection<ExtensionDataSet> LoadSampledata()
        {
            var extensions = new ObservableCollection<ExtensionDataSet>
            {
                new ExtensionDataSet(null, "Ex1", GetDefaultIcon(), Guid.Empty),
                new ExtensionDataSet(null, "Ex2", GetDefaultIcon(), Guid.Empty),
                new ExtensionDataSet(null, "Ex3", GetDefaultIcon(), Guid.Empty),
                new ExtensionDataSet(null, "Ex4", GetDefaultIcon(), Guid.Empty),
                new ExtensionDataSet(null, "Ex5", GetDefaultIcon(), Guid.Empty),
                new ExtensionDataSet(null, "Ex6", GetDefaultIcon(), Guid.Empty),
                new ExtensionDataSet(null, "Ex7", GetDefaultIcon(), Guid.Empty),
                new ExtensionDataSet(null, "Ex8", GetDefaultIcon(), Guid.Empty),
                new ExtensionDataSet(null, "Ex9", GetDefaultIcon(), Guid.Empty),
            };

            extensions.First().IsSelected = true;

            return extensions;
        }

        private static ImageSource GetDefaultIcon()
        {
            return new BitmapImage(new Uri("pack://application:,,,/ProjectWerner;component/Images/default-icon.png"));
        }
    }
}