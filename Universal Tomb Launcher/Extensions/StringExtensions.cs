using System;

namespace UniversalTombLauncher.Extensions
{
	internal static class StringExtensions
	{
		/// <summary>
		/// Checks if the given string equals any of the provided strings using the specified comparison type.
		/// </summary>
		public static bool EqualsAny(this string value, StringComparison comparisonType, params string[] strings)
			=> Array.Exists(strings, x => x.Equals(value, comparisonType));
	}
}
