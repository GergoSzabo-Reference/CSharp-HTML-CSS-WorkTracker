using Autoszerelo_API.Data;
using Autoszerelo_API.Services;
using Autoszerelo_API.Interfaces;
using Microsoft.EntityFrameworkCore; //sqlserver

var builder = WebApplication.CreateBuilder(args);
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins"; // Policy neve

// Add services to the container.
builder.Services.AddDbContext<AutoszereloDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

//will not need to create objects manually
//scoped: only 1 instance inside one HTTP request.Dispose() after
//stateless
//transient: always new
//singleton: every http gets the same object
builder.Services.AddScoped<WorkHourEstimationService>();
builder.Services.AddScoped<IUgyfelService, UgyfelService>();
builder.Services.AddScoped<IMunkaService, MunkaService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("https://localhost:7090", "http://localhost:5191", "https://localhost:7168", "http://localhost:7168")
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                      });
});

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(MyAllowSpecificOrigins);

app.UseAuthorization();

app.MapControllers();

app.Run();
