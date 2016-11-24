using System;
using System.ComponentModel.Composition;

namespace ProjectWerner.Contracts.Extensions
{
	[MetadataAttribute]
	public class AppExtensionMetadataAttribute : Attribute, IAppExtensionMetaData
	{
		public string Name { get; set; }				
	}
}