using MediatR;
using Microsoft.OpenApi.Models;
using WhatIf.Api.Actors;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://localhost:5178");
var allowDaprForSwagger = "allowDaprForSwagger";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: allowDaprForSwagger,
                      builder =>
                      {
                          builder.WithOrigins("*")
                                 .AllowAnyHeader()
                                 .AllowAnyMethod();
                      });
});

builder.Services.AddControllers()
    .AddDapr();
builder.Services.AddMediatR(typeof(Program).Assembly);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(t => {
    t.AddServer(new OpenApiServer { Url = "http://localhost:3602/v1.0/invoke/whatifapi/method" });
});
builder.Services.AddActors(config => {
    config.Actors.RegisterActor<PairActor>();
    config.ActorIdleTimeout = TimeSpan.FromMinutes(20);
});

var app = builder.Build();
app.UseCors(allowDaprForSwagger);
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.UseCloudEvents();
app.MapActorsHandlers();
app.MapSubscribeHandler();
app.MapControllers();

app.Run();
