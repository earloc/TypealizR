using System;
using Microsoft.Extensions.Localization;

namespace TypealizR.Abstractions
{
	public interface IGroupedStringLocalizer<T> : IStringLocalizer<T>
	{
	}
}
