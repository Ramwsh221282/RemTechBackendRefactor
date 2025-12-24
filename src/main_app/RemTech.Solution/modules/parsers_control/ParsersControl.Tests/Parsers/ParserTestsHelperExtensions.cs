using Microsoft.Extensions.DependencyInjection;
using ParsersControl.Core.Parsers.Contracts;
using ParsersControl.Core.Parsers.Features.EnableParser;
using ParsersControl.Core.Parsers.Features.StartParserWork;
using ParsersControl.Core.Parsers.Features.StopParserWork;
using ParsersControl.Core.Parsers.Features.SubscribeParser;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Tests.Parsers;

public static class ParserTestsHelperExtensions
{
    extension(IServiceProvider services)
    {
        public async Task<Result<SubscribedParser>> InvokeSubscription(string domain, string type, Guid id)
        {
            SubscribeParserCommand command = new(id, domain, type);
            await using AsyncServiceScope scope = services.CreateAsyncScope();
            ICommandHandler<SubscribeParserCommand, SubscribedParser> handler = scope.ServiceProvider.GetRequiredService<ICommandHandler<SubscribeParserCommand, SubscribedParser>>();
            return await handler.Execute(command);
        }

        public async Task<Result<ISubscribedParser>> GetParser(Guid id)
        {
            await using AsyncServiceScope scope = services.CreateAsyncScope();
            ISubscribedParsersRepository repository = scope.ServiceProvider.GetRequiredService<ISubscribedParsersRepository>();
            return await repository.Get(new SubscribedParserQuery(Id: id));
        }
        
        public async Task<bool> EnsureSaved(Guid id)
        {
            await using AsyncServiceScope scope = services.CreateAsyncScope();
            ISubscribedParsersRepository repository = scope.ServiceProvider.GetRequiredService<ISubscribedParsersRepository>();
            Result<ISubscribedParser> parser = await repository.Get(new SubscribedParserQuery(Id: id));
            return parser.IsSuccess;
        }
        
        public async Task<Result<SubscribedParser>> EnableParser(Guid id)
        {
            await using AsyncServiceScope scope = services.CreateAsyncScope();
            ICommandHandler<EnableParserCommand, SubscribedParser> handler =
                scope.ServiceProvider.GetRequiredService<ICommandHandler<EnableParserCommand, SubscribedParser>>();
            EnableParserCommand command = new(id);
            return await handler.Execute(command);
        }
    
        public async Task<Result<SubscribedParser>> StartParser(Guid id)
        {
            await using AsyncServiceScope scope = services.CreateAsyncScope();
            ICommandHandler<StartParserCommand, SubscribedParser> handler =
                scope.ServiceProvider.GetRequiredService<ICommandHandler<StartParserCommand, SubscribedParser>>();
            StartParserCommand command = new(id);
            return await handler.Execute(command);
        }
    
        public async Task<Result<SubscribedParser>> SleepParser(Guid id)
        {
            await using AsyncServiceScope scope = services.CreateAsyncScope();
            ICommandHandler<StartParserCommand, SubscribedParser> handler =
                scope.ServiceProvider.GetRequiredService<ICommandHandler<StartParserCommand, SubscribedParser>>();
            StartParserCommand command = new(id);
            return await handler.Execute(command);
        }
    
        public async Task<Result<SubscribedParser>> DisableParser(Guid id)
        {
            await using AsyncServiceScope scope = services.CreateAsyncScope();
            ICommandHandler<StopParserWorkCommand, SubscribedParser> handler =
                scope.ServiceProvider.GetRequiredService<ICommandHandler<StopParserWorkCommand, SubscribedParser>>();
            StopParserWorkCommand command = new(id);
            return await handler.Execute(command);
        }
    }
}