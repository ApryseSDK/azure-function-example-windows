
namespace azure_function_app_windows.Interfaces
{
    public interface IStorageService
    {
        Task<string> DownloadAsync(
            string AzureConnectionString,
            string Container,
            string VirtualPath,
            string DestinationPath,
            CancellationToken CancellationToken);

        Task UploadZipAsync(
            string AzureConnectionString,
            string Container,
            string VirtualPath,
            string SourceDirectory,
            CancellationToken CancellationToken);
    }
}
