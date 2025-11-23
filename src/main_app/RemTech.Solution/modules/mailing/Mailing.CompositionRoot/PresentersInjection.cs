using Mailing.Presenters;
using Mailing.Presenters.Inbox.CreateInboxMessage;
using Mailing.Presenters.Mailers.AddMailer;
using Mailing.Presenters.Mailers.TestEmailSending;
using Mailing.Presenters.Mailers.UpdateMailer;
using Microsoft.Extensions.DependencyInjection;

namespace Mailing.CompositionRoot;

public static class PresentersInjection
{
    extension(IServiceCollection services)
    {
        public void AddMailerPresentationDependencies()
        {
            services.AddAddInboxMessageGateway();
            services.AddMailerGateway();
            services.AddTestMailerGateway();
            services.AddUpdateMailerGateway();
        }

        private void AddAddInboxMessageGateway()
        {
            services.AddScoped<
                IGateway<CreateInboxMessageRequest, CreateInboxMessageResponse>,
                CreateInboxMessageGateway>();
        }

        private void AddMailerGateway()
        {
            services.AddScoped<
                IGateway<AddMailerRequest, AddMailerResponse>,
                AddMailerGateway>();
        }

        private void AddTestMailerGateway()
        {
            services.AddScoped<
                IGateway<TestEmailSendingRequest, TestEmailSendingResponse>,
                TestEmailSendingGateway>();
        }

        private void AddUpdateMailerGateway()
        {
            services.AddScoped<
            IGateway<UpdateMailerRequest, UpdateMailerResponse>,
            UpdateMailerGateway>();
        }
    }
}