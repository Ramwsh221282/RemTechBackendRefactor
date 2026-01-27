using FluentValidation;
using FluentValidation.Results;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace RemTech.SharedKernel.Core.Handlers.Decorators.Validation;

/// <summary>
/// Расширения для валидации команд.
/// </summary>
public static class CommandValidationExtensions
{
	/// <summary>
	/// Проверяет, что значение является допустимым согласно предоставленной функции валидации.
	/// </summary>
	/// <typeparam name="T">Тип объекта, содержащего свойство для валидации.</typeparam>
	/// <typeparam name="TProperty">Тип свойства для валидации.</typeparam>
	/// <param name="builder">Построитель правил валидации.</param>
	/// <param name="validation">Функция валидации, возвращающая результат.</param>
	/// <returns>Опции построителя правил с условиями.</returns>
	public static IRuleBuilderOptionsConditions<T, TProperty> MustBeValid<T, TProperty>(
		this IRuleBuilderInitial<T, TProperty> builder,
		Func<TProperty, Result> validation
	) =>
		builder.Custom(
			(validate, context) =>
			{
				Result result = validation(validate);
				if (result.IsFailure)
				{
					string error = result.Error.Message;
					context.AddFailure(new ValidationFailure() { ErrorMessage = error });
				}
			}
		);

	/// <summary>
	/// Проверяет, что каждый элемент в коллекции соответствует всем предоставленным функциям валидации.
	/// </summary>
	/// <typeparam name="T">Тип объекта, содержащего коллекцию для валидации.</typeparam>
	/// <typeparam name="TProperty">Тип элемента коллекции для валидации.</typeparam>
	/// <param name="builder">Построитель правил валидации.</param>
	/// <param name="validations">Массив функций валидации для каждого элемента коллекции.</param>
	/// <returns>Опции построителя правил с условиями.</returns>
	public static IRuleBuilderOptionsConditions<T, IEnumerable<TProperty>> EachMustFollow<T, TProperty>(
		this IRuleBuilderInitial<T, IEnumerable<TProperty>> builder,
		Func<TProperty, Result>[] validations
	) =>
		builder.Custom(
			(validate, context) =>
			{
				List<ValidationFailure> failures = [];
				foreach (TProperty item in validate)
				{
					foreach (Func<TProperty, Result> validation in validations)
					{
						Result result = validation(item);
						if (result.IsFailure)
						{
							string error = result.Error.Message;
							failures.Add(new ValidationFailure() { ErrorMessage = error });
						}
					}
				}

				if (failures.Count > 0)
				{
					foreach (ValidationFailure failure in failures)
						context.AddFailure(failure);
				}
			}
		);

	/// <summary>
	/// Проверяет, что все элементы в коллекции являются допустимыми согласно предоставленной функции валидации.
	/// </summary>
	/// <typeparam name="T">Тип объекта, содержащего коллекцию для валидации.</typeparam>
	/// <typeparam name="TProperty">Тип элемента коллекции для валидации.</typeparam>
	/// <param name="builder">Построитель правил валидации.</param>
	/// <param name="validation">Функция валидации для каждого элемента коллекции.</param>
	/// <returns>Опции построителя правил с условиями.</returns>
	public static IRuleBuilderOptionsConditions<T, IEnumerable<TProperty>> AllMustBeValid<T, TProperty>(
		this IRuleBuilderInitial<T, IEnumerable<TProperty>> builder,
		Func<TProperty, Result> validation
	) =>
		builder.Custom(
			(validate, context) =>
			{
				List<ValidationFailure> failures = [];
				foreach (TProperty item in validate)
				{
					Result result = validation(item);
					if (result.IsFailure)
					{
						string error = result.Error.Message;
						failures.Add(new ValidationFailure() { ErrorMessage = error });
					}
				}

				if (failures.Count > 0)
				{
					foreach (ValidationFailure failure in failures)
						context.AddFailure(failure);
				}
			}
		);
}
