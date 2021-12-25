using WhatIf.Api.Actors;

var builder = WebApplication.CreateBuilder(args);
// var  MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

// builder.Services.AddCors(options =>
// {
//     options.AddPolicy(name: MyAllowSpecificOrigins,
//                       builder =>
//                       {
//                           builder.WithOrigins("*")
//                                  .AllowAnyHeader()
//                                  .AllowAnyMethod();
//                       });
//});
// builder.Services
   // .AddMvc()
    // .AddDapr(); // build => build.UseHttpEndpoint("http://localhost:3500").UseGrpcEndpoint("http://localhost:60001"));

builder.Services.AddControllers().AddDapr();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddActors(config => {
    config.Actors.RegisterActor<PairActor>();
    config.ActorIdleTimeout = TimeSpan.FromMinutes(20);
});
// builder.Services.AddDaprSidekick(builder.Configuration, c => {
//     c.Sidecar ??= new Man.Dapr.Sidekick.DaprSidecarOptions();
//     c.Sidecar.AppId = "whatifapi";
// });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.UseAuthorization();
app.UseCloudEvents();
app.MapActorsHandlers();
app.MapSubscribeHandler();
app.MapControllers();
// app.MapHealthChecks("/health");

app.Run();
