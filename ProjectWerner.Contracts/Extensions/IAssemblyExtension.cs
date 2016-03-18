using System.ComponentModel.Composition;

namespace ProjectWerner.Contracts.Extensions
{
    [InheritedExport]
    public interface IAssemblyExtension
    {
        void Run();
    }
}
