using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Core;

public static class ParsersDependencyInjection
{
    public static void RegisterParserControlHandlers(this IServiceCollection services)
    {
        Assembly assembly = typeof(SubscribedParser).Assembly;
        
        services.Scan(x => x.FromAssemblies([assembly])
            .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<,>)))
            .AsSelfWithInterfaces()
            .WithScopedLifetime()
        );
        
        services.AddValidatorsFromAssembly(assembly);
        services.RegisterLoggingHandlers();
        services.RegisterValidatingHandlers();
        services.UseValidatingHandlers();
        services.UseLoggingHandlers();
    }
}