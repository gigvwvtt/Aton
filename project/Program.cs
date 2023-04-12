using Microsoft.EntityFrameworkCore;
using project;
using project.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddScoped<IUserDbRepository, UserDbRepository>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(options => options
    .UseInMemoryDatabase("inMemoryDb"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


Seed.SeedData(app);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();