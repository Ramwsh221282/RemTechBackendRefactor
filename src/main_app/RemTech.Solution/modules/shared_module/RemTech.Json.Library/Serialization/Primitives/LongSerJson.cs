namespace RemTech.Json.Library.Serialization.Primitives;

public sealed class LongSerJson(string key, long value) : PrimitiveSerJson(key, value);

public sealed class DoubleSerJson(string key, double value) : PrimitiveSerJson(key, value);
