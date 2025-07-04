using MediatR;
using Microsoft.AspNetCore.Diagnostics;
using Serilog;
using Valkyrie.Application.Extensions;
using Valkyrie.Application.Features.Fields.Commands.CreateField;
using Valkyrie.Application.Features.Fields.Commands.DeleteField;
using Valkyrie.Application.Features.Fields.Commands.UpdateField;
using Valkyrie.Application.Features.Fields.Queries.GetAllFields;
using Valkyrie.Application.Features.Fields.Queries.GetFieldById;
using Valkyrie.Infrastructure.Extensions;
using Valkyrie.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Initialize Serilog from configuration
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();
builder.Host.UseSerilog();

// To run as a Lambda HTTP API (optional, if you want to test in AWS Lambda)
builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);

// DI and solution configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
Log.Information("Connection string in use: {ConnectionString}", connectionString);
builder.Services.AddValkyrieDbContext(builder.Configuration);
builder.Services.AddValkyrieInfrastructure();
builder.Services.AddValkyrieApplicationServices();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateFieldCommand).Assembly));
builder.Services.AddLogging();

// Swagger for development
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        var exception = exceptionHandlerPathFeature?.Error;

        Log.Error(exception, "Unhandled exception");

        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync("{\"error\": \"Internal Server Error\"}");
    });
});

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ValkyrieDBContext>();
    SeedData.Seed(db);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ENDPOINTS

app.MapGet("", () => "Hello world!");

// GET ALL
app.MapGet("/fields", async (IMediator mediator) =>
{
    var fields = await mediator.Send(new GetAllFieldsQuery());
    return Results.Ok(fields);
});

// GET BY ID
app.MapGet("/fields/{id:int}", async (int id, IMediator mediator) =>
{
    var field = await mediator.Send(new GetFieldByIdQuery { Id = id });
    return field is not null ? Results.Ok(field) : Results.NotFound();
});

// CREATE
app.MapPost("/fields", async (CreateFieldCommand command, IMediator mediator) =>
{
    if (command.CategoryId <= 0)
        return Results.BadRequest("CategoryId is required");
    var field = await mediator.Send(command);
    return Results.Created($"/fields/{field.FieldId}", field);
});

// UPDATE
app.MapPut("/fields/{id:int}", async (int id, UpdateFieldCommand command, IMediator mediator) =>
{
    if (command.CategoryId <= 0)
        return Results.BadRequest("CategoryId is required");
    var updateCommand = command with { Id = id };
    var field = await mediator.Send(updateCommand);
    return Results.Ok(field);
});

// DELETE
app.MapDelete("/fields/{id:int}", async (int id, IMediator mediator) =>
{
    await mediator.Send(new DeleteFieldCommand { Id = id });
    return Results.NoContent();
});

// GET ALL CATEGORIES
app.MapGet("/categories", async (IMediator mediator) =>
{
    var categories = await mediator.Send(new Valkyrie.Application.Features.Categories.Queries.GetAllCategories.GetAllCategoriesQuery());
    return Results.Ok(categories);
});

// GET ALL FIELD TYPES
app.MapGet("/fieldtypes", async (IMediator mediator) =>
{
    var fieldTypes = await mediator.Send(new Valkyrie.Application.Features.FieldTypes.Queries.GetAllFieldTypes.GetAllFieldTypesQuery());
    return Results.Ok(fieldTypes);
});

app.Run();
