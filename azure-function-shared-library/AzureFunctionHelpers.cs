
using Azure.Storage.Blobs;

namespace azure_function_shared_library
{
    public class AzureFunctionHelpers
    {
        /// <summary>
        /// Deletes the blob from the specified Azure storage Container.
        /// </summary>
        /// <param name="AzureConnectionString">Connection string for the Azure storage.</param>
        /// <param name="Container">The Container containing the blob to delete.</param>
        /// <param name="VirtualPath">The virtual path plus file name to be removed.</param>        
        /// <returns></returns>
        public static async Task<Azure.Response<bool>> DeleteBlobAsync(
            string AzureConnectionString,
            string Container,
            string VirtualPath)
        {
            BlobContainerClient containerClient =
                new BlobServiceClient(AzureConnectionString)
                    .GetBlobContainerClient(Container);
            var response = await containerClient
                .GetBlobClient(VirtualPath)
                .DeleteIfExistsAsync();
            return response;
        }

        /// <summary>
        /// Downloads the blob from the specified Azure storage Container.
        /// </summary>
        /// <param name="AzureConnectionString">Connection string for the Azure storage.</param>
        /// <param name="Container">The Container containing the blob to download.</param>
        /// <param name="VirtualPath">The virtual path plus file name to be downloaded.</param>
        /// <param name="DownloadLocation">The path/file name to download the file.</param>
        /// <returns></returns>
        public static async Task<Azure.Response> DownloadBlobAsync(
            string AzureConnectionString,
            string Container,
            string VirtualPath,
            string DownloadLocation)
        {
            BlobContainerClient containerClient =
                new BlobServiceClient(AzureConnectionString)
                    .GetBlobContainerClient(Container);
            BlobClient blob_client = containerClient.GetBlobClient(VirtualPath);
            var response = await containerClient
                .GetBlobClient(VirtualPath)
                .DownloadToAsync(DownloadLocation);
            return response;
        }

        /// <summary>
        /// Uploads the file to the specified Azure storage Container, placing it in the virtual path
        /// if set.
        /// Ex. 1a2ee70b-b7c7-4dd0-a2dc-23602f9a6211/Fishermen.docx
        /// </summary>
        /// <param name="AzureConnectionString">Connection string for the Azure storage.</param>
        /// <param name="Container">The Azure storage Container to upload to.</param>
        /// <param name="FileToUpload">The full path to the file to upload.</param>
        /// <param name="VirtualPath">The virtual path in the Container to store the file.</param>
        /// <param name="Overwrite">Overwrite any existing file, default is true.</param>
        /// <returns></returns>
        public static async Task<Azure.Response<Azure.Storage.Blobs.Models.BlobContentInfo>> UploadFileAsync(
            string AzureConnectionString,
            string Container,
            string FileToUpload,
            string VirtualPath,
            Boolean Overwrite = true)
        {
            string blobName = System.IO.Path.GetFileName(FileToUpload);
            BlobContainerClient containerClient =
                new BlobServiceClient(AzureConnectionString)
                    .GetBlobContainerClient(Container);
            blobName =
                String.IsNullOrEmpty(VirtualPath) ? blobName : $"{VirtualPath}/{blobName}";
            BlobClient blobClient = containerClient.GetBlobClient(blobName);
            var response = await blobClient.UploadAsync(
                FileToUpload,
                overwrite: Overwrite);
            return response;
        }
    }
}
