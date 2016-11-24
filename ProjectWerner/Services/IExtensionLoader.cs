using System.Collections.Generic;
using ProjectWerner.Dto;

namespace ProjectWerner.Services
{
	internal interface IExtensionLoader
	{
		IReadOnlyList<ExtensionDataSet> GetExtensions();
	}
}