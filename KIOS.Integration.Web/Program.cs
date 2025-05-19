using KIOS.Integration.Application.Commands;
using KIOS.Integration.Application.Handlers.QueryHandler;
using KIOS.Integration.Application.Services.Abstraction;
using KIOS.Integration.Application.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using POS_IntegrationCommonInfrastructure.Database;
using KIOS.Integration.Web.Services;
using KIOS.Integration.Web.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// JWT Configuration
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtKey = builder.Configuration["Jwt:Key"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtIssuer,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

// Add Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Indolge Integration With KDS",
        Version = "v1"
    });
});

// Mediatr & App Services
builder.Services.AddMediatR(typeof(GetAllUserTypeQueryHandler).Assembly);
builder.Services.AddScoped<GetAllUserTypeQueryHandler>();
builder.Services.AddScoped<CreateUserTypeCommand>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICreateOrderService, CreateOrderService>();
builder.Services.AddScoped<ICreateCashOrderService, CreateCashOrderService>();
builder.Services.AddScoped<ICheckPosStatusService, CheckPosStatusService>();
builder.Services.AddScoped<ICreateOrderDTService, CreateOrderDTService>();

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration["ConnectionStrings:AppDbConnection"]);
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});

// CORS
builder.Services.AddCors(p => p.AddPolicy("corsapp", policyBuilder =>
{
    policyBuilder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));

var app = builder.Build();

// Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Indolge Integration With KDS V1");
        c.RoutePrefix = "swagger"; // Default is "swagger"
    });
}

app.UseCors("corsapp");

// Optional: app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
