using WhatIf.Api.Actors;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddDapr();
builder.Services.AddDaprSidekick();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddActors(config => {
    config.ActorIdleTimeout = TimeSpan.FromSeconds(10);
    // config.Actors.Add(new Dapr.Actors.Runtime.ActorRegistration()).Add().UseActor<IPairActor>();
});
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseCloudEvents();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
