using System.ComponentModel.Composition;
using ProjectWerner.Contracts.Extensions;

namespace ProjectWerner.Services
{
	internal class ExtensionLoader
	{
		[Import]
		private IAppExtension appExtension;

		public ExtensionLoader()
		{
			App.CompositionContainer.ComposeParts(this);
		}

		public IAppExtension GetFaceToSpeachExtension()
		{
			return appExtension;
		}
	}
}
