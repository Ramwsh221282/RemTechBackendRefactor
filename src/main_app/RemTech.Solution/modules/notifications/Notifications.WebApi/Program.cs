using Notifications.WebApi.Extensions;
using RemTech.SharedKernel.Infrastructure.Database;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddNotificationsModule(builder.Environment.IsDevelopment());
    
WebApplication app = builder.Build();
app.Services.ApplyModuleMigrations();

app.Run();

namespace Notifications.WebApi
{
    public partial class Program { }
}