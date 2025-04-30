using webApiProject.Middlewares;
using webApiProject.Models;
using webApiProject.Services;
using Scalar.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using webApiProject.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication(options =>
                {
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(cfg =>
                {
                    cfg.RequireHttpsMetadata = false;
                    cfg.TokenValidationParameters =
                        AuthTokenService.GetTokenValidationParameters();
                });

builder.Services.AddAuthorization(cfg =>
        {
            cfg.AddPolicy("user",
                policy => policy.RequireClaim("type", "user", "admin"));
            cfg.AddPolicy("admin",
                policy => policy.RequireClaim("type", "admin"));
        });

builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Shoes", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter JWT with Bearer into field",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                { new OpenApiSecurityScheme
                        {
                         Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer"}
                        },
                    new string[] {}
                }
                });
            });



            
builder.Services.AddScoped<ActiveUserService>(); // Register ActiveUserService as Scoped
builder.Services.AddScoped<IService<Users>, UsersServiceJson>(); // רישום השירות `UsersServiceJson`
builder.Services.AddScoped<IService<Shoes>, ItemServiceJson<Shoes>>(); // רישום השירות `ItemServiceJson<Shoes>`

// הוספת Factory
builder.Services.AddScoped<Func<IService<Users>>>(sp => () => sp.GetRequiredService<IService<Users>>());
builder.Services.AddScoped<Func<IService<Shoes>>>(sp => () => sp.GetRequiredService<IService<Shoes>>());

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
    });
    app.MapScalarApiReference(options =>
    options.WithTheme(ScalarTheme.Mars)
    );
}

app.UseLog();
app.UseErrorMiddleware();

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseAuthentication();
app.UseMiddleware<UserMiddleware>(); 
app.UseAuthorization();

app.MapControllers();

app.Run();
