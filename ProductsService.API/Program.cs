using BusinessLogicLayer;
using DataAccessLayer;
using FluentValidation.AspNetCore;
using ProductsService.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add DAL & BLL to the DI container
builder.Services.AddDataAccessLayer(builder.Configuration);
builder.Services.AddBusinessLogicLayer();

builder.Services.AddControllers();


//FluentValidations
builder.Services.AddFluentValidationAutoValidation();

var app = builder.Build();

app.UseExceptionHandlingMiddleware();
app.UseRouting();

//Auth
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
