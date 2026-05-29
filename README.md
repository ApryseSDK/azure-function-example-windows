# Azure Function Example for Windows
This project provides a sample Azure Function that demonstrates how to convert Microsoft Word documents into PDF format using the Apryse PDFNet SDK. The function is invoked via an HTTP trigger and secured with function-level authentication. Designed for a Windows-based environment, this solution serves as a reference implementation for secure, serverless document processing workflows.

# Included Projects
  # azure-function-app-windows
  The Azure Function application intended for deployment to your Azure environment.
  Implements an HTTP-triggered function secured with function-level authentication (via function key in the request URL). This authentication model can be modified to meet your security requirements.

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

# Extension Modules

# Azure Service Plans
