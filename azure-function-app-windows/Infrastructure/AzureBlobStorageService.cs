
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using azure_function_app_windows.Interfaces;
using azure_function_shared_library;
using System.IO.Compression;

namespace azure_function_app_windows.Infrastructure
{
    public sealed class AzureBlobStorageService : IStorageService
    {
        public async Task<string> DownloadAsync(
            String AzureConnectionString,
            String Container,
            String VirtualPath,
            String DestinationPath,
            CancellationToken CancellationToken)
        {
            try
            {
                await AzureFunctionHelpers.DownloadBlobAsync(
                    AzureConnectionString,
                    Container,
                    VirtualPath,
                    DestinationPath);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return DestinationPath;
        }

        public async Task UploadZipAsync(
            String AzureConnectionString,
            String Container,
            String VirtualPath,
            String SourceDirectory,
            CancellationToken CancellationToken)
        {
            BlobClient client =
                new BlobServiceClient(AzureConnectionString)
                .GetBlobContainerClient(Container)
                .GetBlobClient(VirtualPath);

            await using Stream stream =
                await client.OpenWriteAsync(
                    true,
                    new BlobOpenWriteOptions
                    {
                        HttpHeaders = new BlobHttpHeaders
                        {
                            ContentType = "application/zip"
                        }
                    },
                    CancellationToken);

            using ZipArchive zip =
                new ZipArchive(stream, ZipArchiveMode.Create);

            foreach (string file in Directory.GetFiles(SourceDirectory))
            {
                ZipArchiveEntry entry =
                    zip.CreateEntry(Path.GetFileName(file));
                await using Stream entry_stream = entry.Open();
                await using FileStream fileStream =
                    File.OpenRead(file);
                await fileStream.CopyToAsync(entry_stream, CancellationToken);
            }
        }
    }
}
