using System;

namespace UniversalTombLauncher.Extensions
{
	public static class StringExtensions
	{
		public static bool BulkStringComparision(this string value, StringComparison comparisonType, params string[] strings) =>
			Array.Exists(strings, x => x.Equals(value, comparisonType));
	}
}
