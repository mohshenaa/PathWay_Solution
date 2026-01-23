using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PathWay_Solution.Data;
using PathWay_Solution.Data.Seeder;
using PathWay_Solution.IdentityModels;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
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

builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<PathwayDBContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});

//  IdentityUser and IdentityRole represent the default user and role entities
builder.Services.AddIdentity<IdentityUser, IdentityRole>(opt =>
{
    opt.Password.RequiredLength = 5;
    opt.Password.RequireUppercase = true;
    opt.Password.RequireLowercase = true;
    opt.Password.RequireDigit = true;
    opt.Password.RequireNonAlphanumeric = true;
})
    .AddEntityFrameworkStores<PathwayDBContext>()
    .AddDefaultTokenProviders();

//builder.Services.AddIdentityCore<IdentityUser>()   // AddIdentityCore for restapi and frontend ,jwt
//    .AddRoles<IdentityRole>()
//    .AddEntityFrameworkStores<PathwayDBContext>();


var app = builder.Build();

// Identity + RoleManager
using (var scope = app.Services.CreateScope())
{
    var rolemanager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();
    var usermanager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

    await RoleSeeder.SeedAsync(rolemanager);
    await AdminUserSeeder.SeedAsync(usermanager);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) //1st:developerException
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseRouting();  //second

//app.UseCors();

app.UseAuthentication(); //3rd
app.UseAuthorization(); //4th

app.MapControllers();

//app.MapGet("/",()=>"");

app.Run();
