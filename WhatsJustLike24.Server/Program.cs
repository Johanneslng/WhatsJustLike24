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
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Azure.Storage.Blobs;
using RestSharp;

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
builder.Services.AddScoped<ShowApiService>();
builder.Services.AddScoped<BookApiService>();
builder.Services.AddScoped<MovieDTOMapper>();
builder.Services.AddScoped<ShowDTOMapper>();
builder.Services.AddScoped<GameDTOMapper>();
builder.Services.AddScoped<BookDTOMapper>();
builder.Services.AddScoped<GameApiService>();
builder.Services.AddScoped<GameLookupService>();
builder.Services.AddSingleton(s => new RestClient("https://api.example.com"));
builder.Services.AddSingleton<TokenService>();
builder.Services.AddSingleton<ImageBlobService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Fill in the JWT token",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new List<String>()
        }
    });
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

builder.Services.AddAuthentication(x =>
    {
        x.DefaultAuthenticateScheme =
        x.DefaultChallengeScheme =
        x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(y =>
    {
        y.SaveToken = false;
        y.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    builder.Configuration["JWT:SigningKey"]!)),
            ValidateIssuer = false,
            ValidateAudience = false,
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
      .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
      .RequireAuthenticatedUser()
      .Build();
});

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
