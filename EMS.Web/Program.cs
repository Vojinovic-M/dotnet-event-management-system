using Microsoft.EntityFrameworkCore;
using EMS.Infrastructure.Contexts;
using EMS.Infrastructure.Identity;
using EMS.Infrastructure.Services;
using EMS.Application.Interfaces;
using MediatR;
using System.Reflection;
using EMS.Application;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetAssembly(typeof(AssemblyReference))!)); 
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000")
            .AllowAnyHeader().AllowAnyMethod();
        });
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 21)),
        b => b.MigrationsAssembly("EMS.Web")));

builder.Services.AddAuthorization();
builder.Services.AddIdentityApiEndpoints<ApplicationUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddScoped<IEventReadService, EventReadService>();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();
app.MapIdentityApi<ApplicationUser>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowReact");
app.UseAuthorization();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
