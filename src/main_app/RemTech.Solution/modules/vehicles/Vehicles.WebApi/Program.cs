using RemTech.SharedKernel.Infrastructure.Database;
using Vehicles.WebApi.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.RegisterVehiclesModule(builder.Environment.IsDevelopment());
WebApplication app = builder.Build();
app.Services.ApplyModuleMigrations();
app.Run();