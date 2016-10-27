using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Reflection;
using System.Windows;
using ProjectWerner.API;
using ProjectWerner.Contracts.API;
using ProjectWerner.ServiceLocator;

namespace ProjectWerner
{

	public partial class App : Application
    {
		public static CompositionContainer CompositionContainer;

        protected override void OnStartup(StartupEventArgs e)
        {
            MicroKernel.Kernel.Bind<ICamera3D>().To<Camera3D>().InSingletonScope();

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
