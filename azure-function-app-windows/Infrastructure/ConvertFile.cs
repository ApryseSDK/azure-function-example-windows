
using azure_function_app_windows.Interfaces;
using azure_function_shared_library;
using Microsoft.Extensions.Logging;

namespace azure_function_app_windows.Infrastructure
{
    public sealed class ConvertFile : IConvertFile
    {
        private readonly ILogger<ArpyseConvertFileFunction> _logger;
        private readonly IFileConversionService _converter;
        private readonly IStorageService _storage;

        public ConvertFile(
            ILogger<ArpyseConvertFileFunction> Logger,
            IStorageService Storage,
            IFileConversionService Converter)
        {
            _storage = Storage;
            _converter = Converter;
            _logger = Logger;
        }

        public async Task<AzureConvertFileResponse> ExecuteAsync(
            FunctionPaths Paths,
            AzureConvertFileRequest Request,
            CancellationToken CancellationToken)
        {
            var inputStorage = Request?.Input?.AzureBlobStorage;
            string inputFile = Path.Combine(
                Request?.Input?.AzureBlobStorage.InputFileVirtualPath,
                Request?.Input?.AzureBlobStorage.InputFileName
            );

            string outputFile = Path.Combine(
                Paths.InputFilePath,
                inputStorage.InputFileName
            );

            inputFile = await _storage.DownloadAsync(
                Request.Input.AzureBlobStorage.AzureConnString,
                Request.Input.AzureBlobStorage.InputFileContainerName,
                inputFile,
                outputFile,
                CancellationToken
            );

            _logger.LogInformation($"Converting file: {inputFile}");
            await _converter.ConvertFileAsync(
                inputFile,
                Paths,
                CancellationToken);

            _logger.LogInformation($"Uploading output file");
            await _storage.UploadZipAsync(
                Request.Output.AzureBlobStorage.AzureConnString,
                Request.Output.AzureBlobStorage.OutputFileContainerName,
                Path.Combine(
                    Request.Output.AzureBlobStorage.OutputFileVirtualPath,
                    $"{Request.Output.AzureBlobStorage.OutputFileVirtualPath}.output.zip"),
                Paths.OutputFilePath,
                CancellationToken);

            return new AzureConvertFileResponse
            {
                OutputFile = $"{Request.Output.AzureBlobStorage.OutputFileVirtualPath}.output.zip",
                StorageContainer = Request.Output.AzureBlobStorage.OutputFileContainerName,
                StorageVirtualPath = Request.Output.AzureBlobStorage.OutputFileVirtualPath
            };
        }
    }

}
