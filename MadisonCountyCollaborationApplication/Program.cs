using MadisonCountyCollaborationApplication.Pages.DataClasses;
using Azure.Storage.Blobs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddSession();
builder.Services.AddScoped<WhiteListService>(); // Existing registration of WhiteListService

// Corrected to match the updated appsettings.json structure
var azureBlobStorageConfig = builder.Configuration.GetSection("AzureBlobStorage");
if (azureBlobStorageConfig.Exists()) // Ensure the configuration section exists
{
    // Register BlobServiceClient with the DI container using the retrieved connection string
    builder.Services.AddSingleton(new BlobServiceClient(azureBlobStorageConfig["ConnectionString"]));
}
else
{
    throw new InvalidOperationException("Azure Blob Storage configuration section is missing in appsettings.json");
}

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
