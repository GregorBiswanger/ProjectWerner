using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using ProjectWerner.API;
using ProjectWerner.Contracts.API;
using ProjectWerner.Features.Camera3DSimulator;
using ProjectWerner.ServiceLocator;
using ProjectWerner.Services;

namespace ProjectWerner
{

	public partial class App : Application
    {
		public static CompositionContainer CompositionContainer;

        protected override void OnStartup(StartupEventArgs e)
        {
            MicroKernel.Kernel.Bind<ICamera3D>().To<Camera3D>().InSingletonScope();
            MicroKernel.Kernel.Bind<IExtensionLoader>().To<ExtensionLoader>().InSingletonScope();


            if (e.Args.Contains("camera3d-simulator"))
            {
                var camera3DSimulatorView = new Camera3DSimulatorView();
                camera3DSimulatorView.Show();
            }

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

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            IExtensionLoader extensionloader = MicroKernel.Get<IExtensionLoader>();
            var allExtensions = extensionloader.GetExtensions();
            foreach (var extension in allExtensions)
            {
                extension.Extension.OnApplicationClosed();
            }
        }
    }
}
