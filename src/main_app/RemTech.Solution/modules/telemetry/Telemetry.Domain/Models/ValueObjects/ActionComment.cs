using RemTech.Core.Shared.Result;

namespace Telemetry.Domain.Models.ValueObjects;

/// <summary>
/// Комментарий к действию
/// </summary>
public sealed record ActionComment
{
    public string Value { get; }

    private ActionComment(string value) => Value = value;

    public static Status<IEnumerable<ActionComment>> Create(IEnumerable<string?> inputs)
    {
        List<string> errorMessages = [];
        List<ActionComment> comments = [];
        foreach (var input in inputs)
        {
            Status<ActionComment> comment = Create(input);
            if (comment.IsSuccess)
            {
                comments.Add(comment);
                continue;
            }

            errorMessages.Add(comment.Error.ErrorText);
        }

        return errorMessages.Any()
            ? Error.Validation($"Невалидные комментарии: {string.Join(", ", errorMessages)}")
            : Create(comments);
    }

    public static Status<IEnumerable<ActionComment>> Create(IEnumerable<ActionComment> comments)
    {
        ActionComment[] origin = comments.ToArray();
        ActionComment[] distinct = origin.Distinct().ToArray();
        if (origin.Length != distinct.Length)
        {
            ActionComment[] duplicates = origin.GroupBy(x => x.Value).SelectMany(d => d).ToArray();
            string duplicateValues = string.Join(" ,", duplicates.Select(d => d.Value));
            string errorMessage = $"Найдены дубликаты в комментариях к действию: {duplicateValues}";
            return Error.Validation(errorMessage);
        }

        return origin;
    }

    public static Status<ActionComment> Create(string? value) =>
        value switch
        {
            null => Error.Validation(
                "Комментарий к записи действия телеметрии не может быть пустым."
            ),
            not null when string.IsNullOrWhiteSpace(value) => Error.Validation(
                "Комментарий к записи действия телеметрии не может быть пустым."
            ),
            _ => new ActionComment(value),
        };
}
