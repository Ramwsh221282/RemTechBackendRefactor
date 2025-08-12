namespace RemTech.Core.Shared.Serialization.Primitives;

public sealed class DateTimeSerJson(string key, DateTime value) : PrimitiveSerJson(key, value);
