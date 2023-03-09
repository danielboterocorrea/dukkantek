using dukkantek.Api;
using dukkantek.Api.Middlewares;
using MediatR;
using Serilog;



var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMediatR(typeof(Program));

builder.Host
    .UseSerilog((context, configuration) => configuration.WriteTo.Console())
    .ConfigureAppConfiguration((_, configurationBuilder) => configurationBuilder.BuildConfiguration())
    .ConfigureServices((context, collection) => collection.BuildServices(context.Configuration))
    .ConfigureLogging((context, loggingBuilder) => loggingBuilder.AddSerilog());

var app = builder.Build();

app.UseSwagger();
app.MapSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapControllers();

app.UseExceptionMiddleware();

app.Run();