
using System.Text.Json.Serialization;

namespace azure_function_shared_library
{
    public class AzureConvertFileRequest
    {
        [JsonPropertyName("input")]
        public Input? Input { get; set; }

        [JsonPropertyName("output")]
        public Output? Output { get; set; }
    }

    public class Input
    {
        [JsonPropertyName("azureBlobStorage")]
        public AzureInputBlobStorage? AzureBlobStorage { get; set; }
    }

    public class Output
    {
        [JsonPropertyName("azureBlobStorage")]
        public AzureOutputBlobStorage? AzureBlobStorage { get; set; }
    }

    public class AzureModuleBlobStorage
    {
        [JsonPropertyName("azureConnString")]
        public string? AzureConnString { get; set; }

        [JsonPropertyName("moduleContainer")]
        public string? ModuleContainer { get; set; }

        [JsonPropertyName("moduleVirutalPath")]
        public string? ModuleVirutalPath { get; set; }
    }

    public class AzureInputBlobStorage
    {
        [JsonPropertyName("azureConnString")]
        public string? AzureConnString { get; set; }

        [JsonPropertyName("inputFileName")]
        public string? InputFileName { get; set; }

        [JsonPropertyName("inputFileContainerName")]
        public string? InputFileContainerName { get; set; }

        [JsonPropertyName("inputFileVirtualPath")]
        public string? InputFileVirtualPath { get; set; }
    }

    public class AzureOutputBlobStorage
    {
        [JsonPropertyName("azureConnString")]
        public string? AzureConnString { get; set; }

        [JsonPropertyName("outputFileContainerName")]
        public string? OutputFileContainerName { get; set; }

        [JsonPropertyName("outputFileVirtualPath")]
        public string? OutputFileVirtualPath { get; set; }
    }
}
