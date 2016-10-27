using System;
using System.Windows.Media;
using ProjectWerner.Contracts.Extensions;
using PropertyChanged;

namespace ProjectWerner.Dto
{
    [ImplementPropertyChanged]
    internal class ExtensionDataSet
    {
        public string Name { get; }
        public ImageSource Icon { get; }
        public Guid Id { get; }
        public IAppExtension Extension { get; }
        public bool IsSelected { get; set; }

        public ExtensionDataSet(IAppExtension extension, string name, ImageSource icon, Guid id)
        {
            Name = name;
            Icon = icon;
            Id = id;
            Extension = extension;
            IsSelected = false;
        }
    }
}