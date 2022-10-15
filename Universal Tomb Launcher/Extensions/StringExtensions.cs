using System;

namespace UniversalTombLauncher.Extensions
{
	internal static class StringExtensions
	{
		public static bool BulkStringComparision(this string value, StringComparison comparisonType, params string[] strings)
			=> Array.Exists(strings, x => x.Equals(value, comparisonType));
	}
}
