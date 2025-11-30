using BusinessLogicLayer;
using DataAccessLayer;
using eCommerce.ProductsMicroService.API.APIEndpoints;
using FluentValidation.AspNetCore;
using ProductsService.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add DAL & BLL to the DI container
builder.Services.AddDataAccessLayer(builder.Configuration);
builder.Services.AddBusinessLogicLayer();

builder.Services.AddControllers();

//FluentValidations
builder.Services.AddFluentValidationAutoValidation();


//Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "eCommerce Products API",
        Version = "v1",
        Description = "API for managing Products"
    });
});


//Cors
builder.Services.AddCors(options => {
    options.AddDefaultPolicy(builder => {
        builder.WithOrigins("*")
        .AllowAnyMethod()
        .AllowAnyHeader();
    });
});


var app = builder.Build();

app.UseExceptionHandlingMiddleware();




app.UseRouting();

//Cors
app.UseCors();


//Swagger - should be early in the pipeline
// Swagger - always enabled
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "eCommerce Products API v1");
    c.RoutePrefix = string.Empty;  // Serve at root
});

// Redirect root to Swagger if needed
app.MapGet("/", () => Results.Redirect("/index.html")).ExcludeFromDescription();

//Auth
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Map minimal API product endpoints
app.MapProductAPIEndpoints();

app.Run();
