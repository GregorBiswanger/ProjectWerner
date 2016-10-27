using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ProjectWerner.Contracts.Extensions;
using ProjectWerner.Dto;

namespace ProjectWerner.Services
{
	internal class ExtensionLoader : IExtensionLoader
	{

		#pragma warning disable 649

		[ImportMany(typeof (IAppExtension))]
		private IEnumerable<Lazy<IAppExtension, IAppExtensionMetaData>> _extensions;
		

		#pragma warning restore 649


		public ExtensionLoader()
		{
			App.CompositionContainer.ComposeParts(this);
		}
		
		public IReadOnlyList<ExtensionDataSet> GetExtensions ()
		{
			return _extensions.Select(importedData => new ExtensionDataSet(importedData.Value, importedData.Metadata.Name, GetDefaultIcon(), Guid.NewGuid()))
								.ToList();
		}

		private static ImageSource GetDefaultIcon ()
		{
			return new BitmapImage(new Uri("pack://application:,,,/ProjectWerner;component/Images/default-icon.png"));
		}
	}
}
