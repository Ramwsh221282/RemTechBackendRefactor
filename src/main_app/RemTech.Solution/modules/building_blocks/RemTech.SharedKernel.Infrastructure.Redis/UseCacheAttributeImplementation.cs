using System.Reflection;
using RemTech.SharedKernel.Core.Handlers.Decorators.CacheQuery;

namespace RemTech.SharedKernel.Infrastructure.Redis;

public static class UseCacheAttributeImplementation
{
	private const BindingFlags _inspectFlags =
		BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;

	extension(UseCacheAttribute)
	{
		public static bool ObjectContainsAttribute(Type type, ISet<Type> visited)
		{
			return !visited.Add(type) ? false
				: TypeMarkedWithAttribute(type) ? true
				: InspectTypeFields(type, visited) ? true
				: InspectProperties(type, visited);
		}
	}

	private static bool InspectTypeFields(Type type, ISet<Type> visited)
	{
		foreach (FieldInfo field in type.GetFields(_inspectFlags))
		{
			Type fieldType = field.FieldType;
			if (TypeMarkedWithAttribute(fieldType))
			{
				return true;
			}

			if (ObjectContainsAttribute(fieldType, visited))
			{
				return true;
			}
		}

		return false;
	}

	private static bool InspectProperties(Type type, ISet<Type> visited)
	{
		foreach (PropertyInfo prop in type.GetProperties(_inspectFlags))
		{
			if (prop.GetIndexParameters().Length > 0)
			{
				continue;
			}

			Type propertyType = prop.PropertyType;
			if (TypeMarkedWithAttribute(propertyType))
			{
				return true;
			}

			if (ObjectContainsAttribute(propertyType, visited))
			{
				return true;
			}
		}

		return false;
	}

	private static bool TypeMarkedWithAttribute(Type type)
	{
		return type.GetCustomAttribute<UseCacheAttribute>() != null;
	}
}
