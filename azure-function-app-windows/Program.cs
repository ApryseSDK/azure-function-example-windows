
using azure_function_app_windows.Infrastructure;
using azure_function_app_windows.Interfaces;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using pdftron;

string? license = Environment.GetEnvironmentVariable("PDFNET_SDK_LICENSE");
if (string.IsNullOrWhiteSpace(license))
{
    throw new InvalidOperationException("PDFNET_SDK_LICENSE not configured");
}
PDFNet.Initialize(license);

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.TryAddScoped<IConvertFile, ConvertFile>();
        services.TryAddSingleton<IFileConversionService, ApryseConversionService>();
        services.TryAddScoped<IStorageService, AzureBlobStorageService>();
        services.TryAddScoped<ICleanupService, CleanupService>();
    })
    .Build();

host.Run();


