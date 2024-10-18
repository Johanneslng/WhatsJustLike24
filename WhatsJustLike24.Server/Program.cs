using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using WhatsJustLike24.Server.Data;
using WhatsJustLike24.Server.Data.Models;
using WhatsJustLike24.Server.Data.Mappers;
using WhatsJustLike24.Server.Services;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Mvc;
using WhatsJustLike24.Server.Data.Identity;

var builder = WebApplication.CreateBuilder(args);

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("https://localhost:4200")
                          .AllowCredentials()
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
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "Cookie Auth for Identity",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseSqlServer(
        builder.Configuration.GetConnectionString("AZURE_SQL_CONNECTIONSTRING")
    )
);

builder.Services.AddIdentityApiEndpoints<AppUser>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = false;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 10;

})
    .AddEntityFrameworkStores< ApplicationDbContext>();

builder.Services.AddAuthentication();

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app
    .MapGroup("/api")
    .MapIdentityApi<AppUser>();

app.UseHttpsRedirection();
app.UseCors(MyAllowSpecificOrigins);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
