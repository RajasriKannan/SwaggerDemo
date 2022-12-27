using Azure.Identity;
using Microsoft.Extensions.Azure;
var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddAzureKeyVault($"https://{builder.Configuration["KeyVault:Vault"]}.vault.azure.net/", builder.Configuration["KeyVault:ClientId"], builder.Configuration["KeyVault:ClientSecret"]);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApplicationInsightsTelemetry();
builder.Services.AddAzureClients(clientBuilder =>
{
    clientBuilder.AddBlobServiceClient(builder.Configuration["ConnectionStrings:AzureStorageConn:blob"], preferMsi: true);
    clientBuilder.AddQueueServiceClient(builder.Configuration["ConnectionStrings:AzureStorageConn:queue"], preferMsi: true);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();

app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
