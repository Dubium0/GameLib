using GameLibApi.Data;
using GameLibApi.EndPoints;
using Microsoft.EntityFrameworkCore;


var  MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var connectionString = builder.Configuration.GetConnectionString("GameLibTest");



builder.Services.AddDbContext<GameLibContext>(options =>
    options.UseNpgsql(connectionString));


 
var app = builder.Build();
app.UseCors("AllowAll");

app.UseCors(MyAllowSpecificOrigins);
app.MapGamesEndpoints();
app.MapGenresEndpoints();
app.MapPlatformsEndpoints();


await app.MigrateDbAsync();

app.Run();
