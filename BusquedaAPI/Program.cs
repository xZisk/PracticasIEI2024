using System.Reflection;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "BusquedaAPI", Version = "v1" });
    c.EnableAnnotations();  // Habilita anotaciones
    // Incluir comentarios XML en Swagger
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});
builder.Services.AddHttpClient();

// Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost3000",
        builder =>
        {
            builder.WithOrigins("http://localhost:3000")
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials();
        });
});

var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "BusquedaAPI v1");
    c.RoutePrefix = string.Empty; // Mostrar Swagger en la ra�z
});

app.UseAuthorization();

app.UseCors("AllowLocalhost3000"); 

app.MapControllers(); 

app.Run();
