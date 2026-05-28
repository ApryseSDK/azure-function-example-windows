
using azure_function_app_windows.Interfaces;
using Microsoft.Extensions.Logging;
using pdftron.Common;

namespace azure_function_app_windows.Infrastructure
{
    public sealed class ApryseConversionService : IFileConversionService
    {
        private readonly ILogger<ApryseConversionService> _logger;

        public ApryseConversionService(
            ILogger<ApryseConversionService> Logger)
        {
            _logger = Logger;
        }

        public async Task ConvertFileAsync(
            string InputFilePath,
            FunctionPaths Paths,
            CancellationToken CancellationToken)
        {
            await Task.Run(() =>
            {
                CancellationToken.ThrowIfCancellationRequested();

                _logger.LogInformation(
                    "Converting file {File}.", InputFilePath);

                try
                {
                    string originalFileName = Path.GetFileName(InputFilePath);
                    string pdfnetSDKVersion = pdftron.PDFNet.GetVersionString();
                    using (pdftron.PDF.PDFDoc pdfDoc = new())
                    {
                        string outputFile = Path.Combine(Paths.OutputFilePath, originalFileName) + $"_{pdfnetSDKVersion}.pdf";
                        var options = new pdftron.PDF.OfficeToPDFOptions();
                        pdftron.PDF.Convert.OfficeToPDF(pdfDoc, InputFilePath, options);
                        pdfDoc.Save(outputFile, pdftron.SDF.SDFDoc.SaveOptions.e_linearized);
                    }
                }
                catch (PDFNetException ex)
                {
                    _logger.LogError(ex, "PDFNet exception converting file: {Path}", InputFilePath);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Exception converting file: {Path}", InputFilePath);
                }

            }, CancellationToken);
        }                
    }
}
