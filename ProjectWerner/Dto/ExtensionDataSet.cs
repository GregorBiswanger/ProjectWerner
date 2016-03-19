using System;
using System.ComponentModel;
using System.Windows.Media;
using ProjectWerner.Contracts.Extensions;

#pragma warning disable 0067

namespace ProjectWerner.Dto
{
	internal class ExtensionDataSet : INotifyPropertyChanged
	{
		public ExtensionDataSet(IAppExtension extension, string name, ImageSource icon, Guid id)
		{
			Name = name;
			Icon = icon;
			Id = id;
			Extension = extension;
		}

		public string        Name      { get; }
		public ImageSource   Icon      { get; }
		public Guid          Id        { get; }
		public IAppExtension Extension { get; }

		public event PropertyChangedEventHandler PropertyChanged;
	}
}
