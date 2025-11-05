using chatbot.Data;
using chatbot.Interfaces;
using chatbot.Repositories;
using chatbot.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseNpgsql(connectString, o => o.UseVector());
});

builder.Services.AddScoped<IChatRepository, ChatRepository>();
builder.Services.AddScoped<IEmbeddingRepository, EmbeddingRepository>();
builder.Services.AddScoped<IDataService, SpreadsheetService>();
builder.Services.AddScoped<IChatService, ChatGeminiService>();
builder.Services.AddScoped<IEmbeddingService, EmbeddingGeminiService>();
builder.Services.AddScoped<IEmbeddingDataService, EmbeddingDataService>();
builder.Services.AddScoped<VectorSearchService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

