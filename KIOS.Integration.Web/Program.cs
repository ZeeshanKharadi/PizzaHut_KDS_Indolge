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

var builder = WebApplication.CreateBuilder(args);


var jwtIssuer = builder.Configuration.GetSection("Jwt:Issuer").Get<string>();
var jwtKey = builder.Configuration.GetSection("Jwt:Key").Get<string>();

//var provider = builder.Services.BuildServiceProvider();
//var config = provider.GetRequiredService<IConfiguration>();



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

// Add services to the container.

builder.Services.AddControllers();

//Add Mediatr
//builder.Services.AddMediatR(Assembly.GetExecutingAssembly());
builder.Services.AddScoped<GetAllUserTypeQueryHandler>();
builder.Services.AddScoped<CreateUserTypeCommand>();
//builder.Services.AddScoped<IUserTypeRepository, UserTypeRepository>();
//builder.Services.AddMediatR(typeof(UserTypeRepository).Assembly);

builder.Services.AddMediatR(typeof(GetAllUserTypeQueryHandler).Assembly);

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICreateOrderService, CreateOrderService>();
builder.Services.AddScoped<ICreateCashOrderService, CreateCashOrderService>();
builder.Services.AddScoped<ICheckPosStatusService, CheckPosStatusService>();
builder.Services.AddScoped<ICreateOrderDTService, CreateOrderDTService>();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration["ConnectionStrings:AppDbConnection"]);
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});

builder.Services.AddSwaggerGen(opt => opt.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
{
    //Title = "Kababjees CallCenter Integration API V3",
    Title = "Indolge Integration With KDS",

   // Description = "PrintSaleOrder KOT d365 f&O Integration version 3.0"
}));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Allow cors origin
builder.Services.AddCors(p => p.AddPolicy("corsapp", builder =>
{
    builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
//|| app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSwagger();
app.UseSwaggerUI();

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
