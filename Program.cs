using webApiProject.Middlewares;
using webApiProject.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddShoesConst();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


///////
app.UseMailMiddleware();
app.UseLog();
app.UseErrorMiddleware();
///////


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
