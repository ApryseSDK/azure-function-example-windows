
using azure_function_app_windows.Interfaces;

namespace azure_function_app_windows.Infrastructure
{
    public sealed class CleanupService : ICleanupService
    {
        public async Task CleanupAsync(
            FunctionPaths? Paths,
            CancellationToken CancellationToken)
        {
            if (Paths is null) return;

            await Task.WhenAll(
                DeleteAsync(Paths.InputFilePath, CancellationToken),
                DeleteAsync(Paths.OutputFilePath, CancellationToken));
        }

        private static Task DeleteAsync(
            String? Path,
            CancellationToken CancellationToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Path) ||
                    !Directory.Exists(Path))
                    return Task.CompletedTask;

                return Task.Run(() =>
                {
                    Directory.Delete(Path, true);
                }, CancellationToken);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

}
