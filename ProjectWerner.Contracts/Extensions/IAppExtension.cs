using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;

namespace ProjectWerner.Contracts.Extensions
{
    //[InheritedExport]
    public interface IAppExtension
    {
         UserControl AppUserControl { get; }
    }
}