
namespace azure_function_app_windows.Interfaces
{
    public interface IFileConversionService
    {
        Task ConvertFileAsync(
            string InputFilePath,
            FunctionPaths Paths,
            CancellationToken CancellationToken);
    }
}
