using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Reflection;
using System.Windows;
using ProjectWerner.Services;
using ProjectWerner.ViewModels.MainWindow;

namespace ProjectWerner
{

	public partial class App : Application
    {
		

		public static CompositionContainer CompositionContainer;

        protected override void OnStartup(StartupEventArgs e)
        {
            var aggregateCatalog = new AggregateCatalog();
            aggregateCatalog.Catalogs.Add(new AssemblyCatalog(Assembly.GetExecutingAssembly()));

            foreach (string directoryPath in Directory.EnumerateDirectories("Extensions"))
            {
                aggregateCatalog.Catalogs.Add(new DirectoryCatalog(directoryPath));
            }

            CompositionContainer = new CompositionContainer(aggregateCatalog);

            try
            {
                CompositionContainer.ComposeParts(this);
            }
            catch (CompositionException compositionException)
            {
                Console.WriteLine(compositionException.ToString());
            }

            base.OnStartup(e);

			var extensionLoader = new ExtensionLoader();

			var mainWindowViewModel = new MainWindowViewModel(extensionLoader);

	        var mainWindow = new MainWindow
	        {
				DataContext = mainWindowViewModel
	        };

			mainWindow.Show();

        }
    }
}
