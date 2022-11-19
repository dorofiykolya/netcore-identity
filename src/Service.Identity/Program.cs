using System;
using System.Text;
using Common.Jwt;
using Common.Mongo;
using Common.Redis;
using Identity.Repositories.Caches;
using Identity.Services.Emails;
using Identity.Services.Identities;
using Identity.Services.Jwt;
using Identity.Services.Passwords;
using Identity.Services.Users;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

#region SERVICES

{
    builder.Services.AddMemoryCache();
    builder.Services.AddJwtGenerator(builder.Configuration.GetSection("Jwt"));
    builder.Services.AddMongoRepository(builder.Configuration.GetSection("Mongo"));
    builder.Services.AddRedisCache(builder.Configuration.GetSection("Redis"));
    builder.Services.AddEmailSender(builder.Configuration.GetSection("Email"));
    builder.Services.AddPasswordValidator(builder.Configuration.GetSection("Password"));
    builder.Services.AddPasswordGenerator(builder.Configuration.GetSection("Password"));
    builder.Services.AddInvalidPasswordOptions(builder.Configuration.GetSection("InvalidPassword"));
    builder.Services.AddUserScopeProvider(builder.Configuration.GetSection("Scopes"));
    builder.Services.AddEmailValidator();
    builder.Services.AddCors(service => service.AddDefaultPolicy(b => b.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));
    builder.Services.AddAuthorization();
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecurityKey"]))
            };
        });
}

#endregion

#region REPOSITORIES

{
    builder.Services.AddUserJwtTokenRepository();
}

#endregion

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        BearerFormat = "JWT",
        Name = "JWT Authentication",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        Description = "Put **_ONLY_** your JWT Bearer token on textbox below!",

        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };
    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, jwtSecurityScheme);
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {jwtSecurityScheme, Array.Empty<string>()}
    });
});

var app = builder.Build();

app.UseRouting();
app.UseCors();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

#region MIDDLEWARE

{
    app.UseIdentityMiddleware();
    app.UseJwtValidatorMiddleware();
}

#endregion

app.MapControllers();

app.Run();