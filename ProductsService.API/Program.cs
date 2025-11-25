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
builder.Services.AddSwaggerGen();


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

//Swagger
app.UseSwagger();
app.UseSwaggerUI();


//Auth
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Map minimal API product endpoints
app.MapProductAPIEndpoints();

app.Run();
