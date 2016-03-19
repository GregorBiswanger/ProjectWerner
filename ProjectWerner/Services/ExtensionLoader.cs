using System.ComponentModel.Composition;
using ProjectWerner.Contracts.Extensions;

namespace ProjectWerner.Services
{
	internal class ExtensionLoader
	{
		[ImportMany]
		private IAppExtension[] appExtensions;

		public ExtensionLoader()
		{
			App.CompositionContainer.ComposeParts(this);
		}

		public IAppExtension GetFaceToSpeachExtension()
		{
			return appExtensions[0];
		}

		public IAppExtension GetA()
		{
			return appExtensions[1];
		}
	}
}
