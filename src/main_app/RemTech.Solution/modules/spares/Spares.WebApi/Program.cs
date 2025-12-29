using RemTech.SharedKernel.Infrastructure.Database;
using Spares.WebApi.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.RegisterSparesModule(builder.Environment.IsDevelopment());
WebApplication app = builder.Build();
app.Services.ApplyModuleMigrations();
app.Run();