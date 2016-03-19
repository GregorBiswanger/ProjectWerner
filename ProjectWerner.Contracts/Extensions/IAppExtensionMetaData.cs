using System.ComponentModel;

namespace ProjectWerner.Contracts.Extensions
{
	public interface IAppExtensionMetaData
	{
		[DefaultValue("NoExtName")]
		string Name { get; }
	}
}
