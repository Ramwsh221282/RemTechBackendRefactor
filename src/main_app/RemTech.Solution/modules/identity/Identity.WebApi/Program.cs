using Identity.WebApi.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.RegisterIdentityModule(builder.Configuration);
WebApplication app = builder.Build();
app.Run();

namespace Identity.WebApi
{
	public partial class Program { }
}
