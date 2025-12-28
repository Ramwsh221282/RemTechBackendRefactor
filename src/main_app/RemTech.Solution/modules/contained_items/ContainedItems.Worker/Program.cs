using ContainedItems.Domain;
using ContainedItems.Infrastructure;
using RemTech.SharedKernel.Configurations;
using RemTech.SharedKernel.Core.Logging;
using RemTech.SharedKernel.Infrastructure.Database;
using RemTech.SharedKernel.Infrastructure.RabbitMq;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
builder.Services.RegisterLogging();
builder.Services.AddNpgSqlOptionsFromAppsettings();
builder.Services.AddRabbitMqOptionsFromAppsettings();
builder.Services.AddPostgres();
builder.Services.AddRabbitMq();
builder.Services.AddContainedItemsModule();
builder.Services.AddContainedItemsInfrastructure();
builder.Services.AddHostedService<AggregatedConsumersHostedService>();

IHost host = builder.Build();
host.Services.ApplyModuleMigrations();

host.Run();