
using azure_function_shared_library;

namespace azure_function_app_windows.Interfaces
{
    public interface IConvertFile
    {
        Task<AzureConvertFileResponse> ExecuteAsync(
            FunctionPaths Paths,
            AzureConvertFileRequest Request,
            CancellationToken CancellationToken);
    }
}