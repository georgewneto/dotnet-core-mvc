using Microsoft.EntityFrameworkCore;
using Projeto.Infraestrutura.Data;
using Projeto.Infraestrutura.Repositories;
using Projeto.RegrasDeNegocio.Interfaces;
using Projeto.RegrasDeNegocio.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configuração do SQLite
builder.Services.AddDbContext<Projeto.Infraestrutura.Data.ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registrar os repositórios
builder.Services.AddScoped<Projeto.RegrasDeNegocio.Interfaces.IEstadoRepository, Projeto.Infraestrutura.Repositories.EstadoRepository>();
builder.Services.AddScoped<Projeto.RegrasDeNegocio.Interfaces.ICidadeRepository, Projeto.Infraestrutura.Repositories.CidadeRepository>();

// Registrar os serviços
builder.Services.AddScoped<Projeto.RegrasDeNegocio.Interfaces.IEstadoService, Projeto.RegrasDeNegocio.Services.EstadoService>();
builder.Services.AddScoped<Projeto.RegrasDeNegocio.Interfaces.ICidadeService, Projeto.RegrasDeNegocio.Services.CidadeService>();

var app = builder.Build();

// Aplicar migrações automaticamente
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
}

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
