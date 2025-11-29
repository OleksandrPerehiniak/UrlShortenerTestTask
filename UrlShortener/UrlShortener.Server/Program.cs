using System.Text.Json;
using UrlShortener.Domain.Services;
using UrlShortener.Infrastructure.Data.Repositories;
using UrlShortener.Server.Extentions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });
builder.Services.AddSwaggerExplorer()
                .InjectDbContext(builder.Configuration)
                .AddAppConfig(builder.Configuration)
                .AddIdentityHandlersAndStores()
                .ConfigureIdentityOptions()
                .AddIdentityAuth(builder.Configuration);


builder.Services.AddScoped<IShortenedUrlsRepository, ShortenedUrlsRepository>();
builder.Services.AddScoped<IUrlShorteningService, UrlShorteningService>();
builder.Services.AddHttpContextAccessor();  

var app = builder.Build();

app.ConfigureSwaggerExplorer();


app.UseHttpsRedirection();

app.UseRouting();

app.ConfigureCORS(builder.Configuration);

app.AddIdentityAuthMiddlewares();

app.MapControllers();

app.Run();
