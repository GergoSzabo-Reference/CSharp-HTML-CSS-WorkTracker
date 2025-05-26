using Autoszerelo_API.Data;
using Autoszerelo_API.Services;
using Microsoft.EntityFrameworkCore; //sqlserver

var builder = WebApplication.CreateBuilder(args);

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

app.UseAuthorization();

app.MapControllers();

app.Run();
