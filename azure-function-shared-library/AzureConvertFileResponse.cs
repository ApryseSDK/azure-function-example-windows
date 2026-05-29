
namespace azure_function_shared_library
{
    public class AzureConvertFileResponse
    {
        /// <summary>
        /// The file in Azure storage to process. This is for the file name and does not include the virtual path.
        /// </summary>
        public string? OutputFile { get; set; }
        /// <summary>
        /// The Azure storage container for the output.
        /// </summary>
        public string? StorageContainer { get; set; }
        /// <summary>
        /// The virtual path for the fileToConvert and output.
        /// </summary>
        public string? StorageVirtualPath { get; set; }
    }
}
