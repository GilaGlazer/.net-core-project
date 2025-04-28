using webApiProject.Middlewares;
using webApiProject.Models;
using webApiProject.Services;
using Scalar.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
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
                        UsersTokenService.GetTokenValidationParameters();
                });

builder.Services.AddAuthorization(cfg =>
        {
            cfg.AddPolicy("Admin",
                policy => policy.RequireClaim("type", "Admin"));
            cfg.AddPolicy("Agent",
                policy => policy.RequireClaim("type", "Agent", "Admin"));
            cfg.AddPolicy("ClearanceLevel1",
                policy => policy.RequireClaim("ClearanceLevel", "1", "2"));
            cfg.AddPolicy("ClearanceLevel2",
                policy => policy.RequireClaim("ClearanceLevel", "2"));
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

builder.Services.AddControllers();
builder.Services.AddItemJson<Shoes>();
builder.Services.AddItemJson<Users>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    // app.UseSwaggerUI();
    //   app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
    });
    app.MapScalarApiReference(options =>
    options.WithTheme(ScalarTheme.Mars)
    );
}


///////
//app.UseMailMiddleware();
app.UseLog();
app.UseErrorMiddleware();
///////


//app.UseHttpsRedirection();
/*js*/
app.UseDefaultFiles();
app.UseStaticFiles();


app.UseAuthorization();

app.MapControllers();

app.Run();


