using ContainedItems.Worker.Extensions;
using RemTech.SharedKernel.Infrastructure.Database;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
builder.Services.AddContainedItemsModule(builder.Environment.IsDevelopment());
IHost host = builder.Build();
host.Services.ApplyModuleMigrations();
host.Run();
