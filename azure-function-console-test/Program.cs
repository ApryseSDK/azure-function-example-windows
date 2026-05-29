
using Azure;
using azure_function_shared_library;
using System.IO.Compression;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace azure_function_console_test
{
    internal class Program
    {
#if DEBUG
        private static string _functionURL = "";
#else
        private static string _functionURL = "";
#endif
        private static string _azureStorageConnString = "";
        private static string _conversionUploadsContainer = "";
        private static string _conversionOutputContainer = "";

        static async Task<int> Main(string[] args)
        {
            if (String.IsNullOrEmpty(_functionURL))
            {
                Console.WriteLine("Function URL not set. Provide a valid Azure Function URL with authorization code to process files. Example: https://azure-function.azurewebsites.net/api/ConvertFile?code=##################, exiting.");
                return 1;
            }
            if (String.IsNullOrEmpty(_azureStorageConnString))
            {
                Console.WriteLine("Azure connection string for storage container not set. Please provide a valid Azure blob storage connection string for your blob storage, exiting.");
                return 1;
            }
            if (String.IsNullOrEmpty(_conversionUploadsContainer))
            {
                Console.WriteLine("Azure storage container for file uploads not set. Please provide a valid Azure blob storage location for uploading conversion files, exiting.");
                return 1;
            }
            if (String.IsNullOrEmpty(_conversionOutputContainer))
            {
                Console.WriteLine("Azure storage container for converted files not set. Please provide a valid Azure blob storage location for uploading converted files, exiting.");
                return 1;
            }

#if DEBUG
            // Delay running the conversion test until the local function test has started.
            Console.WriteLine("Launching function in debug mode. Press any key to continue once it is loaded.");
            Console.ReadKey();
#endif
            string fileToConvert = @"";
            string outputDirectory = @"";
            if (!File.Exists(fileToConvert))
            {
                Console.WriteLine($"Conversion file not found: {fileToConvert}");
                return 1;
            }
            if (!Directory.Exists(outputDirectory))
            {
                Console.WriteLine($"Conversion output directory not found: {outputDirectory}");
                return 1;
            }

            string fileName = Path.GetFileName(fileToConvert);
            string uploadFile = System.IO.Path.GetFileName(fileName);

            try
            {                              
                #region Upload to Azure Storage
                Console.WriteLine("********************************************************************************");
                Console.WriteLine("Uploading conversion file to storage.");
                string virtualPath = Guid.NewGuid().ToString();
                try
                {
                    var uploadResponse = await AzureFunctionHelpers.UploadFileAsync(
                        _azureStorageConnString,
                        _conversionUploadsContainer,
                        fileToConvert,
                        virtualPath);
                    Console.WriteLine($"Upload successful!");

                }
                catch (RequestFailedException ex)
                {
                    Console.WriteLine($"Azure upload failed:");
                    Console.WriteLine($"  Status: {ex.Status}");
                    Console.WriteLine($"  ErrorCode: {ex.ErrorCode}");
                    Console.WriteLine($"  Message: {ex.Message}");
                    return 1;

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Azure upload failed: {ex.Message}");
                    return 1;
                }
                Console.WriteLine("Upload complete.");
                Console.WriteLine("********************************************************************************");
                Console.WriteLine();
                #endregion Upload to Azure Storage

                #region Process File to Convert
                Console.WriteLine("********************************************************************************");
                Console.WriteLine($"Converting {fileToConvert} to PDF.");

                var payLoad = new AzureConvertFileRequest
                {
                    Input = new Input
                    {
                        AzureBlobStorage = new AzureInputBlobStorage
                        {
                            AzureConnString = _azureStorageConnString,
                            InputFileName = System.IO.Path.GetFileName(fileToConvert),
                            InputFileContainerName = _conversionUploadsContainer,
                            InputFileVirtualPath = virtualPath
                        }
                    },
                    Output = new Output
                    {
                        AzureBlobStorage = new AzureOutputBlobStorage
                        {
                            AzureConnString = _azureStorageConnString,
                            OutputFileContainerName = _conversionOutputContainer,
                            OutputFileVirtualPath = virtualPath
                        }
                    }
                };

                var json = JsonSerializer.Serialize(payLoad);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                using var client = new HttpClient();
                using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5));
                var response = await client.PostAsync(_functionURL, content, cts.Token);

                AzureConvertFileResponse? result = null;
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Successfully converted file: {response.ToString()}");
                    result = await response.Content.ReadFromJsonAsync<AzureConvertFileResponse>();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"File conversion failed.");
                    Console.WriteLine($"  Status: {(int)response.StatusCode} ({response.ReasonPhrase})");
                    Console.WriteLine($"  Body: {errorContent}");
                }                
                Console.WriteLine("********************************************************************************");
                Console.WriteLine();
                #endregion Process File to Convert

                #region Download output from Azure Storage and unzip
                Console.WriteLine("********************************************************************************");
                Console.WriteLine("Downloading output file from storage.");
                try
                {
                    string downloadFile = $"{outputDirectory}AZURE_{result.OutputFile}";
                    string containerPath = $"{result.StorageVirtualPath}/{result.OutputFile}";
                    var download_response = await AzureFunctionHelpers.DownloadBlobAsync(
                        _azureStorageConnString,
                        result.StorageContainer,
                        containerPath,
                        downloadFile);

                    Console.WriteLine($"Extracting output {downloadFile} to {outputDirectory}");
                    ZipFile.ExtractToDirectory(downloadFile, outputDirectory, true);
                    System.IO.File.Delete(downloadFile);
                }
                catch (Azure.RequestFailedException ex) when (ex.Status == 404)
                {
                    Console.WriteLine($"Azure output download failed: Blob not found.");
                }
                catch (RequestFailedException ex)
                {
                    Console.WriteLine($"Azure output download failed:");
                    Console.WriteLine($"  Status: {ex.Status}");
                    Console.WriteLine($"  ErrorCode: {ex.ErrorCode}");
                    Console.WriteLine($"  Message: {ex.Message}");

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Azure output download failed: {ex.Message}");
                }
                Console.WriteLine("********************************************************************************");
                Console.WriteLine();
                #endregion Download Blob from Azure Storage

                #region Delete Files from Azure Storage
                Console.WriteLine("********************************************************************************");
                Console.WriteLine("Deleting input and output file from Azure storage.");
                try
                {
                    // Delete the original file to be converted.
                    string fileToDelete = $"{virtualPath}/{uploadFile}";
                    var deleteResponse = await AzureFunctionHelpers.DeleteBlobAsync(
                        _azureStorageConnString,
                        _conversionUploadsContainer,
                        fileToDelete);
                    if (deleteResponse.Value != true)
                    {
                        Console.WriteLine($"Azure Delete Blob Failed: {fileToDelete}");
                        Console.WriteLine($" Status: {deleteResponse.GetRawResponse().ToString()}");
                    }

                    // Delete the generated output file.
                    fileToDelete = $"{result.StorageVirtualPath}/{result.OutputFile}";
                    deleteResponse = await AzureFunctionHelpers.DeleteBlobAsync(
                        _azureStorageConnString,
                        result.StorageContainer,
                        fileToDelete);
                    if (deleteResponse.Value != true)
                    {
                        Console.WriteLine($"Azure Delete Blob Failed: {fileToDelete}");
                        Console.WriteLine($" Status: {deleteResponse.GetRawResponse().ToString()}");
                    }

                }
                catch (RequestFailedException ex)
                {
                    Console.WriteLine($"Azure Delete Blob Failed");
                    Console.WriteLine($"  Status: {ex.Status}");
                    Console.WriteLine($"  ErrorCode: {ex.ErrorCode}");
                    Console.WriteLine($"  Message: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                Console.WriteLine("********************************************************************************");
                Console.WriteLine();
                #endregion Download File from Azure Storage

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR:");
                Console.WriteLine(ex.Message);
                return 1;
            }
        }
    }
}