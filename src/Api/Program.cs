using Api.Middlewares;
using Api.Services;
using Application;
using Application.Interfaces;
using Asp.Versioning;
using Infrastructure;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddApiVersioning(action =>
{
    action.DefaultApiVersion = new ApiVersion(1,0);
    action.AssumeDefaultVersionWhenUnspecified = true;
    action.ReportApiVersions = true;
});

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes.Add("Bearer", new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "Please Enter JWT Token  : Bearer {token}"
        });
        document.SecurityRequirements.Add(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });

        document.Info = new OpenApiInfo
        {
            Title = "Blog API",
            Version = "v1",
            Description = "Simple Blog System with Clean Architecture",
            Contact = new OpenApiContact
            {
                Name = "Mohammad Jannesari",
                Email = "ompc.music@gmail.com"
            }
        };
        return Task.CompletedTask;
    });
});

#region CORS Strict Policy
if (builder.Environment.IsDevelopment() || builder.Environment.EnvironmentName == "Docker")
{
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("DevPolicy", policy =>
        {
            policy
                .WithOrigins("https://localhost:44346", "https://localhost:8080")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
                .SetPreflightMaxAge(TimeSpan.FromMinutes(60));
        });
    });
}
else
{
    builder.Services.AddCors(options =>
    {
        var corsSettings = builder.Configuration.GetSection("Cors");
        options.AddPolicy("ProductionPolicy", policy =>
        {
            policy
                .WithOrigins(corsSettings.GetSection("AllowedOrigins").Get<string[]>()!)
                .WithMethods(corsSettings.GetSection("AllowedMethods").Get<string[]>()!)
                .WithHeaders(corsSettings.GetSection("AllowedHeaders").Get<string[]>()!)
                .WithExposedHeaders(corsSettings.GetSection("ExposedHeaders").Get<string[]>()!)
                .AllowCredentials()
                .SetPreflightMaxAge(TimeSpan.FromMinutes(
                    corsSettings.GetValue<int>("PreflightMaxAgeInMinutes")));
        });
    });
}

#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.


app.MapOpenApi();
app.MapScalarApiReference(options =>
{
    options.WithTitle("Blog API");
    options.WithTheme(ScalarTheme.Purple);
    options.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    options.AddPreferredSecuritySchemes("Bearer");
    options.WithOpenApiRoutePattern("/openapi/v1.json");
});



if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Docker")
{
    app.UseCors("DevPolicy");
    app.UseDeveloperExceptionPage();
    app.MapGet("/", () => Results.LocalRedirect("/scalar/v1"));
}
else
{
    app.UseCors("ProductionPolicy");
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<SecurityHeadersMiddleware>();

app.UseRouting();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();
