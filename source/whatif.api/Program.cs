using System.Text;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using WhatIf.Api.Actors;
using WhatIf.Api.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<ITokenService>(new TokenService(builder.Configuration["Jwt:Key"], builder.Configuration["Jwt:Issuer"]));

builder.WebHost.ConfigureKestrel(options => options.ListenLocalhost(5178));
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt =>
{
    opt.TokenValidationParameters = new ()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

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
