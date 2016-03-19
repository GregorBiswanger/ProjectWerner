using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using ProjectWerner.Contracts.Extensions;
using ProjectWerner.Dto;

namespace ProjectWerner.Services
{
	internal class ExtensionLoader : IExtensionLoader
	{

		#pragma warning disable 649

		[ImportMany]
		private IAppExtension[] appExtensions;

		#pragma warning restore 649


		public ExtensionLoader()
		{
			App.CompositionContainer.ComposeParts(this);
		}
		
		public IReadOnlyList<ExtensionDataSet> GetExtensions ()
		{
			return appExtensions.Select(extension => new ExtensionDataSet(extension, "no name", null, Guid.NewGuid()))
								.ToList();
		}
	}
}
