# Azure Function Example for Windows
This project provides a sample Azure Function that demonstrates how to convert Microsoft Word documents into PDF format using the Apryse PDFNet SDK. The function is invoked via an HTTP trigger and secured with function-level authentication. Designed for a Windows-based environment, this solution serves as a reference implementation for secure, serverless document processing workflows.

# Included Projects
  # azure-function-app-windows
  This Azure Function application is intended for deployment to your Azure environment. It implements an HTTP-triggered function secured with function-level authentication, using a function key passed in the request URL. This authentication model can be adjusted to align with your organization’s security requirements.
  * The sample function relies on two environment variables required by the Apryse PDFNet SDK:
    * PDFNET_SDK_LICENSE
      * This is your Apryse PDFNet SDK license.
    * AZURE_TEMP_DIRECTORY
      * This is the temporary directory for the input/output files. You should use "D:\local\temp\" which is the ephemeral storage for function instances.

  # azure-function-console-test
  A console application used to test the Azure Function end-to-end. It performs the following steps:
  Uploads the input Word document to Azure Blob Storage, invokes the Azure Function via HTTP, and downloads the generated PDF from Azure Blob Storage
  * Configuration requirements:
    * Function URL (including function key)
    * Azure Storage connection string
    * Input and output Blob container names

  # azure-function-shared-library
  A shared class library used by both the function app and the console test project.
  * Provides common configuration settings and includes helper utilities for interacting with Azure Blob Storage (upload, download, delete operations)

# Publishing
This function has been tested and can be deployed using either the **Consumption** or **Premium** Azure Function plans. 
When selecting a plan, consider:
- **Performance requirements** (processing time and throughput)
- **Document complexity and size**
- **Scaling and concurrency needs**
Choose the plan that best aligns with your workload and expected usage.
* You can find more information on Azure hosting options here:
  * https://learn.microsoft.com/en-us/azure/azure-functions/functions-overview 

# Extension Modules
The Aprye PDFNet SDK implements some extension modules to perform certain conversions; OCR, CAD, and Microsoft Office to PDF to name a few. The following modules have been confirmed to work in a Windows environment.
*  OCR Module
   * https://docs.apryse.com/core/guides/ocr
* Structured Output Module
  *  https://docs.apryse.com/core/guides/features/office/convert-to-office
* CAD Module
  *  https://docs.apryse.com/core/guides/features/conversion/convert-cad-to-pdf
* Data Extraction Module
  *  https://docs.apryse.com/core/guides/smart-data-extraction
* PDF2HTML Reflow Paragraph Module
  *  https://docs.apryse.com/core/guides/features/conversion/convert-pdf-to-html
Note: The Windows HTML2PDF module will not run in a Azure function application due to permission restrictions in the Azure Windows environment. It is suggested to use a Linux container in this case.

The included function example includes only Word to PDF conversions, to use the extension modules they are required to be located in the function ephemeral location.

