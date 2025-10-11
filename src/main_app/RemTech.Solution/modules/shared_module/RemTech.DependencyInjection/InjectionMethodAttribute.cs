namespace RemTech.DependencyInjection;

// аттрибут метка для методов, которые делают инъекцию
[AttributeUsage(AttributeTargets.Method)]
public sealed class InjectionMethodAttribute : Attribute { }
