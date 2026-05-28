
using azure_function_app_windows.Interfaces;
using azure_function_shared_library;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using pdftron.PDF;
using System.Net;
using System.Text.Json;

namespace azure_function_app_windows;

public class ArpyseConvertFileFunction
{
    private readonly ILogger<ArpyseConvertFileFunction> _logger;
    private readonly IConvertFile _convertFile;
    private readonly ICleanupService _cleanup;

    public ArpyseConvertFileFunction(
        ILogger<ArpyseConvertFileFunction> Logger,
        IConvertFile ConvertFile,
        ICleanupService Cleanup)
    {
        _logger = Logger;
        _convertFile = ConvertFile;
        _cleanup = Cleanup;
    }

    [Function("ConvertFile")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData Request,
        FunctionContext executionContext)
    {
        _logger.LogInformation(
            "ConvertFile invocation started. InvocationId={InvocationId}",
            executionContext.InvocationId);

        var response = Request.CreateResponse();
        FunctionPaths? paths = null;

        try
        {
            var request =
                await Request.ReadFromJsonAsync<AzureConvertFileRequest>(
                    cancellationToken: executionContext.CancellationToken);

            var requestJson = JsonSerializer.Serialize(request);
            _logger.LogInformation($"Requst Payload: {requestJson}");

            if (request is null)
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                await response.WriteStringAsync("Invalid or missing request body.");
                return response;
            }

            paths = InitializeFunction(request!, executionContext.InvocationId.ToString());

            var result =
                await _convertFile.ExecuteAsync(
                    paths,
                    request!,
                    executionContext.CancellationToken);

            response.StatusCode = HttpStatusCode.OK;
            await response.WriteAsJsonAsync(result);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Invalid JSON payload");
            await response.WriteAsJsonAsync(new
            {
                error = $"Invalid JSON payload: {ex.Message}",
                status = HttpStatusCode.BadRequest
            });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Configuration error");
            await response.WriteAsJsonAsync(new
            {
                error = $"Configuration error: {ex.Message}",
                status = HttpStatusCode.FailedDependency
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled error");
            await response.WriteAsJsonAsync(new
            {
                error = $"Unhandled error: {ex.Message}",
                status = HttpStatusCode.InternalServerError
            });
            response.StatusCode = HttpStatusCode.InternalServerError;
        }
        finally
        {
            await _cleanup.CleanupAsync(paths, executionContext.CancellationToken);
        }
        return response;
    }

    #region Helper Functions
    /// <summary>
    /// Create any temporary directories needed.
    /// </summary>
    public FunctionPaths InitializeFunction(AzureConvertFileRequest ConversionSettings, String RequestID)
    {
        FunctionPaths result = new FunctionPaths();
        _logger.LogInformation("Initializing function.");
        string basePath = string.Empty;
#if DEBUG
        basePath = System.AppDomain.CurrentDomain.BaseDirectory;
#else
        basePath = $@"{Environment.GetEnvironmentVariable("AZURE_TEMP_DIRECTORY")}";
#endif
        result.InputFilePath = $@"{Path.Combine(basePath, "input_files", RequestID)}";
        result.OutputFilePath = $@"{Path.Combine(basePath, "output_files", RequestID)}";

        try
        {
            _logger.LogInformation($"Input file path : {result.InputFilePath}");
            if (!System.IO.Directory.Exists(result.InputFilePath))
            {
                System.IO.Directory.CreateDirectory(result.InputFilePath);
            }
            _logger.LogInformation($"Output File Path: {result.OutputFilePath}");
            if (!System.IO.Directory.Exists(result.OutputFilePath))
            {
                System.IO.Directory.CreateDirectory(result.OutputFilePath);
            }            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Excepiton caught initializing function.");
            throw;
        }
        finally
        {
            _logger.LogInformation("Function initialization complete.");
        }
    }
    #endregion Helper Functions
}