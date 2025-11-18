namespace CompositionRoot.Shared;

[AttributeUsage(AttributeTargets.Class)]
public class DependencyInjectionClassAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Method)]
public class DependencyInjectionMethodAttribute : Attribute { }