using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddAuthentication().AddJwtBearer();
builder.Services.AddAuthorization();

builder.Services.AddSwaggerGen();

//builder.Services.AddDbContext<PathWayDB>(opt =>
//{
//    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
//});

var app = builder.Build();

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

app.MapGet("/",()=>"");
app.Run();
