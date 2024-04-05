using MadisonCountyCollaborationApplication.Pages.DataClasses;
using Azure.Storage.Blobs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddSession();
builder.Services.AddScoped<WhiteListService>(); // Existing registration of WhiteListService

// Retrieve the connection string from appsettings.json and register BlobServiceClient with the DI container
var azureFileStorageConfig = builder.Configuration.GetSection("AzureFileStorage");
builder.Services.AddSingleton(new BlobServiceClient(azureFileStorageConfig["ConnectionString"]));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
app.UseStaticFiles();
app.UseSession();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();
app.Run();