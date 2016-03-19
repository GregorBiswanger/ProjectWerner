using System;
using System.ComponentModel;
using System.Windows.Media;
using ProjectWerner.Contracts.Extensions;
using ProjectWerner.MvvmHelper.Notifier;

namespace ProjectWerner.Dto
{
	internal class ExtensionDataSet : INotifyPropertyChanged
	{
		private bool isSelected;

		public ExtensionDataSet(IAppExtension extension, string name, ImageSource icon, Guid id)
		{
			Name = name;
			Icon = icon;
			Id = id;
			Extension = extension;
			IsSelected = false;
		}

		public string        Name       { get; private set; }
		public ImageSource   Icon       { get; private set; }
		public Guid          Id         { get; private set; }						
		public IAppExtension Extension  { get; private set; }

		public bool IsSelected
		{
			get { return isSelected; }
			set { PropertyChanged.ChangeAndNotify(this, ref isSelected, value); }
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}
}
