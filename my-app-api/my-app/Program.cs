using my_app.Context;
using Microsoft.EntityFrameworkCore;
using my_app.Services;
using Nest;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var settings = new ConnectionSettings(new Uri("http://localhost:9200"))
        .DefaultIndex("students");
var client = new ElasticClient(settings);
builder.Services.AddSingleton<IElasticClient>(client);

var connectionString = builder.Configuration.GetConnectionString("DBConnection");
builder.Services.AddDbContext<myappContext>(options =>
                options.UseSqlServer(connectionString));

builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<ISenderService, SenderService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:5173"));

app.MapControllers();

app.Run();
