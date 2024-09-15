using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using WhatsJustLike24.Server.Data;
using WhatsJustLike24.Server.Data.Models;
using WhatsJustLike24.Server.Data.Mappers;
using WhatsJustLike24.Server.Services;

var builder = WebApplication.CreateBuilder(args);

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("https://localhost:4200")
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                      });
});

// Add services to the container.
builder.Services.AddScoped<MovieApiService>();
builder.Services.AddScoped<MovieDTOMapper>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseSqlServer(
        builder.Configuration.GetConnectionString("AZURE_SQL_CONNECTIONSTRING")
    )
);

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(MyAllowSpecificOrigins);

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
