namespace RemTech.Core.Shared.Serialization.Primitives;

public sealed class GuidSerJson(string key, Guid value) : PrimitiveSerJson(key, value);
