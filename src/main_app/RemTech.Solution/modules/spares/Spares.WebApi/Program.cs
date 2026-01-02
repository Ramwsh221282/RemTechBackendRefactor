using RemTech.SharedKernel.Infrastructure.Database;
using Spares.WebApi.Extensions;
using SwaggerThemes;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.RegisterSparesModule(builder.Environment.IsDevelopment());

WebApplication app = builder.Build();
app.Services.ApplyModuleMigrations();
app.UseSwagger();
app.UseSwaggerUI(Theme.UniversalDark);
app.MapControllers();
app.Run();