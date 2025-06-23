using FieldBank.Infrastructure.Extensions;
using FieldBank.Application.Extensions;
using MediatR;
using FieldBank.Application.Features.Fields.Queries.GetAllFields;
using FieldBank.Application.Features.Fields.Queries.GetFieldById;
using FieldBank.Application.Features.Fields.Commands.CreateField;
using FieldBank.Application.Features.Fields.Commands.UpdateField;
using FieldBank.Application.Features.Fields.Commands.DeleteField;
using Amazon.Lambda.AspNetCoreServer.Hosting;
using Serilog;
using Microsoft.AspNetCore.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Inicializa Serilog desde la configuración
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();
builder.Host.UseSerilog();

// Para correr como Lambda HTTP API (opcional, si quieres probar en AWS Lambda)
builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);

// DI y configuración de tu solución
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
Log.Information("Cadena de conexión utilizada: {ConnectionString}", connectionString);
builder.Services.AddFieldBankDbContext(builder.Configuration);
builder.Services.AddFieldBankInfrastructure();
builder.Services.AddFieldBankApplicationServices();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateFieldCommand).Assembly));
builder.Services.AddLogging();

// Swagger para desarrollo
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
    var field = await mediator.Send(command);
    return Results.Created($"/fields/{field.Id}", field);
});

// UPDATE
app.MapPut("/fields/{id:int}", async (int id, UpdateFieldCommand command, IMediator mediator) =>
{
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

app.Run();
