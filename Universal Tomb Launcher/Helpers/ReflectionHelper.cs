using System;
using System.Reflection;

namespace UniversalTombLauncher.Helpers
{
	internal static class ReflectionHelper
	{
		public static FieldInfo GetConstant(Type type, string name)
			=> Array.Find(type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy),
				x => x.IsLiteral && !x.IsInitOnly && x.Name.Equals(name));
	}
}
