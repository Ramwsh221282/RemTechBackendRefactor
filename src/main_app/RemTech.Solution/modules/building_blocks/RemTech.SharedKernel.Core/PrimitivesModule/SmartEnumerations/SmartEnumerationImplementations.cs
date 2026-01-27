using System.Collections.Concurrent;
using System.Reflection;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace RemTech.SharedKernel.Core.PrimitivesModule.SmartEnumerations;

/// <summary>
/// Реализации умных перечислений.
/// </summary>
public static class SmartEnumerationImplementations
{
	private static readonly ConcurrentDictionary<Type, object[]> _families = new();

	/// <summary>
	/// Проверяет существование элемента умного перечисления.
	/// </summary>
	/// <typeparam name="TFrom">Тип исходного значения.</typeparam>
	/// <typeparam name="TEnum">Тип умного перечисления.</typeparam>
	/// <param name="from">Исходное значение для поиска.</param>
	/// <param name="matchFn">Функция сопоставления.</param>
	/// <param name="result">Найденный элемент умного перечисления.</param>
	/// <returns>True, если элемент найден; иначе false.</returns>
	public static bool Exists<TFrom, TEnum>(TFrom from, Func<TFrom, TEnum, bool> matchFn, out TEnum result)
		where TEnum : class
	{
		object[] familyMembers = ResolveFamilies<TEnum>();
		Result<TEnum> resolved = ResolveMember(familyMembers, from, matchFn);
		if (resolved.IsFailure)
		{
			result = null!;
			return false;
		}

		result = resolved.Value;
		return true;
	}

	private static object[] ResolveFamilies<TEnum>()
		where TEnum : class
	{
		Type familyMemberType = typeof(TEnum);
		return !IsFamilyDeclarer<TEnum>(familyMemberType)
			? throw new ApplicationException($"{familyMemberType.Name} is not a smart enumerations family declarer.")
			: _families.GetOrAdd(familyMemberType, _ => [.. LoadFamilies<TEnum>()]);
	}

	private static Result<TEnum> ResolveMember<TFrom, TEnum>(
		object[] families,
		TFrom from,
		Func<TFrom, TEnum, bool> matcher
	)
		where TEnum : class
	{
		foreach (TEnum member in families.Cast<TEnum>())
		{
			if (matcher(from, member))
			{
				return member;
			}
		}

		return Result.Failure<TEnum>(Error.Application($"Unable to find family: {typeof(TEnum).FullName}."));
	}

	private static bool IsFamilyDeclarer<TEnum>(Type type)
		where TEnum : class => type.GetCustomAttribute<SmartEnumerationAttribute<TEnum>>() != null && type.IsAbstract;

	private static IEnumerable<TEnum> LoadFamilies<TEnum>()
		where TEnum : class
	{
		Type type = typeof(TEnum);
		Assembly assembly = type.Assembly;
		return assembly
			.GetTypes()
			.Where(t => t.IsClass && !t.IsAbstract && t.GetCustomAttribute<SmartEnumerationAttribute<TEnum>>() != null)
			.Select(t => (TEnum)Activator.CreateInstance(t)!);
	}
}

/// <summary>
/// Атрибут для обозначения класса как декларатора умного перечисления.
/// </summary>
/// <typeparam name="TEnum">Тип умного перечисления.</typeparam>
[AttributeUsage(AttributeTargets.Class)]
public sealed class SmartEnumerationAttribute<TEnum> : Attribute
	where TEnum : class;
