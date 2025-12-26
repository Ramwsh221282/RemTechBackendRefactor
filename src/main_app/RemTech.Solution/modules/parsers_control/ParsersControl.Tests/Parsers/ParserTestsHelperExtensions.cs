using Microsoft.Extensions.DependencyInjection;
using ParsersControl.Core.Contracts;
using ParsersControl.Core.Features.AddParserLink;
using ParsersControl.Core.Features.ChangeLinkActivity;
using ParsersControl.Core.Features.DeleteLinkFromParser;
using ParsersControl.Core.Features.EnableParser;
using ParsersControl.Core.Features.SetLinkParsedAmount;
using ParsersControl.Core.Features.SetLinkWorkTime;
using ParsersControl.Core.Features.StartParserWork;
using ParsersControl.Core.Features.StopParserWork;
using ParsersControl.Core.Features.SubscribeParser;
using ParsersControl.Core.ParserLinks.Models;
using ParsersControl.Core.Parsers.Models;
using ParsersControl.Tests.Parsers.SubscribeParser;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Tests.Parsers;

public static class ParserTestsHelperExtensions
{
    extension(IServiceProvider services)
    {
        public async Task InvokeSubscriptionFromExternalService(string domain, string type, Guid id)
        {
            await using AsyncServiceScope scope = services.CreateAsyncScope();
            FakeParserSubscribeProducer producer =
                scope.ServiceProvider.GetRequiredService<FakeParserSubscribeProducer>();
            await producer.Publish(id, domain, type);
        }
        
        public async Task<Result<SubscribedParser>> InvokeSubscription(string domain, string type, Guid id)
        {
            SubscribeParserCommand command = new(id, domain, type);
            await using AsyncServiceScope scope = services.CreateAsyncScope();
            ICommandHandler<SubscribeParserCommand, SubscribedParser> handler = scope.ServiceProvider.GetRequiredService<ICommandHandler<SubscribeParserCommand, SubscribedParser>>();
            return await handler.Execute(command);
        }
    
        public async Task<Result<SubscribedParserLink>> AddLink(Guid parserId, string linkUrl, string linkName)
        {
            await using AsyncServiceScope scope = services.CreateAsyncScope();
            AddParserLinkCommand command = new(parserId, [ new AddParserLinkCommandArg(linkUrl, linkName) ]);
            ICommandHandler<AddParserLinkCommand, SubscribedParserLink> handler =
                scope.ServiceProvider.GetRequiredService<ICommandHandler<AddParserLinkCommand, SubscribedParserLink>>();
            return await handler.Execute(command);
        }

        public async Task<Result<ISubscribedParser>> GetParser(Guid parserId)
        {
            await using AsyncServiceScope scope = services.CreateAsyncScope();
            ISubscribedParsersRepository repository = scope.ServiceProvider.GetRequiredService<ISubscribedParsersRepository>();
            SubscribedParserQuery query = new(Id: parserId);
            return await repository.Get(query);
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
    
        public async Task<Result<SubscribedParserLink>> ChangeLinkActivity(
            Guid parserId, 
            Guid linkId, 
            bool activity)
        {
            await using AsyncServiceScope scope = services.CreateAsyncScope();
            ICommandHandler<ChangeLinkActivityCommand, SubscribedParserLink> service =
                scope.ServiceProvider.GetRequiredService<ICommandHandler<ChangeLinkActivityCommand, SubscribedParserLink>>();
            ChangeLinkActivityCommand command = new(parserId, linkId, activity);
            return await service.Execute(command);
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

        public async Task<Result<SubscribedParserLink>> SetLinkParsedAmount(
            Guid parserId,
            Guid linkId,
            int amount)
        {
            await using AsyncServiceScope scope = services.CreateAsyncScope();
            ICommandHandler<SetLinkParsedAmountCommand, SubscribedParserLink> handler =
                scope.ServiceProvider.GetRequiredService<ICommandHandler<SetLinkParsedAmountCommand, SubscribedParserLink>>();
            SetLinkParsedAmountCommand command = new(parserId, linkId, amount);
            return await handler.Execute(command);
        }

        public async Task<Result<SubscribedParserLink>> SetLinkWorkTime(
            Guid parserId,
            Guid linkId,
            long totalElapsedSeconds)
        {
            await using AsyncServiceScope scope = services.CreateAsyncScope();
            ICommandHandler<SetLinkWorkingTimeCommand, SubscribedParserLink> handler =
                scope.ServiceProvider.GetRequiredService<ICommandHandler<SetLinkWorkingTimeCommand, SubscribedParserLink>>();
            SetLinkWorkingTimeCommand command = new(parserId, linkId, totalElapsedSeconds);
            return await handler.Execute(command);
        }

        public async Task<Result<SubscribedParserLink>> RemoveLink(Guid parserId, Guid linkId)
        {
            await using AsyncServiceScope scope = services.CreateAsyncScope();
            ICommandHandler<DeleteLinkFromParserCommand, SubscribedParserLink> handler =
                scope.ServiceProvider.GetRequiredService<ICommandHandler<DeleteLinkFromParserCommand, SubscribedParserLink>>();
            DeleteLinkFromParserCommand command = new(parserId, linkId);
            return await handler.Execute(command);
        }
    }
}