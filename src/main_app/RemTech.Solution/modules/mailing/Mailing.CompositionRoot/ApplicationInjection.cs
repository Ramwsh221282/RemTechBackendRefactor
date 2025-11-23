using Mailing.Application;
using Mailing.Application.Inbox.CreateInboxMessage;
using Mailing.Application.Mailers.AddMailer;
using Mailing.Application.Mailers.SendEmail;
using Mailing.Application.Mailers.UpdateMailer;
using Mailing.Core.Inbox;
using Mailing.Core.Mailers;
using Microsoft.Extensions.DependencyInjection;

namespace Mailing.CompositionRoot;

public static class ApplicationInjection
{
    extension(IServiceCollection services)
    {
        public void AddMailingApplicationDependencies()
        {
            services.AddCreatePendingMessageCommand();
            services.AddAddMailerCommand();
            services.AddSendEmailCommand();
            services.AddUpdateMailerCommand(); 
        }

        private void AddCreatePendingMessageCommand()
        {
            services.AddScoped<
                ICommandHandler<CreateInboxMessageCommand, InboxMessage>, 
                CreateInboxMessageCommandHandler>();
        }

        private void AddAddMailerCommand()
        {
            services.AddScoped<
                ICommandHandler<AddMailerCommand, Mailer>,
                AddMailerCommandHandler>();
        }

        private void AddSendEmailCommand()
        {
            services.AddScoped<
                ICommandHandler<SendEmailCommand, DeliveredMessage>,
                SendEmailCommandHandler>();
        }
        
        private void AddUpdateMailerCommand()
        {
            services.AddScoped<
                ICommandHandler<UpdateMailerCommand, Mailer>,
                UpdateMailerCommandHandler>();
        }
    }
}