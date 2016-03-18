using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;

namespace ProjectWerner
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
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
        }
    }
}
