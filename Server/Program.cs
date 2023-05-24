using Asp.Versioning;

using BlazorApp1.Server;
using BlazorApp1.Server.Data;
using BlazorApp1.Server.Endpoints;
using BlazorApp1.Server.Hubs;

using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using Microsoft.AspNetCore.ResponseCompression;

using NSwag.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration);

// Add services to the container.

builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddRazorPages();

builder.Services.AddSignalR();
builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        new[] { "application/octet-stream" });
});

builder.Services.AddSqlite<DataContext>(
    builder.Configuration.GetConnectionString("DefaultConnection"));

builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
})
.AddApiExplorer(option =>
{
    option.GroupNameFormat = "VVV";
    option.SubstituteApiVersionInUrl = true;
})
.EnableApiVersionBinding();

// Register the Swagger services

foreach (var description in ApiVersions.Versions)
{
    builder.Services.AddOpenApiDocument(config =>
    {
        config.DocumentName = $"v{GetApiVersion(description)}";
        config.PostProcess = document =>
        {
            document.Info.Title = "BlazorApp1 API";
            document.Info.Version = $"v{GetApiVersion(description)}";
        };
        config.ApiGroupNames = new[] { GetApiVersion(description) };

        config.DefaultReferenceTypeNullHandling = NJsonSchema.Generation.ReferenceTypeNullHandling.NotNull;

        // config.AddSecurity("JWT", new OpenApiSecurityScheme
        // {
        //     Type = OpenApiSecuritySchemeType.ApiKey,
        //     Name = "Authorization",
        //     In = OpenApiSecurityApiKeyLocation.Header,
        //     Description = "Type into the textbox: Bearer {your JWT token}."
        // });

        // config.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("JWT"));

        // config.SchemaNameGenerator = new CustomSchemaNameGenerator();
    });
}

var app = builder.Build();

app.UseResponseCompression();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStatusCodePages();

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

// Map endpoints

app
    .MapSampleDataEndpoints()
    .MapTodosEndpoints();

app.UseOpenApi();

app.UseSwaggerUi3(options =>
{
    var descriptions = app.DescribeApiVersions();

    // build a swagger endpoint for each discovered API version
    foreach (var description in descriptions)
    {
        var name = $"v{description.ApiVersion}";
        var url = $"/swagger/v{GetApiVersion(description)}/swagger.json";

        options.SwaggerRoutes.Add(new SwaggerUi3Route(name, url));
    }

    static string GetApiVersion(Asp.Versioning.ApiExplorer.ApiVersionDescription description)
    {
        var apiVersion = description.ApiVersion;
        return (apiVersion.MinorVersion == 0
            ? apiVersion.MajorVersion.ToString()
            : apiVersion.ToString())!;
    }
});

app.MapRazorPages();

app.MapHub<ChatHub>("/chathub");
app.MapFallbackToFile("index.html");

using (var scope = app.Services.CreateScope())
{
    using var context = scope.ServiceProvider.GetRequiredService<DataContext>();

    //await context.Database.EnsureDeletedAsync();
    await context.Database.EnsureCreatedAsync();
}

await app.RunAsync();

static string GetApiVersion(ApiVersion version)
{
    return (version.MinorVersion == 0
        ? version.MajorVersion.ToString()
        : version.ToString())!;
}