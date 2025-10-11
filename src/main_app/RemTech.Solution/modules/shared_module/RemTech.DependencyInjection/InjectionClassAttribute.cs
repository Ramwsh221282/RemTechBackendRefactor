namespace RemTech.DependencyInjection;

// атрибут метка для классов, которые делают инъекцию
[AttributeUsage(AttributeTargets.Class)]
public sealed class InjectionClassAttribute : Attribute { }
