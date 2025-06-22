using Hangfire;
using Lab11_MiguelQuispe.Application.Configuration;
using Lab11_MiguelQuispe.Application.Mapping;
using Lab11_MiguelQuispe.Domain.Interfaces;
using Lab11_MiguelQuispe.Infraestructure.configuration;

var builder = WebApplication.CreateBuilder(args);

// Configuración de servicios
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApplicationServices();
builder.Services.AddAutoMapper(typeof(MappingProfile));

var app = builder.Build();

// Middlewares
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.UseHangfireDashboard("/hangfire");

// JOBS programados (ejemplo)
RecurringJob.AddOrUpdate<IDataCleanupService>(
    "job-limpieza-datos",
    service => service.CleanOldData(),
    Cron.Daily);

// Archivos estáticos
app.MapStaticAssets();

// Rutas MVC (siguen funcionando, pero la raíz será Swagger)
app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// SWAGGER COMO PÁGINA PRINCIPAL (en "/")
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ticketera API V1");
    c.RoutePrefix = string.Empty; // <- Esto hace que Swagger sea el home
});

app.Run();