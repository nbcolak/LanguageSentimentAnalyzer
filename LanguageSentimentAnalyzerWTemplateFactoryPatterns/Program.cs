using TranslateSentimentProcessorWTemplateFactoryPatterns.Factory;
using TranslateSentimentProcessorWTemplateFactoryPatterns.Template;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllersWithViews();

builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
});
builder.Services.Configure<AzureTextAnalyticsSettings>(builder.Configuration.GetSection("TextAnalytics"));
builder.Services.Configure<GoogleApiSettings>(builder.Configuration.GetSection("GoogleApiSettings"));


builder.Services.AddScoped<TextProcessingFactory>();
builder.Services.AddScoped<GoogleLanguageAzureSentiment>();
builder.Services.AddScoped<AzureLanguageGoogleSentiment>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapControllers(); 



app.Run();

