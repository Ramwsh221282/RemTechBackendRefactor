using RemTech.SharedKernel.Infrastructure.Database;
using RemTech.SharedKernel.NN;
using Vehicles.WebApi.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.RegisterVehiclesModule(builder.Environment.IsDevelopment());
builder.Services.AddSingleton<EmbeddingsProvider>();
WebApplication app = builder.Build();
app.Services.ApplyModuleMigrations();
app.Run();