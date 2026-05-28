
namespace azure_function_app_windows.Interfaces
{
    public interface ICleanupService
    {
        Task CleanupAsync(FunctionPaths? Paths, CancellationToken CancellationToken);
    }
}
