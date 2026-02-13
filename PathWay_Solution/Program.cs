using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using PathWay_Solution.Data;
using PathWay_Solution.Data.Seeder;
using PathWay_Solution.Models.IdentityModels;
using PathWay_Solution.Services;
using System;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(opt=>
{
    opt.JsonSerializerOptions.ReferenceHandler= System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();

builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(opt =>
{
    opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
        RoleClaimType = ClaimTypes.Role,    
        NameClaimType = ClaimTypes.NameIdentifier 
    };
});

// for who what can do
builder.Services.AddAuthorization(opt =>
{
    opt.AddPolicy("AdminOnly", policy =>
    policy.RequireRole("Admin"));

    opt.AddPolicy("AdminOrCounterStaff", policy =>
    policy.RequireRole("Admin", "CounterStaff"));
});

//builder.Services.AddSwaggerGen();

//builder.Services.AddSwaggerGen(options =>
//{
//    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
//    {
//        Name = "Authorization",
//        In = ParameterLocation.Header,
//        Type = SecuritySchemeType.Http,
//        Scheme = "Bearer",
//        BearerFormat = "JWT"
//    });

//    options.AddSecurityRequirement(new OpenApiSecurityRequirement
//    {
//        {
//            new OpenApiSecurityScheme
//            {
//                Reference = new OpenApiReference
//                {
//                    Type = ReferenceType.SecurityScheme,
//                    Id = "Bearer"
//                }
//            },
//            Array.Empty<string>()
//        }
//    });
//});

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {   
         [   new OpenApiSecuritySchemeReference("Bearer", document)] = []
        
    });
});

builder.Services.AddDbContext<PathwayDBContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});

//  IdentityUser and IdentityRole represent the default user and role entities

////mvc+api
//builder.Services.AddIdentity<AppUser,AppRole>(opt =>
//{
//    opt.Password.RequiredLength = 5;
//    opt.Password.RequireUppercase = true;
//    opt.Password.RequireLowercase = true;
//    opt.Password.RequireDigit = true;
//    opt.Password.RequireNonAlphanumeric = true;
//})
//    .AddEntityFrameworkStores<PathwayDBContext>()
//    .AddDefaultTokenProviders();

//builder.Services.AddIdentityCore<AppUser>()   // AddIdentityCore for restapi and frontend ,jwt
//    .AddRoles<AppRole>()
//    .AddEntityFrameworkStores<PathwayDBContext>();

builder.Services
    .AddIdentityCore<AppUser>(options =>
    {
        options.User.RequireUniqueEmail = true;
    })
    .AddRoles<AppRole>()
    .AddEntityFrameworkStores<PathwayDBContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<IImageUpload,ImageUpload>();

var app = builder.Build();

// Identity + RoleManager
using (var scope = app.Services.CreateScope())
{
    var rolemanager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();
    var usermanager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
    var db = scope.ServiceProvider.GetRequiredService<PathwayDBContext>();

    await RoleSeeder.SeedAsync(rolemanager);
    await LocationSeeder.SeedAsync(db);
    await RouteSeeder.SeedAsync(db);
    await CounterSeeder.SeedAsync(db);
    await AdminUserSeeder.SeedAsync(usermanager, rolemanager, db);
    await CounterStaffSeeder.SeedAsync(usermanager,rolemanager,db); //need to maintain sequence
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) //1st:developerException
{
    app.UseSwagger();
    app.UseSwaggerUI();
    //app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseRouting();  //second

//app.UseCors();

app.UseAuthentication(); //3rd
app.UseAuthorization(); //4th

app.MapControllers();

//app.MapGet("/",()=>"");

app.Run();
