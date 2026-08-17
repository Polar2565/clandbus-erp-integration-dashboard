using System.Net;
using ClandbusERPIntegration.Configurations;
using ClandbusERPIntegration.Interfaces;
using ClandbusERPIntegration.Services;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// ======================================
// CONFIGURATION
// ======================================

builder.Services
    .AddOptions<AcumaticaSettings>()
    .Bind(builder.Configuration.GetSection("Acumatica"))
    .Validate(
        settings => Uri.TryCreate(settings.BaseUrl, UriKind.Absolute, out _),
        "Acumatica:BaseUrl must be an absolute URL.")
    .ValidateOnStart();

// ======================================
// ERP SESSION / HTTP CLIENT
// ======================================

builder.Services.AddSingleton<CookieContainer>();

builder.Services.AddSingleton<IAcumaticaService>(
    serviceProvider =>
    {
        var settings =
            serviceProvider
                .GetRequiredService<
                    IOptions<AcumaticaSettings>>();

        var cookies =
            serviceProvider
                .GetRequiredService<
                    CookieContainer>();

        var handler =
            new HttpClientHandler
            {
                CookieContainer = cookies,

                UseCookies = true
            };

        var httpClient =
            new HttpClient(handler);

        return new AcumaticaService(
            httpClient,
            settings);
    });

// ======================================
// CONTROLLERS
// ======================================

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

// ======================================
// CORS
// ======================================

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AngularPolicy",
        policy =>
        {
            policy
                .WithOrigins(
                    "http://localhost:4200"
                )
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});

var app = builder.Build();

// ======================================
// PIPELINE
// ======================================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AngularPolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();
