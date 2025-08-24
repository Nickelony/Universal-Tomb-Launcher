using System;
using System.Reflection;

namespace UniversalTombLauncher.Helpers
{
	/// <summary>
	/// Helper methods for reflection operations.
	/// </summary>
	internal static class ReflectionHelper
	{
		/// <summary>
		/// Gets a constant field from a specified type by its name.
		/// </summary>
		public static FieldInfo GetConstant(Type type, string name)
			=> Array.Find(type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy),
				x => x.IsLiteral && !x.IsInitOnly && x.Name.Equals(name));
	}
}
