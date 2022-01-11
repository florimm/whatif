using System.Security.Claims;
using System.Text;
using Dapr.Client;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using WhatIf.Api.Actors;
using WhatIf.Api.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<ITokenService>(
    new TokenService(builder.Configuration["Jwt:Key"], builder.Configuration["Jwt:Issuer"]));

builder.WebHost.ConfigureKestrel(options => options.ListenLocalhost(5178));

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new ()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService>(
    s => new CurrentUserService(principal: (ClaimsPrincipal)s.GetService<IHttpContextAccessor>().HttpContext.User));

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
    config.Actors.RegisterActor<MonitorActor>();
    config.ActorIdleTimeout = TimeSpan.FromMinutes(5);
});

var app = builder.Build();
app.UseCors(allowDaprForSwagger);
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseCloudEvents();

app.UseEndpoints(x => {    
    x.MapActorsHandlers();
    x.MapSubscribeHandler();
    x.MapControllers();
    x.MapPost("price-change", async (PairPriceChanged data, DaprClient daprClient) =>
    {
        await daprClient.SaveStateAsync<PairPriceChanged>("statestore", data.Pair, data);
        return Results.Ok();
    })
    .WithTopic("pubsub", "price-change");;
});


app.Run();
